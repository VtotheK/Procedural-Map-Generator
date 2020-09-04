using System.Linq;
using System.Collections.Generic;
using System.Threading;


public class MapRegions
{
    const string DIRECTION_LEFT = "LEFT";
    const string DIRECTION_UP = "UP";
    const string DIRECTION_RIGHT = "RiGHT";
    const string DIRECTION_DOWN = "DOWN";

    int minCaveSize;
    int[,] regionMap;
    int regionNum = 2;
    int pointCounter = 10;

    List<Point> points = new List<Point>();
    List<Point> tempRoomEndPoints = new List<Point>();
    List<Point> roomEndPoints = new List<Point>();

    CavePathing cavePathing = new CavePathing();

    public int[,] GetRegions(int[,] map, int minCaveSize, bool drawCaveOuterLines)
    {
        this.minCaveSize = minCaveSize;
        pointCounter = minCaveSize;
        regionMap = map;
        for (int x = 0; x < regionMap.GetLength(0); x++)
        {
            for (int y = 0; y < regionMap.GetLength(1); y++)
            {
                if (regionMap[x, y] == 0) //If loop encounters background space
                {
                    var thread = new Thread( _ =>
                    FloodFill(regionMap, x,y, string.Empty),800000); //Increase stack size to 8Mb, recursive flooding causes stack overflow otherwise
                    thread.Start();
                    thread.Join();
                    if (points.Count < minCaveSize)
                    {
                        foreach (Point point in points)
                        {
                            regionMap[point.x, point.y] = 1;
                            tempRoomEndPoints.Clear();
                        }
                        points.Clear();
                        pointCounter = minCaveSize;
                    }
                    else if(points.Count >= minCaveSize)
                    {
                        foreach(Point point in tempRoomEndPoints)
                        {
                            roomEndPoints.Add(new Point(point.x, point.y, point.regionNum));
                        }
                        tempRoomEndPoints.Clear();
                        regionNum++;
                        points.Clear();
                        pointCounter = minCaveSize;
                    }
                }
            }
        }
        roomEndPoints = roomEndPoints.GroupBy(x => new { x.x, x.y, x.regionNum }).Select(g => g.First()).ToList();
        if (drawCaveOuterLines)
        {
            foreach (Point point in roomEndPoints) // Room edge points->9999, MapGenerator.MapPointsToColorArray turns room edges to grey
            {
                regionMap[point.x, point.y] = -3;
            }
        }
        CavePathing cavepath = new CavePathing();
        regionMap = cavepath.FindPaths(roomEndPoints, regionMap);
        roomEndPoints.Clear();
        tempRoomEndPoints.Clear();
        regionNum = 2;
        return regionMap;
    }

    private void FloodFill(int[,] regionMap, int x, int y, string direction)
    {
        if(regionMap[x,y] == 1)
        {
            AddTempRoomEndPoint(x, y, direction);
            return;
        }
        else if (regionMap[x, y] > 0 || x == regionMap.GetLength(0)-1 || y== regionMap.GetLength(1)-1 || x ==0 || y==0)    
        {
            return;                 
        }
        else if(regionMap[x,y] == 0)
        {
            this.regionMap[x, y] = regionNum;
            if (pointCounter >= 0)
            {
                points.Add(new Point(x, y,regionNum));
                pointCounter--;
            }
        }
        FloodFill(regionMap, x, y + 1, DIRECTION_DOWN);
        FloodFill(regionMap, x, y - 1, DIRECTION_UP);
        FloodFill(regionMap, x + 1, y, DIRECTION_RIGHT);
        FloodFill(regionMap, x - 1, y, DIRECTION_LEFT);
    }

    private void AddTempRoomEndPoint(int x, int y, string direction)
    {
        if(direction == string.Empty) { tempRoomEndPoints.Add(new Point(x, y, regionNum)); }
        else if(direction == DIRECTION_DOWN) { tempRoomEndPoints.Add(new Point(x, y - 1, regionNum)); }
        else if (direction == DIRECTION_UP) { tempRoomEndPoints.Add(new Point(x, y + 1, regionNum)); }
        else if (direction == DIRECTION_RIGHT) { tempRoomEndPoints.Add(new Point(x - 1, y, regionNum)); }
        else if (direction == DIRECTION_LEFT) { tempRoomEndPoints.Add(new Point(x + 1, y, regionNum)); }
    }
}