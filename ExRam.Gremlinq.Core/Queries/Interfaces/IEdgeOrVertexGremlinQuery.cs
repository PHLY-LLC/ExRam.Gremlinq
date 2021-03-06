﻿namespace ExRam.Gremlinq.Core
{
    public partial interface IEdgeOrVertexGremlinQueryBase :
        IElementGremlinQueryBase
    {
        IValueGremlinQuery<TTarget> Values<TTarget>();
    }

    public interface IEdgeOrVertexGremlinQueryBaseRec<TSelf> :
        IEdgeOrVertexGremlinQueryBase,
        IElementGremlinQueryBaseRec<TSelf>
        where TSelf : IElementGremlinQueryBaseRec<TSelf>
    {
    }

    public partial interface IEdgeOrVertexGremlinQueryBase<TElement> :
        IEdgeOrVertexGremlinQueryBase,
        IElementGremlinQueryBase<TElement>
    {
    }

    public interface IEdgeOrVertexGremlinQueryBaseRec<TElement, TSelf> :
        IEdgeOrVertexGremlinQueryBaseRec<TSelf>,
        IEdgeOrVertexGremlinQueryBase<TElement>,
        IElementGremlinQueryBaseRec<TElement, TSelf>
        where TSelf : IElementGremlinQueryBaseRec<TElement, TSelf>
    {
    }

    public interface IEdgeOrVertexGremlinQuery<TElement> :
        IEdgeOrVertexGremlinQueryBaseRec<TElement, IEdgeOrVertexGremlinQuery<TElement>>
    {
    }
}
