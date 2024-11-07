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
    ALL19 = 8,
}

public static class WinCalc
{
    // 是否打缺
    public static bool IsQUE( this List<MahjongTile> tiles ) => tiles.CountType() <= 2;

    // 获取所有的可能将，不重复。只返回单张。
    private static List<MahjongTile> GetJiangs(this List<MahjongTile> tiles)
        => tiles.GroupBy(t => t.Number)
                .Where(g => g.Count() > 1)
                .SelectMany(g => g)
                .Distinct()
                .ToList();

    private static bool CanSplitIntoTriples( this List<MahjongTile> tiles, out bool allTriple3, out bool is19)
    {
        allTriple3 = true;
        is19 = true;

        // 缺门按pass计算
        if (tiles.Count == 0) return true;  

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
                    if(i != 1 && i != 9) 
                        is19 = false;
                }
                // 尝试形成顺子
                else if ( i <= 7 && counts[i] > 0 && counts[i + 1] > 0 && counts[i + 2] > 0 )
                {
                    counts[i]--;
                    counts[i + 1]--;
                    counts[i + 2]--;
                    if (i != 1 && i != 7) 
                        is19 = false;  
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
        if(tiles.CountOpenTile() != 0)
            return false;

        for (int i = 0; i < tiles.Count; i += 2 )
        {
            if ( tiles[i].Number != tiles[i + 1].Number )
                return false;
        }

        return true;
    }

    // 判断是否平胡
    private static bool IsNormalWin( this List<MahjongTile> tiles, out bool isAllPairs, out bool is19 )
    {
        isAllPairs = false;
        is19 = false;
        
        // 取暗牌，张数计算，因为明牌肯定是碰杠出来的
        var hidden = tiles.GetHiddenTiles();
        if ( hidden.Count != 2 && hidden.Count != 5 && hidden.Count != 8 && hidden.Count != 11 && hidden.Count != 14)
        {
            return false;
        }

        // 2张一样直接true
        if (hidden.Count == 2 && hidden[0].Equals( hidden[1] ))
        {
            isAllPairs = true;
            is19 = (hidden[0].NumberSimple == 1 || hidden[0].NumberSimple == 9);
            return true;
        }

        // 找将
        var jiangs = hidden.GetJiangs();
#if DEBUG
        Trace.WriteLine("可能的将：" + jiangs!.Name());
#endif

        // 去掉将的牌，进行拆解
        foreach (var jiang in jiangs)
        {
            var tilesCopy = tiles.ToList();
            if (!tilesCopy.Remove(jiang)) return false;
            if (!tilesCopy.Remove(jiang)) return false; // 将牌是2个一样的
#if DEBUG
            Trace.Write( $"开始测试将牌 {jiang.Name}：{tilesCopy.Name}");
#endif
            if ( tilesCopy.GetTilesByType(TILE_TYPE.Tong).CanSplitIntoTriples(out bool isAllPairs1, out bool is191)
                && tilesCopy.GetTilesByType(TILE_TYPE.Tiao).CanSplitIntoTriples(out bool isAllPairs2, out bool is192)
                && tilesCopy.GetTilesByType(TILE_TYPE.Wan).CanSplitIntoTriples(out bool isAllPairs3, out bool is193))
            {
#if DEBUG
                Trace.WriteLine( $" 可以完全拆分 ");
#endif
                isAllPairs = isAllPairs1 && isAllPairs2 && isAllPairs3;
                is19 = (jiang.NumberSimple == 1 || jiang.NumberSimple == 9) && is191 && is192 && is193;
                return true;
            }
            else
            {
#if DEBUG
                Trace.WriteLine( $" 拆分失败 " );
#endif
            }
        }

        return false;
    }

    public static bool Calculate(this List<MahjongTile> all, out int score, out string detail) // 是否胡牌计算
    {
        bool isDADUI = false;
        bool isQING = false;
        bool is7 = false;
        bool is19 = false;

        // 是否打缺
        if ( !all.IsQUE() )
        {
            score = 0;
            detail = "没有打缺";
            return false;
        }

        // 判断是否清一色
        if ( all.CountType() == 1)
        {
            isQING = true;
        }

        // 判断7对
        if ( all.Is7Pairs() )
        {
#if DEBUG
#endif
            Trace.WriteLine("暗骑对");
            is7 = true;
            goto COUNT;
        }

        // 是否平胡
        if( all.IsNormalWin(out isDADUI, out is19))
        {
            goto COUNT;
        }

        // 失败
        score = 0;
        detail = "没有胡牌";
        return false;

        // 算分
        COUNT:
        score = (int) ScoreType.NORMAL;
        int dragonCount = all.CountSame4();
        for (int i = 0; i < dragonCount; i++) { 
            score *= 2;
        }

        if (is7) score *= (int)ScoreType.SEVEN_PAIR;
        if (isDADUI) score *= (int)ScoreType.DA_DUI;
        if (isDADUI && all.GetHiddenTiles().Count == 2) score *= (int)ScoreType.SINGLE_FISHING; 
        if (isQING) score *= (int)ScoreType.QING_YISE;
        if (is19) score *= (int)ScoreType.ALL19;

        var sevenStr = is7 ? "暗七对" : "";
        var qingStr = isQING ? "清一色" : "";
        var duiStr = isDADUI ? "大对" : "";
        var one9Str = is19 ? "全带幺" : "";
        detail = $"{sevenStr}{qingStr}{duiStr}{one9Str}";
        detail = detail == "" ? "素番" : detail;
        detail = $"{detail},杠{dragonCount}个";

#if DEBUG
#endif
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
#if DEBUG
#endif
        Trace.WriteLine($"开始计算听牌 {tilesCopy.Name}");

        // 计算
        var connected = tilesCopy.GetConnectedTiles();
        foreach ( var t in connected )
        {
            tilesCopy.Add( t );
            tilesCopy.Sort(new MahjongTileComparer());
            MahjongTileHelper.Sort(tilesCopy);
#if DEBUG
#endif
            Trace.WriteLine($"加入 {t.Name} 计算：");
            if ( tilesCopy.Calculate( out _, out _ ) ) { canWin.Add( t ); }
            tilesCopy.Remove( t );
        }

#if DEBUG
#endif
        Trace.WriteLine( "听牌：" + canWin.Name );

        // 计算剩余牌数
        if(canWin.Count > 0 )
        {
            canWinCount = canWin.Count * 4;
            canWinCount -= tiles.Count( t => canWin.Contains( t ) );
        }

        return canWin;
    }


}
