﻿using System;
using NUnit.Framework;
using Surrogates.Tests.Simple.Entities;
using Surrogates.Utilities;

namespace Surrogates.Tests.Simple._Properties
{
    [TestFixture]
    public class Property_VisitTests
    {
        [Test, ExpectedException(typeof(NotSupportedException))]
        public void WithRegularPropertyAccessors()
        {
            var container =
                new SurrogatesContainer();

            container.Map(m => m
                .From<Dummy>()
                .Visit
                .This(d => d.AccessItWillThrowException)
                .Accessors(a =>
                {
                    With.OneSimpleGetter(a);
                    With.OneSimpleSetter(a);
                }));
        }

        [Test]
        public void OnlyGetter_SetterIsDefault()
        {
            var container =
                new SurrogatesContainer();

            container.Map(m => m
                .From<Dummy>()
                .Visit
                .This(d => d.AccessItWillThrowException)
                .Accessors(a =>
                    a.Getter.Using<InterferenceObject>(d => (Func<Dummy, string, int>) d.SetPropText_InstanceAndMethodName_Return2)));

            var proxy =
                container.Invoke<Dummy>();

            Except(
                () => proxy.AccessItWillThrowException = 2,
                () => { int res = proxy.AccessItWillThrowException; });

            Assert.IsTrue(proxy.Text.Contains("Dummy"));
        }

        [Test]
        public void OnlySetter_GetterIsDefault()
        {
            var container =
                new SurrogatesContainer();

            container.Map(m => m
                .From<Dummy>()
                .Visit
                .This(d => d.AccessItWillThrowException)
                .Accessors(a =>
                    a.Setter.Using<InterferenceObject>(d => (Func<int, Dummy, int>) d.SetPropText_info_Return_FieldPlus1)))
                .Save();

            var proxy =
                container.Invoke<Dummy>();

            Except(
                () => proxy.AccessItWillThrowException = 2,
                () => { int res = proxy.AccessItWillThrowException; });

            Assert.IsTrue(proxy.Text.Contains("was added"));
        }

        [Test]
        public void WithVoid()
        {
            var container =
                new SurrogatesContainer();

            container.Map(m => m
                .From<Dummy>()
                .Visit
                .This(d => d.AccessItWillThrowException)
                .Accessors(a =>
                  a.Getter.Using<InterferenceObject>(d => (Action<Dummy, int>) d.SetPropText_TypeName)));

            var proxy =
                container.Invoke<Dummy>();
            
            Except(
                () => proxy.AccessItWillThrowException = 2,
                () => { int res = proxy.AccessItWillThrowException; });

            Assert.IsTrue(proxy.Text.Contains("Dummy"));
        }

        [Test]
        public void WithFieldAndInstance()
        {
            var container =
                new SurrogatesContainer();

            container.Map(m => m
                .From<Dummy>()
                .Visit
                .This(d => d.AccessItWillThrowException)
                .Accessors(a => a
                    .Getter.Using<InterferenceObject>(d => (Func<int, Dummy, int>) d.SetPropText_info_Return_FieldPlus1))
                );

            var proxy =
                container.Invoke<Dummy>();

            Except(
                () => proxy.AccessItWillThrowException = 2, 
                () => { int res = proxy.AccessItWillThrowException; });

            Assert.IsTrue(proxy.Text.Contains("was added"));
        }

        public void Except(params Action[] actions)
        {
            for (int i = 0; i < actions.Length; i++)
            {

                try
                {
                    actions[i]();
                    Assert.Fail();
                }
                catch { }
            }
        }
    }
}