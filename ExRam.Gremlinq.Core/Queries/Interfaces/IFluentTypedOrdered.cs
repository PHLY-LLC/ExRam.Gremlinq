using System;
using System.Linq.Expressions;

namespace ExRam.Gremlinq.Core
{
    public interface IFluentTypedOrdered<TElement, out TOrderedQuery>
        where TOrderedQuery : IGremlinQuery
    {
        TOrderedQuery ThenBy(Expression<Func<TElement, object>> projection);
        TOrderedQuery ThenByDescending(Expression<Func<TElement, object>> projection);
    }
}