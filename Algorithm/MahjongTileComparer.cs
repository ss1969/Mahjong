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
