using System;
using System.Linq;
using LanguageExt;

namespace ExRam.Gremlinq
{
    public static class GraphElementNamingStrategy
    {
        private sealed class SimpleGraphElementNamingStrategy : IGraphElementNamingStrategy
        {
            public Option<string> TryGetLabelOfType(IGraphModel model, Type type)
            {
                return type.Name;
            }

            public Option<Type> TryGetVertexTypeOfLabel(IGraphModel model, string label)
            {
                return model.VertexTypes
                    .Concat(model.EdgeTypes)
                    .FirstOrDefault(type => type.Name.Equals(label, StringComparison.OrdinalIgnoreCase));
            }

            public Option<Type> TryGetEdgeTypeOfLabel(IGraphModel model, string label)
            {
                return model.EdgeTypes
                    .FirstOrDefault(type => type.Name.Equals(label, StringComparison.OrdinalIgnoreCase));
            }
        }

        public static readonly IGraphElementNamingStrategy Simple = new SimpleGraphElementNamingStrategy();
    }
}