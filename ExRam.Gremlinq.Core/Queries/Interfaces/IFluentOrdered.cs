using System;

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
}