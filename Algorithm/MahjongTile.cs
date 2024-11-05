namespace Algorithm;

public class MahjongTileComparer : IComparer<MahjongTile>
{
    public int Compare( MahjongTile? x, MahjongTile? y )
    {
        if ( x == null || y == null )
        {
            throw new ArgumentNullException( "Tiles to compare cannot be null." );
        }

        //// compare type
        //if ( ( x.Number & 0xF0 ) != ( y.Number & 0xF0 ) )
        //{
        //    throw new ArgumentNullException( $"2 Tiles not same type {x.Number} {y.Number}." );
        //}

        //// 如果类型相同，再比较号码
        return x.Number.CompareTo( y.Number );
    }
}

public enum TileType
{
    Tong = 0x10,
    Tiao = 0x20,
    Wan = 0x40,
}

public static class MahjongTileHelper
{
    // 打印输出
    public static string Name(this List<MahjongTile> tiles)
        //=> $"[{ string.Join(",", tiles.Select(t => $"{t.Number:X2}")) }]";
        => string.Join(",", tiles.Select(t=>t.Name()));

    // 排序
    public static void Sort(this List<MahjongTile> tiles)
    {
        if (tiles.Count == 0)
        {
            throw new ArgumentException("Tile list empty.");
        }

        tiles.Sort(new MahjongTileComparer());
    }

    // 是否某种花色
    public static bool IsType( this MahjongTile tile, TileType type ) => ( tile.Number & (int)type ) > 0;

    // 同花色
    public static bool IsSameType( this MahjongTile tile1, MahjongTile tile2 ) 
        => Math.Abs(tile1.Number - tile2.Number) < 10;

    // 统计花色
    public static int TypeCount( this List<MahjongTile> tiles )
    {
        int sum = 0;
        foreach ( var t in tiles )
        {
            int type = ( t.Number >> 4 ) & 0x0F; // 取出 bit 7:4
            sum |= type; // 将类型按位或到 sum 中
        }

        return sum switch {
            0x1 => 1, 0x2 => 1, 0x4 => 1, 
            0x3 => 2, 0x5 => 2, 0x6 => 2, 
            0x7 => 3,
            _ => 0,
        };
    }

    // 获取某具体牌数量
    public static int GetSpecificCount(this List<MahjongTile> tiles, int tileNumber)
        => tiles.Where(t => t.Number == tileNumber).Count();


    // 获取明牌数量
    public static int OpenCount(this List<MahjongTile> tiles )
        => tiles.Where(t => t.Open).Count();

    // 统计4张数量
    private static int SetsCount(this List<MahjongTile> tiles, int n) 
        => tiles.GroupBy(t => t.Number)
                .Count(g => g.Count() == n);

    public static int FourSetsCount(this List<MahjongTile> tiles) 
        => SetsCount(tiles, 4);
    public static int ThreeSetsCount(this List<MahjongTile> tiles) 
        => SetsCount(tiles, 3);

    // 获取明牌暗牌
    public static List<MahjongTile> GetOpenTiles( this List<MahjongTile> tiles ) 
        => tiles.Where( t => t.Open ).ToList();
    public static List<MahjongTile> GetHiddenTiles( this List<MahjongTile> tiles ) 
        => tiles.Where( t => !t.Open ).ToList();

    // 获取关联牌（相差不超过1）
    public static List<MahjongTile> GetConnectedTiles(this List<MahjongTile> tiles)
    {
        bool[] isPresent = new bool[0x4A]; // 用于标记数字 0x11 到 0x49 是否在结果中

        foreach (var t in tiles.Distinct())
        {
            isPresent[t.Number] = true; // 标记数字本身
            if (t.NumberSimple != 1) isPresent[t.Number - 1] = true; // 标记 num - 1
            if (t.NumberSimple != 9) isPresent[t.Number + 1] = true; // 标记 num + 1
        }

        // 根据 isPresent 数组生成结果列表
        List<MahjongTile> result = [];
        for (int i = 1; i < 0x4A; i++)
        {
            if (isPresent[i])
                result.Add(new MahjongTile(i));
        }

        return result;
    }
}

public class MahjongTile(int number)
{
    public int Index { get; set; } = -1; //位于一副牌中的位置

    private int number = number;

    // 完整值
    public int Number { 
        get => number; 
        set {
            if ( ( value > 0x10 && value <= 0x19 ) || ( value > 0x20 && value <= 0x29 ) || ( value > 0x40 && value <= 0x49 ) )
                number = value;
            else
                throw new InvalidDataException($"Invalid tile number set {number}");
        } 
    }

    // 简易值，只有1-9
    public int NumberSimple => number & 0xF;

    // 牌名
    public string Name()
    {
        string name = number switch
        {
            _ when number < 0x1A => "筒",
            _ when number > 0x40 => "万",
            _ => "条",
        };
        return ( number & 0xF ).ToString() + name;
    }

    // 是否翻开的牌
    public bool Open { get; set; } = false;

    // Override
    public override string ToString() => $"[{number:X2}]";

    public override bool Equals(object? obj)
    {
        if ( obj == null ) return false;
        return number == (obj as MahjongTile)!.number;
    }

    public override int GetHashCode() => Number.GetHashCode();

}
