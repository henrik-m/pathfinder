
using System.Collections.Generic;

public class Graph
{
   public Dictionary<string, Dictionary<string, int>> Vertices { get; set; }

   public Graph()
   {
      Vertices = new Dictionary<string, Dictionary<string, int>>();
   }

   public Graph(Graph other)
   {
      Vertices = new Dictionary<string, Dictionary<string, int>>(other.Vertices);
      foreach (var kvp in other.Vertices)
      {
         Vertices[kvp.Key] = new Dictionary<string, int>(kvp.Value);
      }
   }

   public void AddVertex(string name, Dictionary<string, int> edges)
   {
      Vertices[name] = edges;
   }

   public void RemoveEdge(string from, string to)
   {
      // Remove the edge.
      if (Vertices.ContainsKey(from) && Vertices[from].ContainsKey(to))
      {
         Vertices[from].Remove(to);
      }
   }
}
