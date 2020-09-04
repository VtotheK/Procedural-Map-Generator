public struct Point    //Store individual map-coordinates by this struct.
{
    private int _x;
    private int _y;
    private int _regionNum;

    public Point(int x, int y, int regionNum)
    {
        _x = x;
        _y = y;
        _regionNum = regionNum;
    }

    public int x { get { return _x; } }
    public int y { get { return _y; } }
    public int regionNum { get { return _regionNum; } }


}
