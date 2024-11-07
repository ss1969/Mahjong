/*
    首先，每张牌基准分10分，例如5张万子，基准分50分；
    其次，按如下要素从高到低拆解手牌并进行加扣分:

    四张相同的牌，+50分，例如8888万；
    偏张幺九刻子，+25分，即三张相同的1、2、8、9，例如111万；
    中张刻子，+18分，即三张相同的3、4、5、6、7，例如333万；
    顺子，+10分，例如456万；
    偏张幺九对子，+6分，即1、2、8、9对子，例如22万、99万；
    中张对子，+5分，即3、4、5、6、7对子，例如66万；
    两面搭子，+4分，例如23万、56万；
    三张连坎，+3分，例如 246万；
    中张坎张，+2分，即35、46、57，例如46万；
    偏张坎张，+1分，即13、79、24、68，例如24万；
    单张中张，0分，即3、4、5、6、7单张，例如单张5万；
    边张，-1分，例如12万；
    单张偏张-1分，即2、8单张，例如单张2万；
    单张幺九，-2分，即1、2、8、9单张，例如单张1万。
 */

using System.Diagnostics;

namespace Algorithm;

public static class ValueCalc
{
    #region 多个计算不同组合的小函数
    delegate int CALC_BLOCK(ref string tileInString, bool trace = false);

    // 测试是否包含对应pattern，如果包含则算分
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

    // 计算牌价
    public static int CalculateValue(this MahjongSet set, bool trace = false)
    {
        if (set.Count == 0)
            return 0;

        if (set.CountType() != 1)
        {
            throw new InvalidDataException("Tile pack has more than 1 type");
        }

        var sortedNumbers = set.Tiles.Select(tile => tile.NumberSimple).OrderBy(number => number);
        var tileInString = string.Join("", sortedNumbers); // 从0x11 -> 1, 0x21 -> 2
        return CalculateValue(tileInString, trace);
    }

    // 计算牌价
    public static int CalculateValue(string tileInString, bool trace = false)
    {
        // 定义可能影响得分的特定执行顺序
        List<CALC_BLOCK> order1 = [CalcAAA, CalcABC, CalcAA, CalcBC, CalcACE, CalcBD, CalcAC, CalcAB, CalcA]; // 正常顺序
        List<CALC_BLOCK> order2 = [CalcAAA, CalcAA, CalcABC, CalcBC, CalcACE, CalcBD, CalcAC, CalcAB, CalcA]; // 先对后顺
        List<CALC_BLOCK> order3 = [CalcAAA, CalcABC, CalcAA, CalcACE, CalcBD, CalcAC, CalcBC, CalcAB, CalcA]; // 先卡后连子
        List<CALC_BLOCK> order4 = [CalcAAA, CalcAA, CalcABC, CalcACE, CalcBD, CalcAC, CalcBC, CalcAB, CalcA]; // 先对后顺+先卡后连子

        List<List<CALC_BLOCK>> orders = [order1, order2, order3, order4];

        // 计算每种特定顺序的得分
        int score = 0;
        foreach (var o in orders)
        {
            if (trace)
                Trace.WriteLine("Start Calculate : " + tileInString);
            score = Math.Max(score, CalculateValueByOrder(tileInString, o, trace));
        }

        // 返回最高得分
        return score;
    }

    // 按顺序调用上面的小计分方法算分
    private static int CalculateValueByOrder(string tileInString, List<CALC_BLOCK> order, bool trace)
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
