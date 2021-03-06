﻿
using NUnit.Framework;
using Surrogates.Model.Entities;
using Surrogates.Tactics;
using Surrogates.Tests.Expressions.Entities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogates.Tests.Strategies
{
    public class StrategiesTests<T> : StrategiesTests
        where T : Strategy
    {
        protected T Strategy;
        protected Surrogates.Tactics.Strategies Strategies;
        
        [SetUp]
        public void SetUp()
        {
            Strategies =
                CreateStrategies4<Dummy>();

            Strategy = (T)Activator.CreateInstance(typeof(T), new object[] { Strategies });

            Strategies.Add(Strategy);
        }   
    }

    public class StrategiesTests
    {
        protected Surrogates.Tactics.Strategies CreateStrategies4<T>(string name = null)
        {
            if (string.IsNullOrEmpty(name))
            { name = typeof(T).Name + "Proxy"; }

            var modField = typeof(SurrogatesContainer)
                .GetField("ModuleBuilder", BindingFlags.Instance | BindingFlags.NonPublic);

            var mod = (ModuleBuilder)modField
                .GetValue(new SurrogatesContainer());

            return new Tactics.Strategies(
                typeof(T), name, mod, Access.Container | Access.StateBag | Access.AnyMethod | Access.AnyField | Access.AnyBaseProperty | Access.AnyNewProperty | Access.Instance);
        }

        protected static T FirstStrategy<T>(Surrogates.Tactics.Strategies strats)
            where T : Strategy
        {
            return (T)GetStrategies(strats)[0];
        }

        protected static IList<Strategy> GetStrategies(Surrogates.Tactics.Strategies strats)
        {
            var stratsField = typeof(Surrogates.Tactics.Strategies)
                .GetField("_strategies", BindingFlags.Instance | BindingFlags.NonPublic);

            var strategies = (IList<Strategy>)
                stratsField.GetValue(strats);
            return strategies;
        }
    }
}
