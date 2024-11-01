using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Algorithm;

public enum ScoreType
{
    NORMAL = 1,
    ALL_PAIR = 2,
    UNI_COLOR = 4,
    SEVEN_PAIR = 4,
    SINGLE_FISHING = 4,
    ONE_NINE = 8,


}

public static class WinCalc
{
    // 是否打缺
    private static bool IsQUE( this List<MahjongTile> tiles ) => tiles.TypeCount() <= 2;

    // 判断是否暗7对
    private static bool Is7Pairs( this List<MahjongTile> tiles, out int dragonCount )
    {
        dragonCount = 0;
        if(tiles.Count != 14)
            return false;
        if(tiles.OpenCount() != 0)
            return false;

        int prevNumber = 0;

        for (int i = 0; i < tiles.Count; i += 2 )
        {
            if( tiles[i].Number == prevNumber ) 
                dragonCount++;
            if ( tiles[i].Number != tiles[i + 1].Number )
                return false;
            prevNumber = tiles[i].Number;
        }

        return true;
    }

    // 判断是否平胡
    private static bool IsNormalWin( this List<MahjongTile> tiles )
    {
        return false;
    }

    private static int Count4Same( this List<MahjongTile> tiles )
    {
        return 0;
    }

    public static bool Calculate(List<MahjongTile> tiles, out int score)
    {
        score = 0;
        int multiple = 1;

        // 是否打缺
        if ( !tiles.IsQUE() )
            return false;

        // 判断是否清一色
        if (tiles.TypeCount() == 1 )
            multiple *= 4;

        // 判断7对
        if ( tiles.Is7Pairs(out int dragonCount ) )
        {
            score = (int) ScoreType.SEVEN_PAIR;
            score *= multiple;
            for (int i = 0; i< dragonCount; i++)
                score *= 2;
            return true;
        }

        // 是否平胡
        if( !tiles.IsNormalWin() )
            return false;

        score = (int)ScoreType.NORMAL;

        return false;
    }

}
