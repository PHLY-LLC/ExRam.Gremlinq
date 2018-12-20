using System.Collections.Generic;
using ExRam.Gremlinq.Core;
using LanguageExt;

namespace System.Linq
{
    public static class EnumerableExtensions
    {
        private sealed class PropertyStepComparer : IComparer<PropertyStep>
        {
            public static readonly PropertyStepComparer Instance = new PropertyStepComparer();

            public int Compare(PropertyStep x, PropertyStep y)
            {
                return -(x?.Key is T).CompareTo(y?.Key is T);
            }
        }

        internal static IEnumerable<Step> HandleAnonymousQueries(this IEnumerable<Step> steps)
        {
            using (var e = steps.GetEnumerator())
            {
                var hasNext = e.MoveNext();

                if (!hasNext || !(e.Current is IdentifierStep))
                    yield return IdentifierStep.__;

                if (!hasNext)
                    yield return IdentityStep.Instance;
                else
                    yield return e.Current;

                while (e.MoveNext())
                    yield return e.Current;
            }
        }

        internal static IEnumerable<Either<Step, TStep[]>> Batch<TStep>(this IEnumerable<Step> steps) where TStep : Step
        {
            var propertySteps = default(List<TStep>);

            using (var e = steps.GetEnumerator())
            {
                while (true)
                {
                    var hasNext = e.MoveNext();

                    if (hasNext && e.Current is TStep firstStep)
                    {
                        if (propertySteps == null || propertySteps.Count == 0)
                        {
                            if (hasNext = e.MoveNext())
                            {
                                if (e.Current is TStep secondStep)
                                {
                                    if (propertySteps == null)
                                        propertySteps = new List<TStep>();

                                    propertySteps.Add(firstStep);
                                    propertySteps.Add(secondStep);
                                }
                                else
                                {
                                    yield return firstStep;
                                    yield return e.Current;
                                }
                            }
                            else
                                yield return firstStep;
                        }
                        else
                            propertySteps.Add(firstStep);
                    }
                    else
                    {
                        if (propertySteps != null && propertySteps.Count > 0)
                        {
                            yield return propertySteps.ToArray();

                            propertySteps.Clear();
                        }

                        if (hasNext)
                            yield return e.Current;
                    }

                    if (!hasNext)
                        break;
                }
            }
        }

        //https://issues.apache.org/jira/browse/TINKERPOP-2112.
        internal static IEnumerable<Step> WorkaroundTINKERPOP_2112(this IEnumerable<Step> steps)
        {
            foreach (var either in steps.Batch<PropertyStep>())
            {
                if (either.IsLeft)
                    yield return (Step)either;
                else
                {
                    var propertySteps = (PropertyStep[])either;
                    Array.Sort(propertySteps, PropertyStepComparer.Instance);

                    foreach (var replayPropertyStep in propertySteps)
                    {
                        yield return replayPropertyStep;
                    }
                }
            }
        }
    }
}
