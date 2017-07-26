﻿using System;
using System.Linq;
using System.Reflection;
using Refit.Insane.PowerPack.Configuration;

namespace Refit.Insane.PowerPack.Attributes
{
    public static class ApiDefinitionAttributeExtension
    {	
        public static Uri GetUri<TApi>()
        {
            var attribute = GetAttribute<TApi>();
            return attribute != null ? new Uri(attribute.BaseUri) : BaseApiConfiguration.ApiUri;
        }
        
        public static TimeSpan GetTimeout<TApi>()
        {
            var attribute = GetAttribute<TApi>();
            return attribute?.ApiTimeout ?? TimeSpan.FromSeconds(5);
        }
		
        public static Type GetHttpClientHandlerType<TApi>()
        {
            var attribute = GetAttribute<TApi>();
            return attribute != null ? attribute.HttpClientHandlerType : typeof(HttpClientDiagnosticsHandler);
        }

        private static ApiDefinitionAttribute GetAttribute<TApi>()
        {
            var attrs = typeof(TApi).GetTypeInfo().GetCustomAttributes(typeof(ApiDefinitionAttribute));
            return attrs.FirstOrDefault(attr => attr is ApiDefinitionAttribute) as ApiDefinitionAttribute;
        }
    }
}