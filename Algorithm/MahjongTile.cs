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
    // 是否花色
    public static bool IsType( this MahjongTile tile, TileType type ) => ( tile.Number & (int)type ) > 0;

    // 同花色
    public static bool IsSameType( this MahjongTile tile1, MahjongTile tile2 ) => Math.Abs(tile1.Number - tile2.Number) < 10;

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
            0x1 => 1,
            0x2 => 1,
            0x4 => 1,
            0x3 => 2,
            0x5 => 2,
            0x6 => 2,
            0x7 => 3,
            _ => 0,
        };
    }
    
    // 获取明牌数量
    public static int OpenCount(this List<MahjongTile> tiles ) => tiles.Where(t => t.Open).Count();

    public static List<MahjongTile> OpenTiles( this List<MahjongTile> tiles ) => tiles.Where( t => t.Open ).ToList();
    public static List<MahjongTile> HiddenTiles( this List<MahjongTile> tiles ) => tiles.Where( t => !t.Open ).ToList();


}

public class MahjongTile(int number)
{
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

    // 是否翻开的牌
    public bool Open { get; set; } = false;

}
