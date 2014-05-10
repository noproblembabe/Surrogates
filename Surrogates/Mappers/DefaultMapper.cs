﻿using Surrogates.Expressions.Classes;
using Surrogates.Expressions.Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Surrogates.Mappers
{
    public class DefaultMapper : BaseMapper
    {
        protected IFlushTypes MapExpression; 

        internal DefaultMapper(MappingState state)
        {
            this.State = state;
        }

        public override IMappingExpression<T> Throughout<T>(string name = null)
        {
            return (IMappingExpression<T>)
                (MapExpression = new ClassMappingExpression<T>(name, State));
        }

        internal static string CreateName4<T>()
        {
            return string.Concat(typeof(T).Name, "Proxy");
        }

        public override Type Flush()
        {
            return MapExpression.Flush();
        }
    }
}