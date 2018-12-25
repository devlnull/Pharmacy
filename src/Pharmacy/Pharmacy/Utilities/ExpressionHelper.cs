using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Pharmacy.Utilities
{
    public class ExpressionHelper
    {
        private static readonly MethodInfo LambdaMethod =
            typeof(Expression).GetMethods()
            .First(x => x.Name == "Lambda" && x.ContainsGenericParameters
            && x.GetParameters().Length > 0);


        private static MethodInfo[] QueryableMethods =
            typeof(Queryable).GetMethods()
            .ToArray();
        private static MethodInfo GetLambdaFuncBuilder(Type source, Type dest)
        {
            var predicatedType = typeof(Func<,>).MakeGenericType(source, dest);
            return LambdaMethod.MakeGenericMethod(predicatedType);
        }

        public static PropertyInfo GetPropertyInfo<T>(string name)
         => typeof(T).GetProperties()
            .Single(x => x.Name == name);

        public static ParameterExpression Parameter<T>()
            => Expression.Parameter(typeof(T));

        public static MemberExpression GetPropertyExpression(ParameterExpression obj,
            PropertyInfo property)
            => Expression.Property(obj, property);

        public static LambdaExpression GetLambda<TSource, TDest>(ParameterExpression obj, Expression arg)
            => GetLambda(typeof(TSource), typeof(TDest), obj, arg);

        public static LambdaExpression GetLambda(Type source, Type dest, ParameterExpression obj, Expression arg)
        {
            var lambdaBuilder = GetLambdaFuncBuilder(source, dest);
            return (LambdaExpression)lambdaBuilder.Invoke(null, new object[] { arg, new[] { obj } });
        }

        public static IQueryable<TEntity> CallOrderByOrThenBy<TEntity>
            (IQueryable<TEntity> modifiedQuery, Type propertyType,
            bool descending, LambdaExpression keySelector)
        {
            modifiedQuery = modifiedQuery.Provider.CreateQuery<TEntity>
                (Expression.Call((descending ? orderByDescending : orderBy)
                .MakeGenericMethod(typeof(TEntity), propertyType),
                        modifiedQuery.Expression,
                        keySelector));

            return modifiedQuery;
        }

        public static IQueryable<TEntity> CallWhere<TEntity>(IQueryable<TEntity> modifiedQuery,
            LambdaExpression lambdaExpression)
        {
            modifiedQuery = modifiedQuery.Provider.CreateQuery<TEntity>
                    (Expression.Call((Where).MakeGenericMethod(typeof(TEntity)),
                        modifiedQuery.Expression,
                        lambdaExpression));

            return modifiedQuery;
        }


        static MethodInfo orderBy = typeof(Queryable)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .Where(x => x.Name == "OrderBy" && x.GetParameters().Length == 2)
            .First();
        static MethodInfo orderByDescending = typeof(Queryable)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .Where(x => x.Name == "OrderByDescending" && x.GetParameters().Length == 2)
            .First();
        static MethodInfo Where = typeof(Queryable)
                    .GetMethods(BindingFlags.Static | BindingFlags.Public)
                    .Where(x => x.Name == "Where" && x.GetParameters().Length == 2)
                    .First();
    }

    public static class ExpressionHelpersExtensionMethods
    {
        public static List<T> GetAllPublicConstantValues<T>(this Type type)
        {
            return type
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(T))
                .Select(x => (T)x.GetRawConstantValue())
                .ToList();
        }
    }
}
