using System.Collections.Generic;
using System.Linq;

public class YenKShortestPaths
{
   private readonly Graph graph;
   private readonly string source;
   private readonly string sink;

   public YenKShortestPaths(Graph graph, string source, string sink)
   {
      this.graph = graph;
      this.source = source;
      this.sink = sink;
   }

   public List<(int Distance, List<string> Path)> FindKShortestPaths(int k)
   {
      var A = new List<(int Distance, List<string> Path)>(); // Stores the shortest paths
      var B = new SortedSet<(int, List<string>)>(Comparer<(int, List<string>)>.Create((a, b) => a.Item1.CompareTo(b.Item1))); // Potential kth shortest paths

      // Determine the shortest path from the source to the sink.
      var shortestPath = Dijkstra.CalculateShortestPathToTarget(graph, source, sink);
      A.Add(shortestPath);

      for (int i = 1; i < k; i++)
      {
         // The spur node ranges from the first node to the next to last node in the previous k-shortest path.
         for (int j = 0; j < A[i - 1].Path.Count - 1; j++)
         {
            // Spur node is retrieved from the previous k-shortest path, k − 1.
            string spurNode = A[i - 1].Path[j];
            // The sequence of nodes from the source to the spur node of the previous k-shortest path.
            var rootPath = A[i - 1].Path.Take(j + 1).ToList();

            Graph newGraph = new Graph(graph);

            foreach (var path in A)
            {
               if (rootPath.SequenceEqual(path.Path.Take(j + 1)))
               {
                  newGraph.RemoveEdge(path.Path[j], path.Path[j + 1]); // Remove the links that are part of the previous shortest paths which share the same root path.
               }
            }

            // Calculate the spur path from the spur node to the sink.
            var spurPath = Dijkstra.CalculateShortestPathToTarget(newGraph, spurNode, sink).Path.Skip(1).ToList(); // Skip the spur node itself

            if (spurPath.Any())
            {
               // Entire path is made up of the root path and spur path.
               var totalPath = rootPath.Concat(spurPath).ToList();
               var totalDistance = CalculatePathDistance(totalPath);
               // Add the potential k-shortest path to the heap.
               B.Add((totalDistance, totalPath));
            }
         }

         if (B.Count == 0)
         {
            // This means there are no more paths.
            break;
         }

         // Add the lowest cost path becomes the k-shortest path.
         A.Add(B.First());
         B.Remove(B.First());
      }

      return A;
   }

   private int CalculatePathDistance(List<string> path)
   {
      int distance = 0;
      for (int i = 0; i < path.Count - 1; i++)
      {
         distance += graph.Vertices[path[i]][path[i + 1]];
      }
      return distance;
   }
}
