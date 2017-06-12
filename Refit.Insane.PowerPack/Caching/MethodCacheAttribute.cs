using System;
using Refit.Insane.PowerPack.Caching.Internal;

namespace Refit.Insane.PowerPack.Caching
{
    public class MethodCacheAttributes
    {
        public MethodCacheAttributes(RefitCacheAttribute cacheAttribute, RefitCachePrimaryKeyAttribute primaryKeyAttribute, string paramName,
                                    Type paramType, int paramOrder)
        {
            CacheAttribute = cacheAttribute;
            CachePrimaryKeyAttribute = primaryKeyAttribute;
            ParameterName = paramName;
            ParameterType = paramType;
            ParameterOrder = paramOrder;
        }

        public int ParameterOrder { get; }

        public RefitCacheAttribute CacheAttribute { get; }

        public RefitCachePrimaryKeyAttribute CachePrimaryKeyAttribute { get; }  

        public string ParameterName { get; }

        public Type ParameterType { get; }
    }

}
