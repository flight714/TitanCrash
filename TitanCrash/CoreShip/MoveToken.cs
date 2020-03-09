using Godot;
using System;
using System.Collections.Generic;

public class MoveToken : Node2D
{
   
    [Export]
    public Vector2 Velocity = new Vector2();
    public TrajectoryPath AssignedPath;
    [Signal]
    public delegate void FinishedMove();

    public override void _Ready()
    {

    }
    public override void _Process(float delta)
    {
        Update();
        if (AssignedPath != null)
        {
            if (AssignedPath.PathProgress > 100f)
            {
                Velocity = AssignedPath.CurrentVelocity;
                EmitSignal(nameof(FinishedMove));
            }
            else
            {
                Position = AssignedPath.GetPositionAtTime(AssignedPath.PathProgress);
            }
        }
    }
    public override void _Draw()
    {
        DrawCircle(new Vector2(), 2f, new Color(1,1,1));
    }

}
