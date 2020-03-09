using Godot;
using System;
using System.Collections.Generic;

public class TrajectoryPath : Node
{
    [Export]
    public Vector2 CurrentVelocity = new Vector2();
    public Vector2 CurrentPosition = new Vector2();
    public Vector2[] PathPoints = new Vector2[100];
    [Export]
    public Vector2[] VelocityChanges = new Vector2[100];
    public List<CelestialBody> GravityBodies = new List<CelestialBody>();
    public Curve2D PathCurve = new Curve2D();
    public float PathProgress = 0.0f;
    public MoveToken AssignedToken;
    public override void _Ready()
    {

    }

    public override void _Process(float delta)
    {
        
    }

    public void CreateNewPath()
    {
        Vector2 currentPos = CurrentPosition;
        Vector2 currentVel = CurrentVelocity;
        for (int i = 0; i < PathPoints.Length; i++)
        {
            currentVel += VelocityChanges[i];
            currentVel += GetGravityForces(currentPos);
            currentPos += currentVel;
            PathPoints[i] = currentPos;
        }
        PathCurve.ClearPoints();
        for (int i = 0; i < PathPoints.Length; i++)
        {
            PathCurve.AddPoint(PathPoints[i]);
        }
        CurrentVelocity = currentVel;
    }

    public Vector2 GetGravityForces(Vector2 currentPos)
    {
        Vector2 forces = new Vector2();

        foreach (CelestialBody body in GravityBodies)
        {
            forces += new Vector2(1,1) * (GravityData.GRAVITY*GravityData.GravityModifier*body.GetMass())/currentPos.DistanceSquaredTo(body.Position);
        }

        return forces;
    }

    public void AddGravityBody(CelestialBody newBody)
    {
        if (!GravityBodies.Contains(newBody))
        {
            GravityBodies.Add(newBody);
        }
    }
    public void AddGravityBody(List<CelestialBody> newBodies)
    {
        foreach (CelestialBody body in newBodies)
        {
            AddGravityBody(body);
        }
    }
    public void AssignToken(MoveToken token)
    {
        AssignedToken = token;
        token.AssignedPath = this;
        CurrentPosition = token.Position;
        CurrentVelocity = token.Velocity;
    }
    public Vector2 GetPositionAtTime(float offset)
    {
        return PathCurve.Interpolatef(offset);
    }

}
