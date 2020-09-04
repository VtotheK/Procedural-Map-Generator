using UnityEngine;


public class MapGenerator : MonoBehaviour
{
    [SerializeField] int width = 256, height = 256;
    [SerializeField] bool useRandomSeed = false;
    [SerializeField] string seed;

    [Range(0, 100)][Tooltip("The amount of white noise in texture")]
    [SerializeField] int fillAmount = 40;

    [Tooltip("Minimum cave size")]
    [SerializeField] int minCaveSize = 10;

    [Range(1, 9)][Tooltip("Neighbour wall smoothing threshold")]
    [SerializeField] int neighbourThreshold = 4;

    [Range(0, 20)][Tooltip("How many times the script run a noise smoothing algorithm")]
    [SerializeField] int smoothIterations = 4;

    [Tooltip("Erases lone dots from the map when checked")]
    [SerializeField] bool eraseSingleDots = false;

    [SerializeField] bool drawCaveOuterLines = false;

    [SerializeField] int cavePathWidth = 1;
    int[,] map;
    Color[,] colors;

    TilePlacer tilePlacer;
    MapRegions regions;
    Texture2D texture;
    System.Random pseudoRandom;
    SpriteRenderer renderer;
    // Use this for initialization
    void Start()
    {
        regions =new MapRegions();
        renderer = GetComponent<SpriteRenderer>();
        tilePlacer = FindObjectOfType<TilePlacer>();
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Home))
        {
            if (texture != null) { Destroy(texture); }

            texture = new Texture2D(width, height);
            texture.filterMode = FilterMode.Point;
            colors = new Color[width, height];
            map = new int[width, height];
            //FindObjectOfType<CameraControl>().SetAspectRatio(width, height); //Causes error, fix
            //FindObjectOfType<CameraControl>().MoveCamera(width, height);

            if (useRandomSeed) //Get a new random seed everytime a map will be created
            {                  // if useRandomSeed is true.
                seed = System.DateTime.Now.Ticks.ToString();
                pseudoRandom = new System.Random(seed.GetHashCode());
            }
            else
            {
                pseudoRandom = new System.Random(seed.GetHashCode());
            }

            GenerateMap();
            for (int i = 0; i < smoothIterations; i++)
            {
                SmoothMap();
            }
            if (eraseSingleDots)
            {
                EraseSingleDots();
            }
            map = regions.GetRegions(map, minCaveSize, drawCaveOuterLines);
            EraseSingleDots();
            tilePlacer.SetTiles(map);
            //MapPointsToColorArray();
            //SetColorsToTexture(colors);
        }
    }

    private void MapPointsToColorArray()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == 0) { colors[x, y] = Color.white; }
                else if (map[x, y] == 1) { colors[x, y] = Color.black; }
                else if (map[x, y] == 2) { colors[x, y] = Color.red; }
                else if (map[x, y] == 3) { colors[x, y] = Color.blue; }
                else if (map[x, y] == 9999) { colors[x, y] = Color.gray; }
                else if (map[x, y] == -3) { colors[x, y] = Color.gray; }//Colors the edge of the rooms to grey
                else { colors[x, y] = Color.magenta; }
            }
        }

    }

    private void GenerateMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    map[x, y] = 1;
                }
                else
                {
                    map[x, y] = pseudoRandom.Next(0, 100) > fillAmount ? 1 : 0;
                }
            }
        }
    }

    void SmoothMap()
    {
        map = CellularAutomata(map);
    }

    int[,] CellularAutomata(int[,] map)
    {
        int wallCount = 0;
        int[,] nextGeneration = new int[width, height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (x > 1 && y > 1 && x < width - 2 && y < height - 2)
                {
                    for (int yNeighbour = y - 1; yNeighbour <= y + 1; yNeighbour++)
                    {
                        for (int xNeighbour = x - 1; xNeighbour <= x + 1; xNeighbour++)
                        {
                            wallCount += map[xNeighbour, yNeighbour];
                        }
                    }
                    if (wallCount > neighbourThreshold)
                    {
                        nextGeneration[x, y] = 1;
                        wallCount = 0;
                    }
                    else if (wallCount < neighbourThreshold)
                    {
                        nextGeneration[x, y] = 0;
                        wallCount = 0;
                    }
                }
                else
                {
                    nextGeneration[x, y] = 1;
                }
            }
        }
        return nextGeneration;
    }

    private void SetColorsToTexture(Color[,] colors)
    {
        for (int y = 0; y < colors.GetLength(1); y++)
        {
            for (int x = 0; x < colors.GetLength(0); x++)
            {
                texture.SetPixel(x, y, colors[x, y]);
            }
        }
        texture.Apply();
        renderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0), 1);
    }

    private void EraseSingleDots()
    {
        for (int y = 1; y < height - 2; y++)
        {
            for (int x = 1; x < width - 2; x++)
            {
                if(map[x+1,y] == 0 && map[x -1, y] == 0 && map[x, y +1] == 0 && map[x , y -1] == 0)
                {
                    map[x, y] = 0;
                }
                else if (map[x, y] > 1)
                {
                    if (map[x + 1, y] == 1 && map[x - 1, y] == 1 && map[x, y + 1] == 1 && map[x, y - 1] == 1)
                    {
                        map[x, y] = 1;
                    }
                }
            }
        }
    }
}

// 0's are backgrounds, 1's and above are tiles.
