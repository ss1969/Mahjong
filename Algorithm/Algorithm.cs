using System.Collections.Generic;
using System.Diagnostics;

namespace Algorithm;


public static class StringHelper
{
    public static bool RemoveCharsIfPossible(ref string source, string toRemove)
    {
        // 将字符串A和B转换为List<char>
        List<char> listA = [.. source];
        List<char> listB = [.. toRemove];

        // 对于B中的每一个字符
        foreach (char charToRemove in listB)
        {
            // 尝试在listA中找到并移除第一个匹配的字符
            if (!listA.Remove(charToRemove))
            {
                // 如果移除失败，不修改原始字符串source
                return false;
            }
        }

        // 如果所有字符顺利移除，则source修改为移除后的集合转化后的新字符串
        source = new string(listA.ToArray());
        return true;
    }

    public static bool HasDuplicate(this string str)
    {
        var seen = new HashSet<char>();
        foreach (char c in str)
        {
            if (!seen.Add(c)) // 如果添加失败，说明c已经在seen中了
            {
                return true; // 有重复值
            }
        }
        return false; // 没有重复值
    }
}

public static class MahjongHelper
{
    /// <summary>
    /// 测试是否包含对应pattern，如果包含则算分
    /// </summary>
    /// <param name="baseString"></param>
    /// <param name="pattern"></param>
    /// <param name="scoreBase"></param>
    /// <returns></returns>
    static int TestPattern(ref string baseString, string[] pattern, int scoreBase)
    {
        int score = 0;
        foreach (var p in pattern)
        {
            if (StringHelper.RemoveCharsIfPossible(ref baseString, p))
                score += scoreBase;
        }
        return score;
    }

    public static void SortTiles(this List<MahjongTile> tiles)
    {
        if (tiles.Count == 0)
        {
            throw new ArgumentException("Tile list empty.");
        }

        tiles.Sort(new MahjongTileComparer());
    }

    #region 多个计算不同组合的小函数
    delegate int CALC_BLOCK(ref string tileInString, bool trace = false);

    private static int CalcAAA(ref string tileInString, bool trace = false)
    {
        int temp;
        temp = TestPattern(ref tileInString, ["1111", "2222", "3333", "4444", "5555", "6666", "7777", "8888", "9999"], 50);
        temp += TestPattern(ref tileInString, ["111", "222", "888", "999"], 25);
        temp += TestPattern(ref tileInString, ["333", "444", "555", "666", "777"], 18);
        if (temp != 0 && trace)
            Trace.WriteLine($"'刻子' 得分 : {temp}");
        return temp;
    }

    private static int CalcABC(ref string tileInString, bool trace = false)
    {
        int temp;
        temp = TestPattern(ref tileInString, ["123", "234", "345", "456", "567", "678", "789"], 10);
        if (temp != 0 && trace) 
            Trace.WriteLine($"'顺子' 得分 : {temp}");
        return temp;
    }

    private static int CalcAA(ref string tileInString, bool trace = false)
    {
        int temp;
        temp = TestPattern(ref tileInString, ["11", "22", "88", "99"], 6);
        temp += TestPattern(ref tileInString, ["33", "44", "55", "66", "77"], 5);
        if (temp != 0 && trace)
            Trace.WriteLine($"'对子' 得分 : {temp}");
        return temp;
    }

    private static int CalcBC(ref string tileInString, bool trace = false)
    {
        int temp;
        temp = TestPattern(ref tileInString, ["23", "34", "45", "56", "67", "78"], 4);
        if (temp != 0 && trace)
            Trace.WriteLine($"'中间连张' 得分 : {temp}");
        return temp;
    }

    private static int CalcACE(ref string tileInString, bool trace = false)
    {
        int temp;
        temp = TestPattern(ref tileInString, ["135", "579", "246", "468", "357"], 3); // sequence here.
        if (temp != 0 && trace)
            Trace.WriteLine($"'三连卡张' 得分 : {temp}");
        return temp;
    }

