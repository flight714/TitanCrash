using Godot;
using System;
using System.Collections.Generic;

public class ShipFrame : Node2D
{
    [Export]
    public Dictionary<int,int> ShipLayout = new Dictionary<int, int>()
    {
        {1, 1},
        {2, 3},
        {3, 3},
        {4, 3},
        {5, 2}
    };
    [Export]
    public Color DrawColor = new Color(1,1,1);
    // Drawable area

    // numerical code: 0 empty, 1, open, 2 filled
    public int[,][] FrameSpace = new int[5,3][];
    int rowMiddle = 6;
    int columnMiddle = 6;
    int shipMiddle = 3;
    public override void _Ready()
    {
        SetupShipLayout();
    }
    public override void _Process(float delta)
    {
        Update();
    }
    public override void _Draw()
    {
        int currentRow = -rowMiddle;
        for (int i = 0; i < FrameSpace.GetLength(0); i++)
        {
            currentRow += 1;
            int currentColumn = -columnMiddle;
            for (int j = 0; j < FrameSpace.GetLength(1); j++)
            {
                currentColumn += 1;
                if (FrameSpace[i,j][2] > 0)
                {
                    DrawRect(GetBox(4f, new Vector2(currentRow*5, currentColumn*5)), DrawColor);
                }
                else
                {
                    DrawRect(GetBox(4f, new Vector2(currentRow*5, currentColumn*5)), new Color(1,1,1,0.2f));
                }
            }
        }
    }
    public void SetupShipLayout()
    {
        int ShipHeight = ShipLayout.Keys.Count;
        int ShipWidth;
        int[] widths = new int[ShipHeight];
        foreach (int num in ShipLayout.Keys)
        {
            widths[num-1] = ShipLayout[num];
        }
        ShipWidth = widths.GetHighest();
        FrameSpace = new int[ShipWidth, ShipHeight][];

        rowMiddle = FrameSpace.GetLength(0).GetMiddleValue();
        columnMiddle = FrameSpace.GetLength(1).GetMiddleValue();
        shipMiddle = ShipLayout.Count.GetMiddleValue();

        for (int i = 0; i < FrameSpace.GetLength(0); i++)
        {
            for (int j = 0; j < FrameSpace.GetLength(1); j++)
            {
                int spaceStatus = 1; // 0 none, 1 vacant, 2 filled
                
                FrameSpace[i,j] = new int[3]{i,j,spaceStatus};
            }
        }
        
    }
    public Rect2 GetBox(float size, Vector2 location)
    {
        Rect2 newRect = new Rect2();
        newRect.Size = new Vector2(size, size);
        newRect.Position = location;
        return newRect;
    }
}
