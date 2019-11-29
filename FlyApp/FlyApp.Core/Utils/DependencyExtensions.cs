using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FlyApp.Core.Exceptions;
using Unity;

namespace FlyApp.Core.Utils
{
    public static class DependencyExtensions
    {
        public static void CheckForCycles(this IUnityContainer container)
        {
            var containerRegistrations = container.Registrations.Where(registration =>
                    registration.RegisteredType?.Namespace?.StartsWith("QQPad") == true)
                .ToList();

            var typeMap = new Dictionary<Type, Type>();
            foreach (var containerRegistration in containerRegistrations)
                typeMap[containerRegistration.RegisteredType] = containerRegistration.MappedToType;

            var types = containerRegistrations.Select(registration => registration.MappedToType).ToList();
            var cycles = types.FindCycles(type => GetRelatedTypes(type, typeMap));

            if (cycles.Any()) throw new DependencyCycleException("Found dependency cycles.", cycles);
        }

        private static IEnumerable<Type> GetRelatedTypes(Type type, Dictionary<Type, Type> typeMap)
        {
            var parameterInfos = type.GetConstructors().FirstOrDefault()?.GetParameters();
            var parameterTypes = parameterInfos?.Select(parameter => parameter.ParameterType).ToList() ??
                                 new List<Type>();
            var mappedTypes = parameterTypes.Select(interfaceType =>
                typeMap.ContainsKey(interfaceType) ? typeMap[interfaceType] : interfaceType);
            return mappedTypes;
        }

        private static TValue ValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
            TValue defaultValue)
        {
            return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
        }

        private static void TryPush<T>(T node, Func<T, IEnumerable<T>> lookup,
            Stack<KeyValuePair<T, IEnumerator<T>>> stack, Dictionary<T, VisitState> visited, List<List<T>> cycles)
        {
            var state = visited.ValueOrDefault(node, VisitState.NotVisited);
            if (state == VisitState.Visited)
            {
            }
            else if (state == VisitState.Visiting)
            {
                Debug.Assert(stack.Count > 0);
                var list = stack.Select(pair => pair.Key)
                    .TakeWhile(parent => !EqualityComparer<T>.Default.Equals(parent, node)).ToList();
                list.Add(node);
                list.Reverse();
                list.Add(node);
                cycles.Add(list);
            }
            else
            {
                visited[node] = VisitState.Visiting;
                stack.Push(new KeyValuePair<T, IEnumerator<T>>(node, lookup(node).GetEnumerator()));
            }
        }

        private static List<List<T>> FindCycles<T>(T root, Func<T, IEnumerable<T>> lookup,
            Dictionary<T, VisitState> visited)
        {
            var stack = new Stack<KeyValuePair<T, IEnumerator<T>>>();
            var cycles = new List<List<T>>();

            TryPush(root, lookup, stack, visited, cycles);
            while (stack.Count > 0)
            {
                var pair = stack.Peek();
                if (!pair.Value.MoveNext())
                {
                    stack.Pop();
                    visited[pair.Key] = VisitState.Visited;
                    pair.Value.Dispose();
                }
                else
                {
                    TryPush(pair.Value.Current, lookup, stack, visited, cycles);
                }
            }

            return cycles;
        }

        private static List<List<T>> FindCycles<T>(this IEnumerable<T> nodes, Func<T, IEnumerable<T>> edges)
        {
            var cycles = new List<List<T>>();
            var visited = new Dictionary<T, VisitState>();
            foreach (var node in nodes) cycles.AddRange(FindCycles(node, edges, visited));

            return cycles;
        }

        private static List<List<T>> FindCycles<T, TValueList>(this IDictionary<T, TValueList> listDictionary)
            where TValueList : class, IEnumerable<T>
        {
            return listDictionary.Keys.FindCycles(key =>
                listDictionary.ValueOrDefault(key, null) ?? Enumerable.Empty<T>());
        }

        private enum VisitState
        {
            NotVisited,
            Visiting,
            Visited
        }
    }
}