using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Algorithm;
using System.Diagnostics;

namespace MJ1;

public partial class MainPageViewModel : ObservableObject
{
    #region O Properties
    [ObservableProperty]
    private string scoreLabel1 = "";
    [ObservableProperty]
    private string scoreLabel2 = "";
    [ObservableProperty]
    private string scoreLabel3 = "";
    [ObservableProperty]
    private string winLabel = "";
    [ObservableProperty]
    private Color scoreLabelColor1 = Colors.White;
    [ObservableProperty]
    private Color scoreLabelColor2 = Colors.White;
    [ObservableProperty]
    private Color scoreLabelColor3 = Colors.White;
    [ObservableProperty]
    private Color winLabelColor = Colors.White;

    [ObservableProperty]
    private int wanCount = 4;
    [ObservableProperty]
    private int tongCount = 4;
    [ObservableProperty]
    private int tiaoCount = 5;
    [ObservableProperty]
    private int totalTiles = 13;

    [ObservableProperty]
    private string canWinTiles = "听牌：";


    partial void OnWanCountChanged(int value) => TotalTiles = WanCount + TongCount + TiaoCount;
    partial void OnTongCountChanged(int value) => TotalTiles = WanCount + TongCount + TiaoCount;
    partial void OnTiaoCountChanged(int value) => TotalTiles = WanCount + TongCount + TiaoCount;

    #endregion

    #region Variables
    private MahjongDeck _deck;
    private MahjongHand _handTile;
    public Grid TilesOne { get; set; } = [];    // 第一行
    public Grid TilesTwo { get; set; } = [];    // 第二行

    private static readonly int SELECT_MOVE_UP_POINT = 50;
    #endregion

    public MainPageViewModel()
    {
        _deck = new();
        _handTile = new();
    }

    public static void FindMaxMin(int num1, int num2, int num3, out int maxValue, out int minValue)
    {
        // 初始化最大值和最小值为第一个数字
        maxValue = num1;
        minValue = num1;

        // 比较第二个数字
        if (num2 > maxValue)
            maxValue = num2;
        else if (num2 < minValue)
            minValue = num2;

        // 比较第三个数字
        if (num3 > maxValue)
            maxValue = num3;
        else if (num3 < minValue)
            minValue = num3;
    }

    // 分别计算三门牌的分值
    private void CalculateTileValue(MahjongHand hand)
    {
        var s1 = hand.GetScoreByType(TILE_TYPE.Tong);
        var s2 = hand.GetScoreByType(TILE_TYPE.Tiao);
        var s3 = hand.GetScoreByType(TILE_TYPE.Wan);
        FindMaxMin(s1, s2, s3, out int max, out int min);

        ScoreLabel1 = $"筒: {s1}";
        ScoreLabel2 = $"条: {s2}";
        ScoreLabel3 = $"万: {s3}";

        if (s1 == max) ScoreLabelColor1 = Colors.Red;
        else if (s1 == min) ScoreLabelColor1 = Colors.Green;
        else ScoreLabelColor1 = Colors.White;
        if (s2 == max) ScoreLabelColor2 = Colors.Red;
        else if (s2 == min) ScoreLabelColor2 = Colors.Green;
        else ScoreLabelColor2 = Colors.White;
        if (s3 == max) ScoreLabelColor3 = Colors.Red;
        else if (s3 == min) ScoreLabelColor3 = Colors.Green;
        else ScoreLabelColor3 = Colors.White;
    }

    // 计算胡牌，不管张数
    private void CalculateTilesCanWin(MahjongHand hand) 
    {
        var win = hand.Set.CalculateWin(out int score, out string detail);
        WinLabel = win ? $"WIN {score}分 {detail}" : $"NO {detail}";
        WinLabelColor = win ? Colors.Red : Colors.Green;
    }

    // 以手牌中的 Status == Hidden 的计算听牌，
    private void CalculateTilesListen(MahjongHand hand)
    {
        if(!hand.Set.CountCanListen())
        {
            CanWinTiles = "张数无法听牌";
            return;
        }
        var canWin = hand.Set.CalculateListen(out int canWinCount);

        // 输出
        if (canWin.Count > 0)
            CanWinTiles = $"听牌：{canWin.Name}, 共 {canWin.Count} 类 {canWinCount} 张";
        else
            CanWinTiles = "无法听牌";
    }

    // 以 Status == Hidden/Selected 计算听牌，显示在TilesTwo
    private void CalculateAllListen(MahjongHand hand, Grid grid)
    {
        // 如果不是打一张下叫，则退出
        if (!hand.Set.CountCanWin())
            return;

        //保存当前选中，使计算后不影响牌选中情况
        hand.Set.SaveSelected();

        // 遍历 Grid 所有 Row == 1 的，一起删除
        List<IView>? childrenToRemove = [];
        foreach (var child in grid.Children)
            if (grid.GetRow(child) == 1)
                childrenToRemove.Add(child);
        foreach (var child in childrenToRemove)
            grid.Children.Remove(child);

        // HashSet用于防止重复
        HashSet<MahjongTile> seenItem = [];
        for (int index = 0; index < hand.Set.Count; index++)
        {
            if (seenItem.Add(hand.Set[index]))  // add success, means first time 
            {
                Label l = new()
                {
                    WidthRequest = 40,
                    Padding = new Thickness(0, 0, 0, 0),
                    Margin = new Thickness(0, 20, 0, 0),
                    FontSize = 16,
                };
                Grid.SetRow(l, 1);
                Grid.SetColumn(l, index);
                grid.Children.Add(l);
                // 选择一张，计算这张打出后听牌
                hand.Set.Select(index, true);
                var canWin = hand.Set.CalculateListen(out int canWinCount);
                if (canWinCount == 0)
                    continue;
                l.Text = canWin.Name + "(" + canWinCount + ")";
            }
        }

        // 恢复选中
        hand.Set.LoadSelected();
    }

