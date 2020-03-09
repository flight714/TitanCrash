using Godot;
using System;
using System.Collections.Generic;

public class MapBase : Node2D
{
   MoveToken token;
   Camera2D camera;

   public enum MapState {
       RUNNING,
       CHOOSING,
       PAUSED,
       CUTSCENE,
       TRANSITION
   }
   public MapState CurrentMapState = MapState.RUNNING;
    
    public override void _Ready()
    {
        camera = GetNode("Camera2D") as Camera2D;

        CreateNewMoveToken(new Vector2(300, 300), new Vector2(1,-2f));
        UpdateAllTokens();
        
        
    }

    public override void _Process(float delta)
    {
        if (CurrentMapState == MapState.RUNNING)
        {
            for (int i = 0; i < GetChildren().Count; i++)
            {
                if (GetChildren()[i] is MoveToken)
                {
                    camera.Position = (GetChildren()[i] as Node2D).Position;
                    (GetChildren()[i] as MoveToken).AssignedPath.PathProgress += delta*20f;
                }
            }
        }

        Update();
        (GetNode("ShipFrame") as ShipFrame).Position = GetGlobalMousePosition();
    }

    public override void _Draw()
    {
        for (int i = 0; i < GetChildren().Count; i++)
        {
            if (GetChildren()[i] is MoveToken)
            {
                DrawMultiline((GetChildren()[i] as MoveToken).AssignedPath.PathPoints, new Color(1,1,1,0.7f));
            }
        }
    }
    public List<CelestialBody> GetAllGravityBodies()
    {
        List<CelestialBody> GravityBodies = new List<CelestialBody>();
        for (int i = 0; i < GetChildren().Count; i++)
        {
            if (GetChildren()[i] is CelestialBody)
            {
                GravityBodies.Add(GetChildren()[i] as CelestialBody);
            }
        }
        return GravityBodies;
    }
    public void CreateNewMoveToken(Vector2 targetPosition, Vector2 startingVelocity)
    {
        MoveToken newToken = new MoveToken();
        newToken.Position = targetPosition;
        newToken.Velocity = startingVelocity;
        AddChild(newToken);
        newToken.Connect("FinishedMove", this, nameof(UpdateAllTokens));
        TrajectoryPath newPath = new TrajectoryPath();
        newPath.AssignToken(newToken);
        newToken.AssignedPath = newPath;
        newToken.AssignedPath.AddGravityBody(GetAllGravityBodies());
        newToken.AssignedPath.CreateNewPath();
        
    }
    public void UpdateAllTokens()
    {
        for (int i = 0; i < GetChildren().Count; i++)
        {
            if (GetChildren()[i] is MoveToken)
            {
                MoveToken targetToken = GetChildren()[i] as MoveToken;
                TrajectoryPath newPath = new TrajectoryPath();
                newPath.AssignToken(targetToken);
                targetToken.AssignedPath = newPath;
                targetToken.AssignedPath.AddGravityBody(GetAllGravityBodies());
                targetToken.AssignedPath.CreateNewPath();          
            }
        }
    }
    
}
