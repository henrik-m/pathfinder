using Godot;
using System;
using System.Collections.Generic;

public partial class GraphManager : Node2D
{
   private List<Relay> _relays = new List<Relay>();
   private Relay _selectedRelay;
   private Relay _startRelay;
   private Graph _graph;

   public override void _Ready()
   {
      var nodes = GetTree().GetNodesInGroup("RelayNodes");
      foreach (Relay node in nodes)
      {
         if (node.IsSelected)
         {
            if (_selectedRelay == null)
            {
               _selectedRelay = node;
            }
            else
            {
               node.IsSelected = false;
            }
         }

         _relays.Add(node);
         node.OnRelayPressed += OnRelayPressed;
         node.OnRelayHover += OnRelayHover;

         foreach (var adjacentNode in node.AdjacentNodes)
         {            
            DrawConnection(node, adjacentNode);
         }

         _graph = CreateGraph();
      }
   }

   private Graph CreateGraph()
   {
      var graph = new Graph();
      foreach (var relay in _relays)
      {
         var edges = new Dictionary<string, int>();
         foreach (var adjacentNode in relay.AdjacentNodes)
         {
            edges[adjacentNode.GetPath()] = 1;
         }
         graph.AddVertex(relay.GetPath(), edges);
      }
      return graph;
   }

   private void OnRelayHover(Relay relay)
   {
      if (relay != _startRelay)
      {
         _startRelay = relay;
         HighlightShortestPaths();
      }
   }

   private void HighlightShortestPaths()
   {
      foreach (var node in GetChildren())
      {
         if (node is Line2D line)
         {
            GD.Print("Resetting line: " + line.GetPath());
            line.Modulate = Colors.White;
            line.Visible = false;
         }
      }

      if (_startRelay != null && _selectedRelay != null)
      {
         var yensKShortestPaths = new YenKShortestPaths(_graph, _startRelay.GetPath(), _selectedRelay.GetPath());
         var shortestPaths = yensKShortestPaths.FindKShortestPaths(2);
         var distance = shortestPaths[0].Distance;

         foreach (var shortestPath in shortestPaths)
         {
            if (distance != shortestPath.Distance)
            {
               break;
            }

            // Highlight the lines between the nodes that are part of the shortest path.
            for (int i = 0; i < shortestPath.Path.Count - 1; i++)
            {
               var linePath = shortestPath.Path[i].Replace("/", "_") + "->" + shortestPath.Path[i + 1].Replace("/", "_");
               var line = GetNodeOrNull<Line2D>(linePath);
               if (line != null)
               {
                  GD.Print("Highlighting line: " + line.Name);
                  line.Modulate = Colors.Red;
                  line.Visible = true;
               }
               else
               {
                  GD.Print("Line not found: " + shortestPath.Path[i] + "->" + shortestPath.Path[i + 1]);
               }
            }
         }
         
      }
   }

   private void DrawConnection(Relay node1, Relay node2)
   {
      var line = new Line2D();
      line.Name = node1.GetPath() + "->" + node2.GetPath();
      line.Points = new Vector2[] { node1.GlobalPosition, node2.GlobalPosition };
      AddChild(line);
   }

   private void OnRelayPressed(Relay relay)
   {
      if (relay.IsSelected)
      {
         relay.IsSelected = false;
         _selectedRelay = null;
      }
      else
      {
         if (_selectedRelay != null)
         {
            _selectedRelay.IsSelected = false;
         }
         _selectedRelay = relay;
         _selectedRelay.IsSelected = true;
      }
   }
}
