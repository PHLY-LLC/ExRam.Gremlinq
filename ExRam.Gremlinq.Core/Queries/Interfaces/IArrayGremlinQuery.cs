using System;
using System.Linq.Expressions;

namespace ExRam.Gremlinq.Core
{
    public interface IFluentOrdered<out TUnorderedQuery, out TOrderedQuery>
        where TUnorderedQuery : IGremlinQuery
        where TOrderedQuery : IGremlinQuery
    {
        TOrderedQuery ThenBy(Func<TUnorderedQuery, IGremlinQuery> traversal);
        TOrderedQuery ThenByDescending(Func<TUnorderedQuery, IGremlinQuery> traversal);
        TOrderedQuery ThenBy(string lambda);
    }

    public interface IFluentTypedOrdered<TElement, out TOrderedQuery>
        where TOrderedQuery : IGremlinQuery
    {
        TOrderedQuery ThenBy(Expression<Func<TElement, object>> projection);
        TOrderedQuery ThenByDescending(Expression<Func<TElement, object>> projection);
    }

    public partial interface IArrayGremlinQuery<TArray, TQuery> : IGremlinQuery<TArray>
    {
        TQuery Unfold();
    }
}
