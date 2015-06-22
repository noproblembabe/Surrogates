﻿using NUnit.Framework;
using System;
using System.Threading;

namespace Surrogates.Applications.Tests
{
    [TestFixture]
    public class CacheTests : AppTests
    {
        [Test]
        public void SimpleCacheValueTest()
        {
            Container.Map(m => m
                .From<Simple>("SimpleCachedTest")
                .Apply
                .Cache(s => (Func<int>)s.GetRandom, timeout: TimeSpan.FromMilliseconds(500)));

            var proxy =
                Container.Invoke<Simple>("SimpleCachedTest");

            var simple = new Simple();

            var pRdn1 = proxy.GetRandom();
            var pRdn2 = proxy.GetRandom();

            Thread.Sleep(600);

            var pRdn3 = proxy.GetRandom();

            Assert.AreEqual(pRdn1, pRdn2);
            Assert.AreNotEqual(pRdn1, pRdn3);
            Assert.AreNotEqual(pRdn2, pRdn3);
        }

        [Test]
        public void SimpleCacheReferenceTest()
        {
            Container.Map(m => m
                .From<Simple>("SimpleCachedTest")
                .Apply
                .Cache(s => (Func<object>)s.GetNewObject, timeout: TimeSpan.FromMilliseconds(500)));

            var proxy =
                Container.Invoke<Simple>("SimpleCachedTest");

            var simple = new Simple();

            var obj1 = proxy.GetNewObject();
            var obj2 = proxy.GetNewObject();

            Thread.Sleep(600);

            var obj3 = proxy.GetNewObject();

            Assert.AreEqual(obj1.GetHashCode(), obj2.GetHashCode());
            Assert.AreNotEqual(obj1.GetHashCode(), obj3.GetHashCode());
            Assert.AreNotEqual(obj2.GetHashCode(), obj3.GetHashCode());
        }
    }
}