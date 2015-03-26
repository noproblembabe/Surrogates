﻿using Surrogates.Tactics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Surrogates.Utilities.Mixins;

namespace Surrogates.Executioners
{
    public class DisableExecutioner : Executioner
    {
        protected static void DisableMethod(TypeBuilder typeBuilder, MethodInfo method)
        {
            var builder = typeBuilder.DefineMethod(
                    method.Name,
                    MethodAttributes.Public | MethodAttributes.Virtual,
                    method.ReturnType,
                    method.GetParameters().Select(p => p.ParameterType).ToArray());

            var gen = builder.GetILGenerator();

            if (method.ReturnType != typeof(void))
            {
                gen.EmitDefaultValue(method.ReturnType);
            }
            gen.Emit(OpCodes.Ret);
        }
                
        protected static void DisableSetter(Strategy.ForProperties strategy, Model.Property property)
        {
            var setter =
                CreateSetter(strategy, property.Original);

            var gen = setter.GetILGenerator();
            gen.Emit(OpCodes.Ret);

            property.Builder.SetSetMethod(setter);
        }
        
        protected static void DisableGetter(Strategy.ForProperties strategy, Model.Property property)
        {
            var getter =
                CreateGetter(strategy, property.Original);

            var gen = getter.GetILGenerator();

            gen.EmitDefaultValue(property.Original.PropertyType);
            gen.Emit(OpCodes.Ret);
        }

        public override void Execute4Properties(Strategy.ForProperties strategy)
        {
            foreach (var property in strategy.Properties)
            {
                DisableSetter(strategy, property);

                DisableGetter(strategy, property);
            }
        }

        public override void Execute4Methods(Strategy.ForMethods strategy)
        {
            foreach (var method in strategy.Methods)
            {
                DisableMethod(strategy.TypeBuilder, method);
            }
        }
    }
}