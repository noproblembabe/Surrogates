﻿using Surrogates;
using NUnit.Framework;
using Surrogates.Tests.Simple.Entities;
using System;

namespace Surrogates.Tests.Simple.Methods.Visit
{
    class Visit_ActionWtihFunctionTest : IInterferenceTest
    {
        [Test]
        public void BothParameterLess()
        {
            var container = new SurrogatesContainer();

            container.Map(m => m
                .Throughout<Dummy>()
                .Visit
                .ThisMethod(d => d.Void_ParameterLess)
                .Using<InterferenceObject>()
                .ThisMethod<int>(r => r.Int_2_ParameterLess));

            var dummy =
                new Dummy();

            var proxy =
                container.Invoke<Dummy>();

            dummy.Void_ParameterLess();
            proxy.Void_ParameterLess();

            Assert.AreEqual("simple", dummy.Text);
            Assert.AreEqual("simple", proxy.Text);
        }

        [Test]
        public void PassingBaseParameters()
        {
            var container = new SurrogatesContainer();

            container.Map(m =>
                m.Throughout<Dummy>()
                .Visit.ThisMethod<string, DateTime, Dummy.EvenMore>(d => d.Void_VariousParameters)
                .Using<InterferenceObject>()
                .ThisMethod<string, Dummy, DateTime, string, Dummy.EvenMore, int>(r => r.Int_2_VariousParametersPlusIntanceAndMethodName));

            var dummy =
                new Dummy();

            var proxy =
                container.Invoke<Dummy>();

            //and now, the comparison between the two methods
            dummy.Void_VariousParameters("this call was not made by the original method", DateTime.Now, new Dummy.EvenMore());
            proxy.Void_VariousParameters("this call was not made by the original method", DateTime.Now, new Dummy.EvenMore());

            Assert.IsNotNullOrEmpty(dummy.Text);
            Assert.AreEqual("complex", dummy.Text);
            Assert.IsNotNullOrEmpty(proxy.Text);
            Assert.AreEqual("complex", proxy.Text);
        }

        [Test, ExpectedException(typeof(NullReferenceException))]
        public void NotPassingBaseParameters()
        {
            var container = new SurrogatesContainer();

            container.Map(m =>
                m.Throughout<Dummy>()
                .Visit.ThisMethod<string, DateTime, Dummy.EvenMore>(d => d.Void_VariousParameters)
                .Using<InterferenceObject>()
                .ThisMethod<string, Dummy, DateTime, string, Dummy.EvenMore, int>(r => r.Int_2_VariousParametersWithDifferentNames));

            var dummy =
                new Dummy();

            var proxy =
                container.Invoke<Dummy>();

            dummy.Void_VariousParameters("text", DateTime.Now, new Dummy.EvenMore());
            proxy.Void_VariousParameters("text", DateTime.Now, new Dummy.EvenMore());
        }

        [Test]
        public void PassingInstanceAndMethodName()
        {
            var container = new SurrogatesContainer();

            container.Map(m => m
                .Throughout<Dummy>()
                .Visit
                .ThisMethod(d => d.Void_ParameterLess)
                .Using<InterferenceObject>()
                .ThisMethod<Dummy, string, int>(r => r.Int_2_InstanceAndMethodName))
                ;

            var dummy =
                new Dummy();

            var proxy =
                container.Invoke<Dummy>();

            dummy.Void_ParameterLess();
            proxy.Void_ParameterLess();


            Assert.AreEqual("simple", dummy.Text);
            Assert.AreEqual("simple", proxy.Text);
        }
    }
}