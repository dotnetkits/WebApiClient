﻿using System;
using System.Linq;
using WebApiClientCore.Exceptions;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示THttpApi的代理类的实例创建者
    /// </summary>
    /// <typeparam name="THttpApi">接口类型</typeparam>
    /// <exception cref="NotSupportedException"></exception>
    /// <exception cref="ProxyTypeCreateException"></exception>
    static class HttpApiProxyBuilder<THttpApi>
    {
        /// <summary>
        /// 接口包含的所有action执行器 
        /// </summary>
        private static readonly IActionInvoker[] actionInvokers;

        /// <summary>
        /// 接口代理类的构造器
        /// </summary>
        private static readonly Func<IActionInterceptor, IActionInvoker[], THttpApi> proxyTypeCtor;

        /// <summary>
        /// IHttpApi的代理类的实例创建者
        /// </summary> 
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ProxyTypeCreateException"></exception>
        static HttpApiProxyBuilder()
        {
            var interfaceType = typeof(THttpApi);

            actionInvokers = interfaceType
                .GetAllApiMethods()
                .Select(item => new ApiActionDescriptor(item, interfaceType))
                .Select(item => CreateActionInvoker(item))
                .ToArray();

            var proxyType = HttpApiProxyTypeBuilder.Build(interfaceType, actionInvokers);
            proxyTypeCtor = Lambda.CreateCtorFunc<IActionInterceptor, IActionInvoker[], THttpApi>(proxyType);
        }

        /// <summary>
        /// 创建Action执行器 
        /// </summary>
        /// <param name="apiAction">action描述器</param>
        /// <returns></returns>
        private static IActionInvoker CreateActionInvoker(ApiActionDescriptor apiAction)
        {
            var resultType = apiAction.Return.DataType.Type;
            var invokerType = typeof(MultiplexedActionInvoker<>).MakeGenericType(resultType);
            return invokerType.CreateInstance<IActionInvoker>(apiAction);
        }

        /// <summary>
        /// 创建IHttpApi的代理类的实例
        /// </summary>
        /// <param name="actionInterceptor">拦截器</param>
        /// <returns></returns>
        public static THttpApi Build(IActionInterceptor actionInterceptor)
        {
            return proxyTypeCtor.Invoke(actionInterceptor, actionInvokers);
        }
    }
}
