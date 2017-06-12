using System;
using System.Reflection;

namespace Refit.Insane.PowerPack.Caching.Internal
{
    class MethodCacheDetails
	{
		public MethodCacheDetails(Type apiInterfaceType, MethodInfo methodInfo)
		{
			ApiInterfaceType = apiInterfaceType;
			MethodInfo = methodInfo;
		}

		public Type ApiInterfaceType { get; }

		public MethodInfo MethodInfo { get; }

		public RefitCacheAttribute CacheAttribute { get; internal set; }

		public override int GetHashCode() => ApiInterfaceType.GetHashCode() * 23 * MethodInfo.GetHashCode() * 23 * 29;

		public override bool Equals(object obj)
		{
			var other = obj as MethodCacheDetails;
			return other != null &&
				   other.ApiInterfaceType.Equals(ApiInterfaceType) &&
				   other.MethodInfo.Equals(MethodInfo);
		}
	}
}
