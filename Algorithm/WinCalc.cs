using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace Algorithm;

public enum ScoreType
{
    NORMAL = 1,
    DA_DUI = 2,
    SINGLE_FISHING = 2, //未用
    QING_YISE = 4,
    SEVEN_PAIR = 4,
    ONE_NINE = 8,   //未用
}

public static class WinCalc
{
    // 是否打缺
    public static bool IsQUE( this List<MahjongTile> tiles ) => tiles.TypeCount() <= 2;

    // 获取所有的可能将，不重复。只返回单张。
    private static List<MahjongTile> GetJiangs(this List<MahjongTile> tiles)
        => tiles.GroupBy(t => t.Number)
                .Where(g => g.Count() > 1)
                .SelectMany(g => g)
                .Distinct()
                .ToList();


    private static bool CanSplitIntoTriples( this List<MahjongTile> tiles, out bool allTriple3 )
    {
        allTriple3 = false;

        // 确保能分成三组
        if ( tiles.Count % 3 != 0 ) return false;

        // 统计每个数字出现的次数
        var counts = new int[10];
        foreach ( var t in tiles )
        {
            counts[t.NumberSimple]++;
        }

        int tripleCount = 0;
        // 按顺序检查数字
        for ( int i = 1; i <= 9; i++ )
        {
            while ( counts[i] > 0 )
            {
                // 尝试形成三组相同数字
                if ( counts[i] >= 3 )
                {
                    counts[i] -= 3;
                    tripleCount += 3;
                }
                // 尝试形成顺子
                else if ( i <= 7 && counts[i] > 0 && counts[i + 1] > 0 && counts[i + 2] > 0 )
                {
                    counts[i]--;
                    counts[i + 1]--;
                    counts[i + 2]--;
                }
                else
                {
                    return false;
                }
            }
        }

        allTriple3 = tiles.Count == tripleCount;
        return true;
    }

    // 判断是否暗7对
    private static bool Is7Pairs( this List<MahjongTile> tiles )
    {
        if(tiles.Count != 14)
            return false;
        if(tiles.OpenCount() != 0)
            return false;

        for (int i = 0; i < tiles.Count; i += 2 )
        {
            if ( tiles[i].Number != tiles[i + 1].Number )
                return false;
        }

        return true;
    }

    // 判断是否平胡
    private static bool IsNormalWin( this List<MahjongTile> tiles, out bool isAllPairs )
    {
        isAllPairs = false;
        
        // 取暗牌，张数计算，因为明牌肯定是碰杠出来的
        var hidden = tiles.GetHiddenTiles();
        if ( hidden.Count != 2 && hidden.Count != 5 && hidden.Count != 8 && hidden.Count != 11 && hidden.Count != 14 )
            return false;

        // 2张一样直接true
        if (hidden.Count == 2 && hidden[0].Equals( hidden[1] ) )
            return true;

        // 找将
        var jiangs = hidden.GetJiangs();
        //Console.WriteLine("可能的将：" + jiangs.Name());

        // 去掉将的牌，进行拆解
        foreach (var jiang in jiangs)
        {
            var tilesCopy = tiles.ToList();
            if (!tilesCopy.Remove(jiang)) return false;
            if (!tilesCopy.Remove(jiang)) return false; // 将牌是2个一样的
            Console.Write( $"开始测试将牌 {jiang.Name()}：{tilesCopy.Name()}");

            if ( tilesCopy.CanSplitIntoTriples(out isAllPairs))
            {
                Console.WriteLine( $" 可以完全拆分 ");
                return true;
            }
            else
            {
                Console.WriteLine( $" 拆分失败 " );
            }
        }

        return false;
    }

    public static bool Calculate(this List<MahjongTile> all, out int score, out string detail) // 是否胡牌计算
    {
        bool isDADUI = false;
        bool isQING = false;
        bool is7 = false;
        bool isNORMAL = false;
        bool is19 = false;

        // 是否打缺
        if ( !all.IsQUE() )
        {
            score = 0;
            detail = "没有打缺";
            return false;
        }

        // 判断是否清一色
        if ( all.TypeCount() == 1)
        {
            isQING = true;
        }

        // 判断7对
        if ( all.Is7Pairs() )
        {
            is7 = true;
            goto COUNT;
        }

        // 是否平胡
        if( all.IsNormalWin(out isDADUI))
        {
            isNORMAL = true;
            goto COUNT;
        }

        // 是否带幺
        score = 0;
        detail = "没有胡牌";
        return false;

        // 算分
        COUNT:
        score = (int) ScoreType.NORMAL;
        int dragonCount = all.FourSetsCount();
        for (int i = 0; i < dragonCount; i++) { 
            score *= 2;
        }

        if (is7) score *= (int)ScoreType.SEVEN_PAIR;
        if (isDADUI) score *= (int)ScoreType.DA_DUI;
        if (isQING) score *= (int)ScoreType.QING_YISE;

        var suStr = isNORMAL ? "素番" : "";
        var sevenStr = is7 ? "暗七对" : "";
        var duiStr = isDADUI ? ",大对" : "";
        var qingStr = isQING ? ",清一色" : "";
        detail = $"{suStr}{sevenStr}{qingStr}{duiStr},杠{dragonCount}个";

        Trace.WriteLine(detail);
        return true;
    }

    public static List<MahjongTile> ListenTiles(this List<MahjongTile> tiles, out int canWinCount )
    {
        List<MahjongTile> canWin = []; // 花色
        canWinCount = 0; // 张数

        if ( !tiles.IsQUE() )
            return canWin;

        // 取暗牌，张数计算，因为明牌肯定是碰杠出来的
        var hidden = tiles.GetHiddenTiles();
        if ( hidden.Count != 1 && hidden.Count != 4 && hidden.Count != 7 && hidden.Count != 10 && hidden.Count != 13 )
            return canWin;

        // 创建副本用于计算，否则会影响
        List<MahjongTile> tilesCopy = new( tiles );

        // 计算
        var connected = tilesCopy.GetConnectedTiles();
        foreach ( var t in connected )
        {
            tilesCopy.Add( t );
            if ( tilesCopy.Calculate( out _, out _ ) ) { canWin.Add( t ); }
            tilesCopy.Remove( t );
        }
        Trace.WriteLine( "can win " + canWin.Name() );

        // 计算剩余牌数
        if(canWin.Count > 0 )
        {
            canWinCount = canWin.Count * 4;
            canWinCount -= tiles.Count( t => canWin.Contains( t ) );
        }

        return canWin;
    }


}
