using Godot;
using System;

public class CelestialBody : Node2D
{
    [Export]
    public float Radius = 50f;
    [Export]
    public Color BodyColor = new Color(1f,1f,1f);
    [Export]
    public bool HasAtmo = false;

    private Sprite planetImage;
    private Sprite planetAtmoImage;
    public override void _Ready()
    {
        setupPlanetTexture();
    }

    private void setupPlanetTexture()
    {
        planetImage = GetNode("Sprite") as Sprite;
        planetAtmoImage = GetNode("Atmo") as Sprite;
        planetImage.Scale = new Vector2(1,1) * (1.0f/1200.0f) * Radius;
        planetImage.Modulate = BodyColor;
        if (HasAtmo)
        {
            planetAtmoImage.Scale = new Vector2(1,1) * (1.0f/1200.0f) * (Radius*1.1f);
            planetAtmoImage.Modulate = BodyColor.Lightened(0.2f);
        }
        else
        {
            planetAtmoImage.QueueFree();
        }
    }

    public float GetMass()
    {
        return Radius*1e10f;
    }

}
