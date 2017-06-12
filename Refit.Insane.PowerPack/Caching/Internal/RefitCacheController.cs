using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Refit.Insane.PowerPack.Data;

namespace Refit.Insane.PowerPack.Caching.Internal
{
    public class RefitCacheController
    {
        private readonly Dictionary<MethodCacheDetails, MethodCacheAttributes> _cacheableMethodsSet = new Dictionary<MethodCacheDetails, MethodCacheAttributes>();

        public RefitCacheController()
        {

        }

        public bool IsMethodCacheable<TApi, TResult>(Expression<Func<TApi, Task<TResult>>> restExpression)
        {
            var methodToCacheDetailsResponse = GetMethodToCacheData(restExpression);

            if (!methodToCacheDetailsResponse.IsSuccess)
                return false;

            lock (this)
            {
                var methodToCacheData = methodToCacheDetailsResponse.Results;
                if (_cacheableMethodsSet.ContainsKey(methodToCacheData))
                    return true;

                var refitCacheAttribute =
                    methodToCacheData
                        .MethodInfo
                        .GetCustomAttribute<RefitCacheAttribute>();

                if (refitCacheAttribute == null)
                    return false;

                var methodParameters = methodToCacheData.MethodInfo.GetParameters()
                                                        .Where(x => !typeof(CancellationToken).GetTypeInfo().IsAssignableFrom(x.ParameterType.GetTypeInfo()))
                                                        .ToList();
                var cachePrimaryKey =
                                methodParameters
                                 .Select((x, index) => new
                                 {
                                     Index = index,
                                     ParameterInfo = x
                                 })
                                     .Where(x => x.ParameterInfo.CustomAttributes.Any(y => y.AttributeType == typeof(RefitCachePrimaryKeyAttribute)))
                                .Select(x => new
                                {
                                    ParameterName = x.ParameterInfo.Name,
                                    ParameterType = x.ParameterInfo.ParameterType,
                                    CacheAttribute = x.ParameterInfo.GetCustomAttribute<RefitCachePrimaryKeyAttribute>(),
                                    ParameterOrder = x.Index
                                }).FirstOrDefault();

                if (cachePrimaryKey == null && methodParameters.Any())
                    throw new InvalidOperationException($"{methodToCacheData.MethodInfo.Name} method has {nameof(RefitCacheAttribute)}, " +
                                                        $"it has method parameters but none of that contain {nameof(RefitCachePrimaryKeyAttribute)}");


                _cacheableMethodsSet.Add(
                    methodToCacheData,
                    new MethodCacheAttributes(refitCacheAttribute, cachePrimaryKey?.CacheAttribute, cachePrimaryKey?.ParameterName, cachePrimaryKey?.ParameterType,
                                              cachePrimaryKey?.ParameterOrder ?? 0)
                );
            }

            return true;
        }

        private Response<MethodCacheDetails> GetMethodToCacheData<TApi, TResult>(Expression<Func<TApi, Task<TResult>>> restExpression)
        {
            var apiInterfaceType = typeof(TApi);

            var methodBody = restExpression.Body as MethodCallExpression;

            if (methodBody == null)
                return new Response<MethodCacheDetails>().SetAsFailureResponse();

            var methodInfo = methodBody.Method;
            var methodToCacheDetails = new MethodCacheDetails(apiInterfaceType, methodInfo);
            return new Response<MethodCacheDetails>(methodToCacheDetails);
        }

        private static IEnumerable<ExtractedConstant> ExtractConstants(Expression expression)
        {
            if (expression == null)
                yield break;

            if (expression is ConstantExpression)
            {
                var constantsExpression = expression as ConstantExpression;
                yield return
                    new ExtractedConstant() { Name = constantsExpression.Type.Name, Value = constantsExpression.Value };
            }


            else if (expression is LambdaExpression)
                foreach (var constant in ExtractConstants(
                    ((LambdaExpression)expression).Body))
                    yield return constant;

            else if (expression is UnaryExpression)
                foreach (var constant in ExtractConstants(
                    ((UnaryExpression)expression).Operand))
                    yield return constant;

            else if (expression is MethodCallExpression)
            {
                foreach (var arg in ((MethodCallExpression)expression).Arguments)
                    foreach (var constant in ExtractConstants(arg))
                        yield return constant;
                foreach (var constant in ExtractConstants(
                    ((MethodCallExpression)expression).Object))
                    yield return constant;
            }
            else if (expression is MemberExpression)
            {
                var memberExpression = expression as MemberExpression;

                foreach (var constants in ExtractConstants(memberExpression.Expression))
                    yield return constants;
            }
            else if (expression is InvocationExpression)
            {
                var invocationExpression = expression as InvocationExpression;

                foreach (var constants in ExtractConstants(invocationExpression.Expression))
                    yield return constants;
            }

            else
                throw new NotImplementedException();
        }

        public string GetCacheKey<TApi, TResult>(Expression<Func<TApi, Task<TResult>>> fromExpression)
        {
            var methodCallExpression = fromExpression.Body as MethodCallExpression;

            string cacheKeyPrefix = typeof(TApi).ToString() + "/" + methodCallExpression.Method.Name.ToString();
            if (!methodCallExpression.Arguments.Any())
                return cacheKeyPrefix;

            var cacheAttributes = GetRefitCacheAttribute(fromExpression);

            var extractedArguments = methodCallExpression.Arguments
                .SelectMany(x => ExtractConstants(x))
                .Where(x => x != null)
                .Where(x => x.Value is CancellationToken == false)
                .ToList();

            if (!extractedArguments.Any())
                return cacheKeyPrefix;

            object primaryKeyValue = null;
            var extractedArgument = extractedArguments[cacheAttributes.ParameterOrder];
            var extractedArgumentValue = extractedArgument.Value;


            bool isArgumentValuePrimitve = extractedArgumentValue.GetType().GetTypeInfo().IsPrimitive ||
                                            extractedArgumentValue is decimal ||
                                             extractedArgumentValue is string;

            if (isArgumentValuePrimitve)
                primaryKeyValue = extractedArgument.Value;
            else
            {
                var primaryKeyValueField = extractedArgumentValue.GetType().GetRuntimeFields().Select((x, i) => new
                {
                    Index = i,
                    Field = x
                }).First(x => x.Index == cacheAttributes.ParameterOrder);

                primaryKeyValue = primaryKeyValueField.Field.GetValue(extractedArgumentValue);
            }

            foreach (var argument in extractedArguments)
            {
                var primaryKeyCacheField = argument
                    .Value
                    .GetType()
                    .GetRuntimeFields()
                    .FirstOrDefault(x => x.Name.Equals(cacheAttributes.ParameterName));

                if (primaryKeyCacheField != null)
                {
                    primaryKeyValue = primaryKeyCacheField.GetValue(argument.Value);
                    break;
                }
            }

            if (primaryKeyValue == null)
                throw new InvalidOperationException($"{nameof(RefitCachePrimaryKeyAttribute)} primary key found for: " + cacheKeyPrefix);

            return $"{cacheKeyPrefix}/{primaryKeyValue.ToString()}";
        }

        public MethodCacheAttributes GetRefitCacheAttribute<TApi, TResult>(Expression<Func<TApi, Task<TResult>>> expression)
        {
            lock (this)
            {
                var methodToCacheData = GetMethodToCacheData(expression).Results;
                return _cacheableMethodsSet[methodToCacheData];
            }
        }

        class ExtractedConstant
        {
            public object Value { get; set; }

            public string Name { get; set; }
        }
    }
}
