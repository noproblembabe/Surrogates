﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Surrogates
{
    internal static class TypeBuilderMixins
    {
        internal static void CreateConstructor4<T>(this TypeBuilder typeBuilder, IList<FieldInfo> fields)
        {
            var ctorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                new Type[] { });

            var ctrGen = ctorBuilder.GetILGenerator();

            ctrGen.EmitConstructor4<T>(fields);
        }

        internal static ILGenerator EmitOverride(this TypeBuilder typeBuilder, MethodInfo newMethod, MethodInfo baseMethod, FieldInfo interceptorField)
        {
            LocalBuilder @return = null;
            return EmitOverride(typeBuilder, newMethod, baseMethod, interceptorField, out @return);
        }

        internal static ILGenerator EmitOverride(this TypeBuilder typeBuilder, MethodInfo newMethod, MethodInfo baseMethod, FieldInfo interceptorField, out LocalBuilder returnField)
        {
            var builder = typeBuilder.DefineMethod(
                baseMethod.Name,
                MethodAttributes.Public | MethodAttributes.Virtual,
                baseMethod.ReturnType,
                baseMethod.GetParameters().Select(p => p.ParameterType).ToArray());

            var gen = builder.GetILGenerator();

            returnField = baseMethod.ReturnType != typeof(void) ?
                gen.DeclareLocal(baseMethod.ReturnType) : 
                null;

            //gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld, interceptorField);

            var @params =
                gen.EmitParameters(newMethod, baseMethod);

            gen.EmitCall(OpCodes.Callvirt, newMethod, @params);

            return gen;
        }
    }
}