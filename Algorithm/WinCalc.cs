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
    SEVEN_PAIR = 4,
    SINGLE_FISHING = 4,
    ONE_NINE = 8,
}

public static class WinCalc
{
    // 是否打缺
    private static bool IsQUE( this List<MahjongTile> tiles ) => tiles.TypeCount() <= 2;


    // 获取所有的可能将，不重复。只返回单张。
    private static List<MahjongTile> GetJiangs(this List<MahjongTile> tiles)
        => tiles.GroupBy(t => t.Number)
                .Where(g => g.Count() > 1)
                .SelectMany(g => g)
                .Distinct()
                .ToList();

    // 是否能全部拆成顺，或一坎牌
    // kiilii 可能存在问题
    private static bool CanSplitIntoTriples(this List<MahjongTile> tiles, out bool allTriple3)
    {
        int tripleCount = 0;

        // 统计每个数字出现的次数
        var frequencyDict = new Dictionary<int, int>();
        foreach (var tile in tiles)
        {
            if (frequencyDict.TryGetValue(tile.Number, out int value))
            {
                frequencyDict[tile.Number] = ++value;
            }
            else
            {
                frequencyDict[tile.Number] = 1;
            }
        }

        // 检查 N、N+1、N+2 的组合
        bool foundSHUN;
        while (true)
        {
            foundSHUN = false;
            foreach (var kvp in frequencyDict)
            {
                int num = kvp.Key;

                if (kvp.Value != 0  
                    && frequencyDict.ContainsKey(num + 1) && frequencyDict[num + 1] > 0
                    && frequencyDict.ContainsKey(num + 2) && frequencyDict[num + 2] > 0)
                {
                    foundSHUN = true;
                    frequencyDict[num] -= 1;
                    frequencyDict[num + 1] -= 1;
                    frequencyDict[num + 2] -= 1;
                }
            }
            if (!foundSHUN) break;
        }

        // 检查 NNN 组合，因为顺子已经移除，每个不同点数顺子最多1次
        foreach (var kvp in frequencyDict)
        {
            int num = kvp.Key;
            if (kvp.Value == 3)
            {
                frequencyDict[num] -= 3;
                tripleCount++;
            }
        }

        // 如果所有数字都可以被分成三个一组,则 frequencyDict 中所有值都应为 0
        bool canSplitIntoTriples = frequencyDict.All(kvp => kvp.Value == 0);

        // 检查是否全是 坎
        allTriple3 = tripleCount == tiles.Count / 3;

        if(canSplitIntoTriples)
            Console.Write($"CanSplitIntoTriples: {tiles.Info()}");
        return canSplitIntoTriples;
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
        // 在暗排里面计算，因为明牌肯定是碰杠出来的
        
        // 找将
        var hiddenTiles = tiles.GetHiddenTiles();
        var jiangs = hiddenTiles.GetJiangs();

        Console.WriteLine("将：" + jiangs.Info());
        // 去掉将的牌，进行拆解
        foreach (var jiang in jiangs)
        {
            var tilesCopy = tiles.ToList();
            if (!tilesCopy.Remove(jiang)) return false;
            if (!tilesCopy.Remove(jiang)) return false; // 将牌是2个一样的

            if (tilesCopy.CanSplitIntoTriples(out isAllPairs))
            {
                Console.WriteLine($"{tilesCopy.Info()} 可以 ");
                return true;
            }
        }
        return false;
    }

    public static bool Calculate(this List<MahjongTile> tiles, out int score, out string detail)
    {
        bool isDADUI = false;
        bool isQING = false;
        bool is7 = false;
        bool isNORMAL = false;
        bool is19 = false;

        // 是否打缺
        if ( !tiles.IsQUE())
        {
            score = 0;
            detail = "没有打缺";
            return false;
        }

        // 判断是否清一色
        if (tiles.TypeCount() == 1)
        {
            isQING = true;
        }

        // 判断7对
        if ( tiles.Is7Pairs() )
        {
            score = (int) ScoreType.SEVEN_PAIR;
            is7 = true;
            goto COUNT;
        }

        // 是否平胡
        if( tiles.IsNormalWin(out isDADUI))
        {
            isNORMAL = true;
            score = (int)ScoreType.NORMAL;
            goto COUNT;
        }

        // 是否带幺
        score = 0;
        detail = "没有胡牌";
        return false;

        // 算分
        COUNT:
        int dragonCount = tiles.FourSetsCount();
        for (int i = 0; i < dragonCount; i++) { 
            score *= 2;
        }

        if (is7) score *= 4;
        if (isDADUI) score *= 2;
        if (isQING) score *= 4;

        var suStr = isNORMAL ? "素番" : "";
        var sevenStr = is7 ? "暗七对" : "";
        var duiStr = isDADUI ? ",大对" : "";
        var qingStr = isQING ? ",清一色" : "";
        detail = $"{suStr}{sevenStr}{qingStr}{duiStr}，杠{dragonCount}个";

        Console.WriteLine(detail);
        return true;
    }

}
