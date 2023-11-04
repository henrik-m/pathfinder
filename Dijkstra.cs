
using System;
using System.Collections.Generic;

public class Dijkstra
{

   public class PriorityQueue<T>
   {
      private List<(int Priority, T Item)> baseHeap;

      public PriorityQueue()
      {
         this.baseHeap = new List<(int Priority, T Item)>();
      }

      public void Enqueue(int priority, T item)
      {
         baseHeap.Add((priority, item));
         this.HeapifyFromEndToBeginning(baseHeap.Count - 1);
      }

      private void HeapifyFromEndToBeginning(int pos)
      {
         if (pos >= baseHeap.Count) return;

         while (pos > 0)
         {
            int parentPos = (pos - 1) / 2;
            if (baseHeap[parentPos].Priority < baseHeap[pos].Priority) break; // Correctly placed
            (baseHeap[parentPos], baseHeap[pos]) = (baseHeap[pos], baseHeap[parentPos]); // Swap
            pos = parentPos;
         }
      }

      public T Dequeue()
      {
         if (!this.IsEmpty)
         {
            T result = baseHeap[0].Item;
            baseHeap[0] = baseHeap[baseHeap.Count - 1];
            baseHeap.RemoveAt(baseHeap.Count - 1);
            this.HeapifyFromBeginningToEnd(0);
            return result;
         }

         throw new InvalidOperationException("The queue is empty");
      }

      private void HeapifyFromBeginningToEnd(int pos)
      {
         if (pos >= baseHeap.Count) return;

         while (true)
         {
            int smallest = pos;
            int left = 2 * pos + 1;
            int right = 2 * pos + 2;
            if (left < baseHeap.Count && baseHeap[left].Priority < baseHeap[smallest].Priority)
               smallest = left;
            if (right < baseHeap.Count && baseHeap[right].Priority < baseHeap[smallest].Priority)
               smallest = right;
            if (smallest == pos) break;
            (baseHeap[smallest], baseHeap[pos]) = (baseHeap[pos], baseHeap[smallest]);
            pos = smallest;
         }
      }

      public bool IsEmpty => baseHeap.Count == 0;
   }

   public static (int Distance, List<string> Path) CalculateShortestPathToTarget(Graph graph, string start, string target)
   {
      var previous = new Dictionary<string, string>();
      var distances = new Dictionary<string, int>();
      var nodes = new PriorityQueue<string>();

      // Initialize all distances as infinite except for the start vertex.
      foreach (var vertex in graph.Vertices)
      {
         if (vertex.Key == start)
         {
            distances[vertex.Key] = 0;
            nodes.Enqueue(0, vertex.Key);
         }
         else
         {
            distances[vertex.Key] = int.MaxValue;
            nodes.Enqueue(int.MaxValue, vertex.Key);
         }

         previous[vertex.Key] = null;
      }

      while (!nodes.IsEmpty)
      {
         var smallest = nodes.Dequeue();

         if (smallest == target)
            break; // Exit if the target has been reached.

         if (distances[smallest] == int.MaxValue)
            break;

         foreach (var neighbor in graph.Vertices[smallest])
         {
            var alt = distances[smallest] + neighbor.Value;
            if (alt < distances[neighbor.Key])
            {
               distances[neighbor.Key] = alt;
               previous[neighbor.Key] = smallest;
               nodes.Enqueue(alt, neighbor.Key);
            }
         }
      }

      var path = GetPathFromPreviousMap(previous, target);
      return (distances[target], path);
   }

   private static List<string> GetPathFromPreviousMap(Dictionary<string, string> previous, string vertex)
   {
      var path = new List<string>();
      while (vertex != null)
      {
         path.Add(vertex);
         vertex = previous[vertex];
      }

      path.Reverse();
      return path;
   }
}
