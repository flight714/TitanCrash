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
        if (ShipWidth.IsEven())
        {
            ShipWidth += 1;
        }

        FrameSpace = new int[ShipWidth, ShipHeight][];
        rowMiddle = FrameSpace.GetLength(1).GetMiddleValue();
        columnMiddle = FrameSpace.GetLength(0).GetMiddleValue();
        shipMiddle = ShipLayout.Count.GetMiddleValue();
        for (int i = 0; i < FrameSpace.GetLength(0); i++)
        {
            for (int j = 0; j < FrameSpace.GetLength(1); j++)
            {
                int spaceStatus = 0; // 0 none, 1 vacant, 2 filled
                int currentRow = j+1;
                int currentColumn = i+1;

                if (ShipLayout.ContainsKey(currentRow))
                {
                    int roomsToAdd = ShipLayout[currentRow];
                    if (roomsToAdd.IsEven())
                    {
                        int split = roomsToAdd/2;
                        if (currentColumn == columnMiddle)
                        {
                            spaceStatus = 0;
                        }
                        else
                        {
                            if ((currentColumn >= columnMiddle - split && currentColumn < columnMiddle) || (currentColumn <= columnMiddle + split && currentColumn > columnMiddle))
                            {
                                spaceStatus = 1;
                            }
                        }
                    }
                    else if (!roomsToAdd.IsEven())
                    {
                        if (roomsToAdd == 1)
                        {
                            if (currentColumn == columnMiddle)
                            {
                                spaceStatus = 1;
                            }
                        }
                        else
                        {
                            int split = (roomsToAdd-1)/2;
                            if (currentColumn == columnMiddle)
                            {
                                spaceStatus = 1;
                            }
                            if ((currentColumn >= columnMiddle - split && currentColumn < columnMiddle) || (currentColumn <= columnMiddle + split && currentColumn > columnMiddle))
                            {
                                spaceStatus = 1;
                            }
                        }
                    }
                }
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
    // USE THIS INSTEAD OF LOOKAT!!
    public void LookAtAdjusted(Vector2 target)
    {
        Rotation = -Mathf.Deg2Rad(90) + Position.AngleToPoint(target);
    }
}
