using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

public static class ExpressionExtentions {

    public static Expression<Func<TInput, bool>> CombineWithAndAlso<TInput>(this Expression<Func<TInput, bool>> func1, Expression<Func<TInput, bool>> func2) {
        return Expression.Lambda<Func<TInput, bool>>(
            Expression.AndAlso(
                func1.Body, new ExpressionParameterReplacer(func2.Parameters, func1.Parameters).Visit(func2.Body)),
            func1.Parameters);
    }

    public static Expression<Func<TInput, bool>> CombineWithOrElse<TInput>(this Expression<Func<TInput, bool>> func1, Expression<Func<TInput, bool>> func2) {
        return Expression.Lambda<Func<TInput, bool>>(
            Expression.AndAlso(
                func1.Body, new ExpressionParameterReplacer(func2.Parameters, func1.Parameters).Visit(func2.Body)),
            func1.Parameters);
    }

    private class ExpressionParameterReplacer : ExpressionVisitor {
        public ExpressionParameterReplacer(IList<ParameterExpression> fromParameters, IList<ParameterExpression> toParameters) {
            ParameterReplacements = new Dictionary<ParameterExpression, ParameterExpression>();
            for (int i = 0; i != fromParameters.Count && i != toParameters.Count; i++)
                ParameterReplacements.Add(fromParameters[i], toParameters[i]);
        }

        private IDictionary<ParameterExpression, ParameterExpression> ParameterReplacements {
            get;
            set;
        }

        protected override Expression VisitParameter(ParameterExpression node) {
            ParameterExpression replacement;
            if (ParameterReplacements.TryGetValue(node, out replacement))
                node = replacement;
            return base.VisitParameter(node);
        }
    }

    public static Expression<Func<T, bool>> CombineAndAlso<T>(this Expression<Func<T, Boolean>> first, Expression<Func<T, Boolean>> second) {
        var toInvoke = Expression.Invoke(second, first.Parameters);

        return (Expression<Func<T, Boolean>>)Expression.Lambda(Expression.AndAlso(first.Body, toInvoke), first.Parameters);
    }

    public static Expression<Func<T, bool>> CombineOr<T>(this Expression<Func<T, Boolean>> first, Expression<Func<T, Boolean>> second) {
        var toInvoke = Expression.Invoke(second, first.Parameters);

        return (Expression<Func<T, Boolean>>)Expression.Lambda(Expression.Or(first.Body, toInvoke), first.Parameters);
    }

    public static Expression<Func<T, bool>> CombineOrElse<T>(this Expression<Func<T, Boolean>> first, Expression<Func<T, Boolean>> second) {
        var toInvoke = Expression.Invoke(second, first.Parameters);

        return (Expression<Func<T, Boolean>>)Expression.Lambda(Expression.OrElse(first.Body, toInvoke), first.Parameters);
    }

    public static Expression<TDelegate> AndAlso<TDelegate>(this Expression<TDelegate> left, Expression<TDelegate> right) {
        return Expression.Lambda<TDelegate>(Expression.AndAlso(left, right), left.Parameters);
    }
    
    public static Expression<TDelegate> Or<TDelegate>(this Expression<TDelegate> left, Expression<TDelegate> right) {
        return Expression.Lambda<TDelegate>(Expression.Or(left, right), left.Parameters);
    }

    public static Expression<TDelegate> OrElse<TDelegate>(this Expression<TDelegate> left, Expression<TDelegate> right) {
        return Expression.Lambda<TDelegate>(Expression.OrElse(left, right), left.Parameters);
    }

    public static string ToHashMD5(this Expression expression) {
        return expression.ToString().ToHashMD5();
    }
    
    private static IOrderedQueryable<T> OrderingHelper<T>(IQueryable<T> source, string propertyName, bool descending, bool anotherLevel) {
        ParameterExpression param = Expression.Parameter(typeof(T), string.Empty);
        MemberExpression property = Expression.PropertyOrField(param, propertyName);
        LambdaExpression sort = Expression.Lambda(property, param);

        MethodCallExpression call = Expression.Call(
            typeof(Queryable),
            (!anotherLevel ? "OrderBy" : "ThenBy") + (descending ? "Descending" : string.Empty),
            new[] { typeof(T), property.Type },
            source.Expression,
            Expression.Quote(sort));

        IOrderedQueryable<T> expression = (IOrderedQueryable<T>)source.Provider.CreateQuery<T>(call);

        return expression;
    }

    public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName) {
        return OrderingHelper(source, propertyName, false, false);
    }

    public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string propertyName) {
        return OrderingHelper(source, propertyName, true, false);
    }

    public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string propertyName) {
        return OrderingHelper(source, propertyName, false, true);
    }

    public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string propertyName) {
        return OrderingHelper(source, propertyName, true, true);
    }

}
