using System;
using System.Collections.Generic;
using UnityEngine;

public class CavePathing   {
    const string DIRECTION_RIGHT = "RIGHT";
    const string DIRECTION_LEFT = "LEFT";
    const string DIRECTION_UP = "UP";
    const string DIRECTION_DOWN = "DOWN";
    const string DIRECTION_DOWNRIGHT = "DOWNRIGHT";
    const string DIRECTION_DOWNLEFT = "DOWNLEFT";
    const string DIRECTION_UPRIGHT = "UPRIGHT";
    const string DIRECTION_UPLEFT = "UPLEFT";

    int[,] tempMap;
    int stepsShort = 99999;
    int currentRegionNum = -1;
    int x, y;

    List<List<Point>> pathways = new List<List<Point>>();
    List<Point> pathway = new List<Point>();
    List<Point> shortestPath = new List<Point>();
    System.Random random = new System.Random();
    public int[,] FindPaths(List<Point> roomEndPoints, int[,] map)
    {
        roomEndPoints.Sort((x, y) => x.regionNum.CompareTo(y.regionNum));
        tempMap = map;
        foreach (Point point in roomEndPoints)
        {
            x = point.x; y = point.y;

            if(currentRegionNum < 0 || point.regionNum != currentRegionNum)
            {
                if(shortestPath.Count>0)
                {
                    pathways.Add(new List<Point>(shortestPath));
                    shortestPath.Clear();
                    stepsShort = 99999;
                }
                currentRegionNum = point.regionNum;
            }

            Move(x + 1 , y, DIRECTION_RIGHT);
            SavePath();
            Move(x - 1 , y, DIRECTION_LEFT);
            SavePath();
            Move(x , y - 1 , DIRECTION_UP);
            SavePath();
            Move(x , y + 1 , DIRECTION_DOWN);
            SavePath();
            Move(x - 1 , y + 1 , DIRECTION_DOWNLEFT);
            SavePath();
            Move(x + 1 , y +1 , DIRECTION_DOWNRIGHT);
            SavePath();
            Move(x - 1 , y - 1 , DIRECTION_UPLEFT);
            SavePath();
            Move(x + 1, y - 1, DIRECTION_UPRIGHT);
            SavePath();
        }
        DrawShortestPath(pathways);

        return tempMap;
    }

    private void DrawShortestPath(List<List<Point>> pathways)
    {
        try
        {
            foreach (List<Point> pointList in pathways)
            {
                foreach (Point point in pointList)
                {
                    if (point.x > 5 && point.y > 6)
                    {
                        for (int i = random.Next(point.x-2, point.x - 0); i < random.Next(point.x +1, point.x + 2); i++)
                        {
                            for (int j = random.Next(point.y-2, point.y - 0); j < random.Next(point.y +2, point.y + 3); j++)
                            {
                                tempMap[i, j] = point.regionNum;
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("Called");
                        tempMap[point.x, point.y] = point.regionNum;
                    }
                }
            }
            pathways.Clear();
        }

        catch (Exception e) { Debug.LogWarning("No shortest path found in CavePathing.cs"); }
        
        }


    private void SavePath()
    {
        if (pathway.Count < stepsShort && pathway.Count>=1)
        {
            shortestPath = new List<Point>(pathway);
            stepsShort = pathway.Count;
        }
        pathway.Clear();
    }

    private void Move(int x, int y, string direction)
    {
        if(x < 0 || y < 0 || x == tempMap.GetLength(0) || y == tempMap.GetLength(1)) { pathway.Clear(); return; }
        else if(tempMap[x,y] == currentRegionNum || tempMap[x,y] == -3) { pathway.Clear(); return; }
        else if(tempMap[x,y] > 1 && tempMap[x,y] != currentRegionNum ) { return; }
        else if(tempMap[x,y] == 1)
        {
            pathway.Add(new Point(x, y, currentRegionNum));
        }
        
        switch(direction)
        {
            case "RIGHT":
                Move(x + 1, y, DIRECTION_RIGHT);
                break;
            case "LEFT":
                Move(x - 1, y, DIRECTION_LEFT);
                break;
            case "UP":
                Move(x, y - 1, DIRECTION_UP);
                break;
            case "DOWN":
                Move(x, y +1, DIRECTION_DOWN);
                break;
            case "DOWNLEFT":
                Move(x -1 , y + 1, DIRECTION_DOWNLEFT);
                break;
            case "DOWNRIGHT":
                Move(x + 1, y + 1, DIRECTION_DOWNRIGHT);
                break;
            case "UPRIGHT":
                Move(x + 1, y - 1, DIRECTION_UPRIGHT);
                break;
            case "UPLEFT":
                Move(x - 1, y - 1, DIRECTION_UPLEFT);
                break;
            default:
                throw new UnityException("Can not recieve correct move direction from string constats in CavePathing.cs");
        }
    }
}
