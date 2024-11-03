
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

#if true
using System.Diagnostics;
using Algorithm;

namespace TestBench;
internal class Program
{

    static void TestScore( string tiles, int standardScore, bool trace = false )
    {
        int s = ValueCalc.Calculate( tiles, trace );
        string r = s == standardScore ? "PASS" : $"FAIL, GET:{s} STANDARD:{standardScore}";
        Console.WriteLine( $"{tiles} : {s} {r}" );
    }

    static void TestWin( List<MahjongTile> tiles, bool standard )
    {
        var isWin = WinCalc.Calculate( tiles, out int score, out string detail );
        string r = (isWin == standard ? "PASS" : "FAIL") + $", Standard: {standard}, Score:{score}, {detail}";
        Console.WriteLine( $"{tiles.Info()} : {r}" );
    }


    static void Main( string[] args )
    {
        TestScore( "2299", 52 );
        TestScore( "2277", 51 );
        TestScore( "4477", 50 );
        TestScore( "3678", 50 );
        TestScore( "2678", 49 );
        TestScore( "1678", 48 );

        TestScore( "2399", 50 );
        TestScore( "3599", 48 );
        TestScore( "1399", 47 );
        TestScore( "1299", 45 );

        TestScore( "2377", 49 );
        TestScore( "3577", 47 );
        TestScore( "1377", 46 );
        TestScore( "1277", 44 );

        TestScore( "2367", 48 );
        TestScore( "2357", 46 );
        TestScore( "2379", 45 );
        TestScore( "2389", 43 );

        TestScore( "3568", 43 );
        TestScore( "3589", 41 );
        TestScore( "1368", 42 );
        TestScore( "1389", 40 );
        TestScore( "1289", 38 );

        TestScore( "3579", 43 );
        TestScore( "1123", 50 );
        TestScore( "1346", 43 );

        TestScore( "13579", 54 );

        TestScore( "455667", 80, true );
        TestScore( "45567", 64, true );
        TestScore( "12234", 61, true );

        TestWin([
                new(0x11),new(0x11),new(0x12),new(0x12),
                new(0x13),new(0x13),new(0x21),new(0x21),
                new(0x22),new(0x22),new(0x25),new(0x25),
                new(0x26),new(0x26)],
                true); // 暗七对
        TestWin([
                new(0x11),new(0x11),new(0x12),new(0x12),
                new(0x13),new(0x13),new(0x21),new(0x21),
                new(0x21),new(0x21),new(0x25),new(0x25),
                new(0x26),new(0x26)],
                true); // 龙七对
        TestWin([
                new(0x11),new(0x11),new(0x12),new(0x12),
                new(0x13),new(0x13),new(0x21),new(0x21),
                new(0x21),new(0x21),new(0x25),new(0x25),
                new(0x25),new(0x25)],
                true); // 双龙七对
        //TestWin( [
        //        new(0x11),new(0x11),new(0x12),new(0x12),
        //        new(0x13),new(0x13),new(0x21),new(0x21),
        //        new(0x11),new(0x11),new(0x24),new(0x25),
        //        new(0x26),new(0x26)],
        //        false );

        TestWin([
                new(0x11),new(0x12),new(0x13),new(0x12),
                new(0x13),new(0x14),new(0x21),new(0x21),
                new(0x21),new(0x21),new(0x22),new(0x23),
                new(0x26),new(0x26)],
                true);

        TestWin([
                new(0x11),new(0x11),new(0x11),new(0x12),
                new(0x12),new(0x12),new(0x21),new(0x21),
                new(0x21),new(0x22),new(0x22),new(0x22),
                new(0x26),new(0x26)],
        true);

    }
}
#endif

#if false
using System;
using System.Collections.Generic;

static class MahjongSolver
{
    public static Tuple<int, List<List<int>>> GetBestCombination( List<int> tiles )
    {
        return GetBestCombination( tiles, new Dictionary<string, Tuple<int, List<List<int>>>>() );
    }

