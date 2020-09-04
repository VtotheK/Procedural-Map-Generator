using UnityEngine;
using UnityEngine.Tilemaps;

public class TilePlacer : MonoBehaviour {
    [Header("Floor Tiles")]
    [SerializeField] Tile squareTile;
    [SerializeField] Tile leftBottomRightUpTileFloor;
    [SerializeField] Tile rightBottomLeftUpTileFloor;
    [SerializeField] Tile pyramidTopTileFloor;
    [SerializeField] Tile floorTile;
    [Header("Ceiling Tiles")]
    [SerializeField] Tile leftBottomRightUpCeiling;
    [SerializeField] Tile rightBottomLeftUpCeiling;
    [SerializeField] Tile pyramidTopTileCeiling;

    [SerializeField] Tile[] decorations;
    Tilemap tilemap;
    int x, y;
    private void Start()
    {
        tilemap = GetComponent<Tilemap>();
    }

    public void SetTiles(int[,] map)
    {
        tilemap.ClearAllTiles();
        for (int x = 1; x < map.GetLength(0)-1 ; x++)
        {
            for (int y = 1; y < map.GetLength(1)-1; y++)
            {
                if (EmptyNeightbours(x, y, map))
                {
                    continue;
                }
                else if (map[x, y]  == 1)
                { 
                     if (map[x,y+1] > 1 && map[x + 1, y ] == 1 && map[x, y - 1] == 1 && map[x + 1, y - 1] == 1 && map[x-1,y] > 1)
                    {
                        tilemap.SetTile(new Vector3Int(x, y, 0), leftBottomRightUpTileFloor);
                    }
                    else if(map[x-1,y]>1 && map[x-1, y+1] > 1 && map[x, y+1] > 1 && map[x+1, y+1] > 1 && map[x+1, y] > 1 && map[x, y-1] == 1)
                    {
                        tilemap.SetTile(new Vector3Int(x, y, 0), pyramidTopTileFloor);
                    }
                    else if(map[x-1,y] == 1  && map[x, y-1] == 1 && map[x+1, y] > 1 && map[x, y+1] > 1 && map[x+1, y+1] > 1)
                    {
                        tilemap.SetTile(new Vector3Int(x, y, 0), rightBottomLeftUpTileFloor);
                    }
                    else if (map[x,y-1] > 1 && map[x-1,y] == 1 && map[x+1, y-1] > 1 && map[x+1, y] >1 && map[x,y+1] == 1)
                    {
                        tilemap.SetTile(new Vector3Int(x, y, 0), leftBottomRightUpCeiling);
                    }
                    else if (map[x + 1, y] == 1 && map[x,y+1] == 1 && map[x-1,y] > 1 && map[x-1,y+1] == 1)
                    {
                        tilemap.SetTile(new Vector3Int(x, y, 0), rightBottomLeftUpCeiling);
                    }
                    else if (map[x + 1, y] > 1 && map[x + 1, y + 1] == 1 && map[x, y - 1] > 1 && map[x - 1, y - 1] > 1 && map[x - 1, y] > 1 && map[x, y + 1] == 1)
                    {
                        tilemap.SetTile(new Vector3Int(x, y, 0), pyramidTopTileCeiling);
                    }
                    else if(map[x,y+1] > 1 || map[x,y+1] <-1)
                    {
                        SetDecoration(map, x,y);
                        tilemap.SetTile(new Vector3Int(x, y, 0), floorTile);
                    }
                    else
                    {
                        tilemap.SetTile(new Vector3Int(x, y, 0), squareTile);
                    }
                }
            }
        }
        tilemap.SetTile(new Vector3Int(0,0,0), leftBottomRightUpTileFloor);
    }

    private void SetDecoration(int[,] map, int x, int y)
    {
       if(UnityEngine.Random.Range(0,101)>95)
        {
            tilemap.SetTile(new Vector3Int(x, y + 1, 0), decorations[UnityEngine.Random.Range(0, decorations.Length - 1)]);
        }
    }

    private bool EmptyNeightbours(int x, int y, int[,] map)
    {
        int count = 0;
        for (int xNeighbour = x - 1; xNeighbour <= x+1 ; xNeighbour++)
        {
            for (int yNeighbour = y - 1; yNeighbour <= y + 1; yNeighbour++)
            {
                if(map[xNeighbour,yNeighbour] > 1 || map[xNeighbour,yNeighbour] == 0)
                {
                    count++;  
                }
            }
        }
        return count == 9 ? true : false;
    }
}
