﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Surrogates.Aspects.Tests
{
    [TestFixture]
    public class InterlockingTests : AppTestsBase
    {
        [Test]
        public void SimpleLock4Methods()
        {
            Container.Map(m =>
                m.From<Simpleton>()
                .Apply
                .ReadAndWrite(s => (Func<int, int>)s.GetFromList, s => (Action<int>)s.Add2List));

            var max = 15000000;

            var proxy = Container
                .Invoke<Simpleton>(args: new List<int>(capacity: max));

            var simple = new Simpleton(new List<int>(capacity: max));


            Action simpleTask =
                () =>
                    Parallel.For(0, max, i => simple.Add2List(i));

            Action safeTask =
                () =>
                    Parallel.For(0, max, i => proxy.Add2List(i));

            var tasks =
                new[] {
                    Task.Run(simpleTask),
                    Task.Run(safeTask)
                };

            Task.WaitAll(tasks);

            int count = 0;

            simple.List.Sort();
            proxy.List.Sort();

            while (simple.List.Count >= count && proxy.List.Count >= count && simple.GetFromList(count) == proxy.GetFromList(count))
            { count++; }

            Assert.AreNotEqual(count, max - 1);
        }
    }
}