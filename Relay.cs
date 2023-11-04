using Godot;
using System;
using System.Collections.Generic;

[Tool]
public partial class Relay : Node2D
{
   public event Action<Relay> OnRelayPressed;
   public event Action<Relay> OnRelayHover;

   private bool _isSelected;

   [Export]
   public bool IsSelected
   {
      get => _isSelected;
      set
      {
         _isSelected = value;
         Update();
      }
   }

   [Export]
   public Godot.Collections.Array<NodePath> AdjacentNodesPaths = new Godot.Collections.Array<NodePath>();

   public List<Relay> AdjacentNodes = new List<Relay>();

   private void Update()
   {
      Modulate = IsSelected ? Colors.Red : Colors.White;
   }

   override public void _Ready()
   {
      var area = GetNode<Area2D>("Area2D");
      area.InputEvent += OnInputEvent;
      Update();

      foreach (var path in AdjacentNodesPaths)
      {
         var node = GetNode<Relay>(path);
         if (node != null)
         {
            AdjacentNodes.Add(node);
            if (!node.AdjacentNodes.Contains(this))
            {
               node.AdjacentNodes.Add(this);
            }
         }
      }
   }

   private void OnInputEvent(Node viewport, InputEvent @event, long shapeIdx)
   {
      if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed)
      {
         OnRelayPressed?.Invoke(this);
      }

      if (@event is InputEventMouseMotion mouseMotion)
      {
         OnRelayHover?.Invoke(this);
      }      
   }
}