    #region Gesture & handlers

    // 上下手势，修改牌号
    private void TileModify(MahjongHand hand, Grid grid, int index, bool minus)
    {
#if DEBUG
        Trace.WriteLine($"TileModify @{index} minus: {minus}");
#endif
        Helpers.HW.VibrateDevice();
        hand.Modify1(index, minus);
        hand.Sort();
        UpdateHandtileToGrid(hand, grid);
        CalculateTileValue(hand);
        CalculateTilesCanWin(hand);
        CalculateTilesListen(hand);
        CalculateAllListen(hand, grid);
    }

    // 点击手势，计算听牌
    private void TryTile(MahjongHand hand, Grid grid, int index)
    {
#if DEBUG
        Trace.WriteLine($"TryTile @{index}");
#endif
        hand.Select1(index);   // 选中处理

        Helpers.HW.VibrateDevice();
        UpdateHandtileToGrid(hand, grid);   // 显示出来

        // 根据暗牌张数进行计算
#if DEBUG
        Trace.WriteLine("Hidden Count Now: " + hand.Set.CountHiddenTile());
#endif
        if (hand.Set.CountCanWin())
        {
            CalculateTilesCanWin(hand);
        }
        // 张数合适计算叫
        CalculateTilesListen(hand);
    }

    // 绘制手牌到顶部TileOne，并且加入手势操作，以便上点击和上下滑动的时候响应。
    private void UpdateHandtileToGrid(MahjongHand hand, Grid grid)
    {
        grid.ColumnDefinitions.Clear();
        grid.Children.Clear();
        int tileCount = hand.Set.Count;

        // 动态创建列定义
        for (int i = 0; i < tileCount; i++)
        {
            int index = i;
            grid.ColumnDefinitions.Add(new() { Width = GridLength.Auto });
            var back = new Image
            {
                Source = UI.Back1,
                HeightRequest = 75,
                WidthRequest = 55,
                Margin = new(0, (hand.IsSelected(index) ? 0 : SELECT_MOVE_UP_POINT), 0, 0)
            };
            var image = new Image
            {
                Source = hand.Images[i],
                HeightRequest = 65,
                WidthRequest = 45,
                Margin = new(0, 12 + (hand.IsSelected(index) ? 0 : SELECT_MOVE_UP_POINT), 12, 0)
            };

            // 添加Swipe手势 : 上下
            var swipeUp = new SwipeGestureRecognizer
            {
                Direction = SwipeDirection.Up,
                Threshold = 20
            };
            swipeUp.Swiped += (sender, e) => TileModify(hand, grid, index, true); 

            var swipeDown = new SwipeGestureRecognizer
            {
                Direction = SwipeDirection.Down,
                Threshold = 20,
            };
            swipeDown.Swiped += (sender, e) => TileModify(hand, grid, index, false);

            // 添加Tap手势
            var tap1 = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1,
            };
            tap1.Tapped += (sender, e) => TryTile(hand, grid, index);

            // 手势加入image
            image.GestureRecognizers.Add(swipeUp);
            image.GestureRecognizers.Add(swipeDown);
            image.GestureRecognizers.Add(tap1);

            // image加入Grid
            grid.Children.Add(back);
            grid.Children.Add(image);
            Grid.SetColumn(back, i);
            Grid.SetColumn(image, i);
        }
    }
    #endregion

    #region Commands
    [RelayCommand]
    private void Draw(string countStr)
    {
        Helpers.HW.VibrateDevice();

        _deck.Initialize();
        _handTile.Clear();

        if (!string.IsNullOrEmpty(countStr))
        {
            int tong, tiao, wan;
            var p = countStr.Split(",");
            tong = int.Parse(p[0]); // never fail because parameter is in .xaml
            tiao = int.Parse(p[1]);
            wan = int.Parse(p[2]);
            TotalTiles = tong + tiao + wan;
            _deck.DrawTile(ref _handTile, tong, tiao, wan);
        }
        else
        {
            _deck.DrawTile(ref _handTile, TongCount, TiaoCount, WanCount);
        }

        _handTile.Sort();
        UpdateHandtileToGrid(_handTile, TilesOne);
        CalculateTileValue(_handTile);
        CalculateTilesCanWin(_handTile);
        CalculateTilesListen(_handTile);
        CalculateAllListen(_handTile, TilesOne);
    }

    [RelayCommand]
    private void DrawRandom(string countStr)
    {
        Helpers.HW.VibrateDevice();

        TotalTiles = Int32.Parse(countStr);

        _deck.Initialize();
        _handTile.Clear();
        _deck.DrawTile(ref _handTile, TotalTiles);
        _handTile.Sort();
        UpdateHandtileToGrid(_handTile, TilesOne);
        CalculateTileValue(_handTile);
        CalculateTilesCanWin(_handTile);
        CalculateTilesListen(_handTile);
        CalculateAllListen(_handTile, TilesOne);
    }

    [RelayCommand]
    private void Calc()
    {
        CalculateAllListen(_handTile, TilesOne);
    }
    #endregion
}
