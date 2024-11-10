using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Algorithm;
using System.Diagnostics;

namespace MJ1;

public partial class ViewModel : ObservableObject
{
    #region Properties
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

    private MahjongDeck _deck;
    private MahjongHand _handTile;
    public Grid TilesOne { get; set; } = [];    // 第一行

    private static readonly int SELECT_MOVE_UP_POINT = 50;

    public ViewModel()
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
        {
            maxValue = num2;
        }
        else if (num2 < minValue)
        {
            minValue = num2;
        }

        // 比较第三个数字
        if (num3 > maxValue)
        {
            maxValue = num3;
        }
        else if (num3 < minValue)
        {
            minValue = num3;
        }
    }

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

    private void CalculateTilesCanWin(MahjongHand hand) // 计算胡牌，不管张数
    {
        var win = hand.Set.CalculateWin(out int score, out string detail);
        WinLabel = win ? $"WIN {score}分 {detail}" : $"NO {detail}";
        WinLabelColor = win ? Colors.Red : Colors.Green;
    }

    private void CalculateTilesListen(MahjongHand hand)
    {
        if(!hand.Set.GetHiddenTiles().Count.CountCanListen())
        {
            CanWinTiles = "无法听牌";
            return;
        }
        var canWin = hand.Set.CalculateListen(out int canWinCount);

        // 输出
        if (canWin.Count > 0)
            CanWinTiles = $"听牌：{canWin.Name}, 共 {canWin.Count} 类 {canWinCount} 张";
        else
            CanWinTiles = "无法听牌";
    }

    private static void VibrateDevice()
    {
        if (DeviceInfo.Platform == DevicePlatform.Android)
        {
            TimeSpan vibrationLength = TimeSpan.FromMilliseconds(50);
            Vibration.Default.Vibrate(vibrationLength);
        }
    }

    #region Gesture & handlers
    private void TileModify(MahjongHand hand, Grid grid, int index, bool minus)
    {
#if DEBUG
        Trace.WriteLine($"TileModify @{index} minus: {minus}");
#endif
        VibrateDevice();
        hand.Modify1(index, minus);
        hand.Sort();
        UpdateHandtileToGrid(hand, grid);
        CalculateTileValue(hand);
        CalculateTilesCanWin(hand);
        CalculateTilesListen(hand);
    }


    private void TryTile(MahjongHand hand, Grid grid, int index)
    {
#if DEBUG
        Trace.WriteLine($"TryTile @{index}");
#endif
        if (!hand.Select1(index))
            return;

        VibrateDevice();
        UpdateHandtileToGrid(hand, grid);

        //        // 修改Margin造成上移效果
        //        foreach (var child in grid.Children)
        //        {
        //            if (child is Image image && Grid.GetColumn(image) == index)
        //            {
        //                if (image.Source == UI.Back1)
        //                    image.Margin = new Thickness(0, (hand.IsSelected(index) ? 0 : SELECT_MOVE_UP_POINT), 0, 0);
        //                else
        //                    image.Margin = new Thickness(0, 12 + (hand.IsSelected(index) ? 0 : SELECT_MOVE_UP_POINT), 12, 0);
        //#if DEBUG
        //                //Trace.WriteLine(" Selected " + hand.IsSelected(index) + " Margin " + image.Margin.Top );   
        //#endif
        //            }
        //        }
        //        // 根据暗牌张数进行计算
#if DEBUG
        Trace.WriteLine("Hidden Count Now: " + hand.Set.CountHiddenTile());
#endif
        if (hand.Set.CountHiddenTile().CountCanWin())
        {
            CalculateTilesCanWin(hand);
        }
        // 张数合适计算叫
        CalculateTilesListen(hand);
    }

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
        VibrateDevice();

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
    }

    [RelayCommand]
    private void DrawRandom(string countStr)
    {
        VibrateDevice();

        TotalTiles = Int32.Parse(countStr);

        _deck.Initialize();
        _handTile.Clear();
        _deck.DrawTile(ref _handTile, TotalTiles);
        _handTile.Sort();
        UpdateHandtileToGrid(_handTile, TilesOne);
        CalculateTileValue(_handTile);
        CalculateTilesCanWin(_handTile);
        CalculateTilesListen(_handTile);
    }
    #endregion
}


public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        BindingContext = new ViewModel();
        (BindingContext as ViewModel)!.TilesOne = TilesOne;

    }

}
