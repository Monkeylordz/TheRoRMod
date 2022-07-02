using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoRMod.Utilities
{
    /// <summary>
    /// A graph data structure with weighted edges
    /// </summary>
    /// <typeparam name="T">The type of the vertices</typeparam>
    /// <typeparam name="V">The type of the edge weights</typeparam>
    public class WeightedGraph<T, V> where V : IComparable
    {
        public struct Edge
        {
            public T A;
            public T B;
            public V Weight;
            
            public Edge(T a, T b, V weight)
            {
                A = a;
                B = b;
                Weight = weight;
            }

            public static int Compare(Edge e1, Edge e2)
            {
                return e1.Weight.CompareTo(e2.Weight);
            }

            public override bool Equals([NotNullWhen(true)] object obj)
            {
                if (!GetType().Equals(obj.GetType())) return false;
                Edge e = (Edge)obj;
                return (A, B, Weight).Equals((e.A, e.B, e.Weight));
            }

            public override int GetHashCode()
            {
                int hash = A.GetHashCode() ^ B.GetHashCode();
                return unchecked(hash * 17 + Weight.GetHashCode());
            }
        }

        public delegate V WeightFunction(T a, T b);

        public HashSet<T> Vertices { get; private set; }
        public HashSet<Edge> Edges { get; private set; }

        public WeightedGraph()
        {
            Vertices = new HashSet<T>();
            Edges = new HashSet<Edge>();
        }

        /// <summary>
        /// Adds a new vertex to the graph
        /// </summary>
        /// <param name="value">The value of the vertex</param>
        public void AddVertex(T value)
        {
            Vertices.Add(value);
        }

        /// <summary>
        /// Adds a new edge between two vertices to the graph
        /// </summary>
        /// <param name="a">The first vertex</param>
        /// <param name="b">The second vertex</param>
        /// <param name="weight">The weight of the edge</param>
        public void AddEdge(T a, T b, V weight)
        {
            Edges.Add(new Edge(a, b, weight));
        }

        /// <summary>
        /// Adds a new edge between two vertices to the graph
        /// </summary>
        /// <param name="a">The first vertex</param>
        /// <param name="b">The second vertex</param>
        /// <param name="weight">The weight function use to determine the edge weight</param>
        public void AddEdge(T a, T b, WeightFunction weightFunction)
        {
            AddEdge(a, b, weightFunction(a, b));
        }

        /// <summary>
        /// Creates a fully connected graph from a collection of objects using a function to determine the weights
        /// </summary>
        /// <param name="vertices">The values of each vertex</param>
        /// <param name="weightFunction">The function used to determine the edge weights</param>
        /// <returns>The graph</returns>
        public static WeightedGraph<T, V> FullyConnectedGraph(List<T> vertices, WeightFunction weightFunction)
        {
            WeightedGraph<T, V> graph = new WeightedGraph<T, V>();

            // Add a vertex for each object
            foreach (T obj in vertices)
            {
                graph.AddVertex(obj);
            }

            // Add an edge between each pair of vertices
            for (int i = 0; i < vertices.Count - 1; i++)
            {
                for (int j = i + 1; j < vertices.Count; j++)
                {
                    graph.AddEdge(vertices[i], vertices[j], weightFunction);
                }
            }

            return graph;
        }

        /// <summary>
        /// Calculates a minimum spanning tree across the graph
        /// </summary>
        /// <returns>A list of edges that compose the tree</returns>
        public List<Edge> MinimumSpanningTree()
        {
            // Uses Kruskal's Algorithm
            var mst = new List<Edge>();

            // Sort edges by weight
            var edges = Edges.ToList();
            edges.Sort(Edge.Compare);

            // Use dictionary to track vertex subsets
            var parentDictionary = new Dictionary<T, T>();
            T Root(T v) => parentDictionary.ContainsKey(v) ? Root(parentDictionary[v]) : v;

            foreach (Edge edge in edges)
            {
                // Check vertex subsets using roots
                T rootA = Root(edge.A);
                T rootB = Root(edge.B);

                // Roots are different: add to MST
                if (!rootA.Equals(rootB))
                {
                    mst.Add(edge);
                    parentDictionary[rootB] = rootA;
                }

                // Break when the MST has |V| - 1 edges
                if (mst.Count >= Vertices.Count - 1) break;
            }

            return mst;
        }
    }
}