    private static int CalcBD(ref string tileInString, bool trace = false)
    {
        int temp;
        temp = TestPattern(ref tileInString, ["35", "46", "57"], 2);
        if (temp != 0 && trace)
            Trace.WriteLine($"'中间卡张' 得分 : {temp}");
        return temp;
    }

    private static int CalcAC(ref string tileInString, bool trace = false)
    {
        int temp;
        temp = TestPattern(ref tileInString, ["13", "24", "68", "79"], 1);
        if (temp != 0 && trace)
            Trace.WriteLine($"'边张卡张' 得分 : {temp}");
        return temp;
    }

    private static int CalcAB(ref string tileInString, bool trace = false)
    {
        int temp;
        temp = TestPattern(ref tileInString, ["12", "89"], -1);
        if (temp != 0 && trace)
            Trace.WriteLine($"'边张' 扣分 : {temp}");
        return temp;
    }

    private static int CalcA(ref string tileInString, bool trace = false)
    {
        if (tileInString.HasDuplicate())
            throw new InvalidDataException($"Have duplicate tile : {tileInString}");

        int temp;
        temp = TestPattern(ref tileInString, ["3", "4", "5", "6", "7"], 0);
        temp += TestPattern(ref tileInString, ["2", "8"], -1);
        temp += TestPattern(ref tileInString, ["1", "9"], -2);
        if (temp != 0 && trace)
            Trace.WriteLine($"'单张' 扣分 : {temp}");
        return temp;
    }
    #endregion

    public static int CalculateScore(this List<MahjongTile> tiles, bool trace = false)
    {
        if ( tiles.TypeCount() != 1 )
        {
            throw new InvalidDataException("Tile pack has more than 1 type");
        }

        var sortedNumbers = tiles.Select(tile => tile.NumberSimple).OrderBy(number => number);
        var tileInString = string.Join("", sortedNumbers); // 从0x11 -> 1, 0x21 -> 2
        return CalculateScore(tileInString, trace);
    }

    public static int CalculateScore(string tileInString, bool trace = false)
    {
        // 定义可能影响得分的特定执行顺序
        List<CALC_BLOCK> order1 = [CalcAAA, CalcABC, CalcAA, CalcBC, CalcACE, CalcBD, CalcAC, CalcAB, CalcA]; // 正常顺序
        List<CALC_BLOCK> order2 = [CalcAAA, CalcAA, CalcABC, CalcBC, CalcACE, CalcBD, CalcAC, CalcAB, CalcA]; // 先对后顺
        List<CALC_BLOCK> order3 = [CalcAAA, CalcABC, CalcAA, CalcACE, CalcBD, CalcAC, CalcBC, CalcAB, CalcA]; // 先卡后连子
        List<CALC_BLOCK> order4 = [CalcAAA, CalcAA, CalcABC, CalcACE, CalcBD, CalcAC, CalcBC, CalcAB, CalcA]; // 先对后顺+先卡后连子

        List<List<CALC_BLOCK>> orders = [order1, order2, order3, order4];

        // 计算每种特定顺序的得分
        int score = 0;
        foreach (var o in orders) {
            if(trace)
                Trace.WriteLine("Start Calculate : " + tileInString);
            score = Math.Max(score, CalculateScoreByOrder(tileInString, o, trace));
        }

        // 返回最高得分
        return score;
    }

    private static int CalculateScoreByOrder(string tileInString, List<CALC_BLOCK> order, bool trace)
    {
        int score = tileInString.Length * 10; // 基准分
        string currentTile = tileInString;

        foreach (var method in order)
        {
            score += method(ref currentTile, trace);
            if (currentTile == "")
                break;
        }

        if (currentTile != "")
        {
            throw new InvalidDataException($"Error: tile left: {currentTile}");
        }

        return score;
    }

}
