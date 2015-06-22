﻿using Surrogates.Applications.Cache.Model;
using System;
using System.Collections.Generic;

namespace Surrogates.Applications.Cache
{
    public class SimpleCacheInterceptor
    {
        private static CachedList _cache;

        static SimpleCacheInterceptor()
        {
            _cache = new CachedList();
        }

        public object CacheMethod(Delegate s_method, object[] args, Dictionary<IntPtr, CacheParams> s_Params)
        {
            object result = null;

            var thisPtr = s_method
                .Method
                .MethodHandle
                .Value;

            var @params = 
                s_Params[thisPtr];

            var key = @params
                .GetKey
                .DynamicInvoke(new object[] { args });

            if(!_cache.TryGet(key, ref result))
            {
                result = s_method
                    .DynamicInvoke(args);

                _cache.Add(key, result, @params.Timeout);
            }

            return result;
        }
    }
}