    private static Tuple<int, List<List<int>>> GetBestCombination( List<int> tiles, Dictionary<string, Tuple<int, List<List<int>>>> memo )
    {
        if ( tiles.Count == 0 )
        {
            return Tuple.Create( 0, new List<List<int>>() );
        }

        string key = string.Join( ",", tiles );
        if ( memo.ContainsKey( key ) )
        {
            return memo[key];
        }

        int bestScore = 0;
        List<List<int>> bestCombination = new List<List<int>>();

        for ( int i = 0; i < tiles.Count; i++ )
        {
            // 组合: 111
            if ( i + 2 < tiles.Count && tiles[i] == tiles[i + 1] && tiles[i] == tiles[i + 2] )
            {
                var newCombination = new List<int> { tiles[i], tiles[i + 1], tiles[i + 2] };
                var remainingTiles = new List<int>( tiles );
                remainingTiles.RemoveRange( i, 3 );
                var result = GetBestCombination( remainingTiles, memo );
                int score = CalculateScore( newCombination ) + result.Item1;
                if ( score > bestScore )
                {
                    bestScore = score;
                    bestCombination = new List<List<int>>( result.Item2 );
                    bestCombination.Insert( 0, newCombination );
                }
            }

            // 组合: 123
            if ( i + 2 < tiles.Count && tiles.Contains( tiles[i] + 1 ) && tiles.Contains( tiles[i] + 2 ) )
            {
                var newCombination = new List<int> { tiles[i], tiles[i] + 1, tiles[i] + 2 };
                var remainingTiles = new List<int>( tiles );
                remainingTiles.Remove( tiles[i] );
                remainingTiles.Remove( tiles[i] + 1 );
                remainingTiles.Remove( tiles[i] + 2 );
                var result = GetBestCombination( remainingTiles, memo );
                int score = CalculateScore( newCombination ) + result.Item1;
                if ( score > bestScore )
                {
                    bestScore = score;
                    bestCombination = new List<List<int>>( result.Item2 );
                    bestCombination.Insert( 0, newCombination );
                }
            }

            // 组合: 11
            if ( i + 1 < tiles.Count && tiles[i] == tiles[i + 1] )
            {
                var newCombination = new List<int> { tiles[i], tiles[i + 1] };
                var remainingTiles = new List<int>( tiles );
                remainingTiles.RemoveRange( i, 2 );
                var result = GetBestCombination( remainingTiles, memo );
                int score = CalculateScore( newCombination ) + result.Item1;
                if ( score > bestScore )
                {
                    bestScore = score;
                    bestCombination = new List<List<int>>( result.Item2 );
                    bestCombination.Insert( 0, newCombination );
                }
            }

            // 组合: 12
            if ( i + 1 < tiles.Count && tiles[i] + 1 == tiles[i + 1] )
            {
                var newCombination = new List<int> { tiles[i], tiles[i + 1] };
                var remainingTiles = new List<int>( tiles );
                remainingTiles.RemoveRange( i, 2 );
                var result = GetBestCombination( remainingTiles, memo );
                int score = CalculateScore( newCombination ) + result.Item1;
                if ( score > bestScore )
                {
                    bestScore = score;
                    bestCombination = new List<List<int>>( result.Item2 );
                    bestCombination.Insert( 0, newCombination );
                }
            }

            // 组合: 13
            if ( i + 1 < tiles.Count && tiles[i] + 2 == tiles[i + 1] )
            {
                var newCombination = new List<int> { tiles[i], tiles[i + 1] };
                var remainingTiles = new List<int>( tiles );
                remainingTiles.RemoveRange( i, 2 );
                var result = GetBestCombination( remainingTiles, memo );
                int score = CalculateScore( newCombination ) + result.Item1;
                if ( score > bestScore )
                {
                    bestScore = score;
                    bestCombination = new List<List<int>>( result.Item2 );
                    bestCombination.Insert( 0, newCombination );
                }
            }

            // 组合: 135
            if ( i + 2 < tiles.Count && tiles.Contains( tiles[i] + 2 ) && tiles.Contains( tiles[i] + 4 ) )
            {
                var newCombination = new List<int> { tiles[i], tiles[i] + 2, tiles[i] + 4 };
                var remainingTiles = new List<int>( tiles );
                remainingTiles.Remove( tiles[i] );
                remainingTiles.Remove( tiles[i] + 2 );
                remainingTiles.Remove( tiles[i] + 4 );
                var result = GetBestCombination( remainingTiles, memo );
                int score = CalculateScore( newCombination ) + result.Item1;
                if ( score > bestScore )
                {
                    bestScore = score;
                    bestCombination = new List<List<int>>( result.Item2 );
                    bestCombination.Insert( 0, newCombination );
                }
            }
        }

        memo[key] = Tuple.Create( bestScore, bestCombination );
        return memo[key];
    }

    private static int CalculateScore( List<int> combination )
    {
        // 根据组合的类型返回分值
        if ( combination.Count == 3 && combination[0] == combination[1] && combination[1] == combination[2] )
        {
            return 18; // 111
        }
        if ( combination.Count == 3 && combination[0] + 1 == combination[1] && combination[1] + 1 == combination[2] )
        {
            return 10; // 123
        }
        if ( combination.Count == 2 && combination[0] == combination[1] )
        {
            return 5; // 11
        }
        if ( combination.Count == 2 && combination[0] + 1 == combination[1] )
        {
            return 4; // 12
        }
        if ( combination.Count == 2 && combination[0] + 2 == combination[1] )
        {
            return 2; // 13
        }
        if ( combination.Count == 3 && combination[0] + 2 == combination[1] && combination[1] + 2 == combination[2] )
        {
            return 3; // 135
        }
        return 0;
    }
}

class Program
{
    static void Main()
    {
        List<int> tiles = new List<int> { 4, 5, 5, 6, 7 };
        var result = MahjongSolver.GetBestCombination( tiles );
        Console.WriteLine( "BestScore: " + result.Item1 );
        Console.WriteLine( "BestCombine: " );
        foreach ( var combination in result.Item2 )
        {
            Console.WriteLine( string.Join( ",", combination ) );
        }
    }
}
#endif