using System.Diagnostics;
using Algorithm;

namespace TestBench;

/// <summary>
/// 深刻领会“尊杠”、“尚攻”、“抢和”、“顺势”四个关键词，确立“平和带杠贪自摸”的王道打法
/// </summary>
/// 
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
internal class Program
{

    static void TestScore(string tiles, int standardScore, bool trace = false)
    {
        int s = MahjongHelper.CalculateScore(tiles, trace);
        string r = s == standardScore ? "PASS" : $"FAIL, GET:{s} STANDARD:{standardScore}";
        Console.WriteLine($"{tiles} : {s} {r}");
    }

    static void Main(string[] args)
    {
        TestScore("2299", 52);
        TestScore("2277", 51);
        TestScore("4477", 50);
        TestScore("3678", 50);
        TestScore("2678", 49);
        TestScore("1678", 48);

        TestScore("2399", 50);
        TestScore("3599", 48);
        TestScore("1399", 47);
        TestScore("1299", 45);
        
        TestScore("2377", 49);
        TestScore("3577", 47);
        TestScore("1377", 46);
        TestScore("1277", 44);
        
        TestScore("2367", 48);
        TestScore("2357", 46);
        TestScore("2379", 45);
        TestScore("2389", 43);

        TestScore("3568", 43);
        TestScore("3589", 41);
        TestScore("1368", 42);
        TestScore("1389", 40);
        TestScore("1289", 38);

        TestScore("3579", 43);
        TestScore("1123", 50);
        TestScore("1346", 43);

        TestScore("13579", 54);

        TestScore("455667", 80, true);
        TestScore("45567", 64, true);
        TestScore("12234", 61, true);
    }
}
