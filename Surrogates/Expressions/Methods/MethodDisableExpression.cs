﻿using Surrogates.Expressions.Classes;
using Surrogates.Mappers;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogates.Expressions.Methods
{
    public class MethodDisableExpression<TBase>
        : FluentExpression<MethodDisableExpression<TBase>, TBase, TBase>
    {
        internal MethodDisableExpression(IMappingExpression<TBase> mapper, MappingState state)
            : base(mapper, state) { }

        public virtual AndExpression<TBase> AllPublicMethods()
        {
            var @public = typeof(TBase)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public);

            for (int i = 0; i < @public.Length; i++)
            {
                RegisterMethod(@public[i]);
            }

            return new AndExpression<TBase>(Mapper);
        }

        public virtual AndExpression<TBase> AllProtected()
        {
            var methods = typeof(TBase)
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);

            for (int i = 0; i < methods.Length; i++)
            {
                if (methods[i].IsFamily && !methods[i].IsPrivate)
                {
                    RegisterMethod(methods[i]);
                }
            }

            return new AndExpression<TBase>(Mapper);
        }

        public virtual AndExpression<TBase> AllInternal()
        {
            var methods = typeof(TBase)
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);

            for (int i = 0; i < methods.Length; i++)
            {
                if (!methods[i].IsFamily && !methods[i].IsPrivate)
                {
                    RegisterMethod(methods[i]);
                }
            }

            return new AndExpression<TBase>(Mapper);
        }

        public virtual AndExpression<TBase> Where(Func<MethodInfo, bool> predicate)
        {
            var methods = typeof(TBase)
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            for (int i = 0; i < methods.Length; i++)
            {
                if ((methods[i].IsFamily || methods[i].IsPublic) &&
                    (predicate(methods[i])))
                {
                    RegisterMethod(methods[i]);
                }
            }

            return new AndExpression<TBase>(Mapper);
        }

        protected override void RegisterMethod(MethodInfo method)
        {
            State.Push(method);

            var builder = State.TypeBuilder.DefineMethod(
                method.Name,
                MethodAttributes.Public | MethodAttributes.Virtual,
                method.ReturnType,
                method.GetParameters().Select(p => p.ParameterType).ToArray());

            var gen = builder.GetILGenerator();

            if (method.ReflectedType != typeof(void))
            {
                gen.Emit(OpCodes.Nop);
                gen.EmitDefaultValue(method.ReturnType);
            }
        }
    }
}