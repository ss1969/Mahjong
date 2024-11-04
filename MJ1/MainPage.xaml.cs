using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Algorithm;

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
    private string totalTiles = "总13 张";

    partial void OnWanCountChanged(int value) => TotalTiles = $"总{WanCount + TongCount + TiaoCount} 张";
    partial void OnTongCountChanged(int value) => TotalTiles = $"总{WanCount + TongCount + TiaoCount} 张";
    partial void OnTiaoCountChanged(int value) => TotalTiles = $"总{WanCount + TongCount + TiaoCount} 张";

    #endregion

    private MahjongDeck _deck;
    private MahjongHand _handTile;
    public event Action<MahjongHand>? UpdateHandView = null;
    public Grid TilesOne { get; set; } = [];

    public ViewModel()
    {
        _deck = new();
        _handTile = new();
    }

    public static void FindMaxMin( int num1, int num2, int num3, out int maxValue, out int minValue )
    {
        // 初始化最大值和最小值为第一个数字
        maxValue = num1;
        minValue = num1;

        // 比较第二个数字
        if ( num2 > maxValue )
        {
            maxValue = num2;
        }
        else if ( num2 < minValue )
        {
            minValue = num2;
        }

        // 比较第三个数字
        if ( num3 > maxValue )
        {
            maxValue = num3;
        }
        else if ( num3 < minValue )
        {
            minValue = num3;
        }
    }

    private void CalculateTileValue()
    {
        var s1 = _handTile.GetScoreByType(TileType.Tong);
        var s2 = _handTile.GetScoreByType(TileType.Tiao);
        var s3 = _handTile.GetScoreByType(TileType.Wan);
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

    private void CalculateHandTileWin() // 计算胡牌，不管张数
    {
        var win = _handTile.Tiles.Calculate(out int score, out string detail);
        WinLabel = win ? $"WIN {score} {detail}" : $"NO {detail}";
        WinLabelColor = win ? Colors.Red : Colors.Green;
    }

    private static void VibrateDevice()
    {
        if (DeviceInfo.Platform == DevicePlatform.Android)
        {
            TimeSpan vibrationLength = TimeSpan.FromMilliseconds(50);
            Vibration.Default.Vibrate(vibrationLength);
        }
    }
    
    private void UpdateHandtileToGrid(MahjongHand hand, Grid grid)
    {
        grid.ColumnDefinitions.Clear();
        grid.Children.Clear();
        int tileCount = hand.Tiles.Count;

        // 动态创建列定义
        for (int i = 0; i < tileCount; i++)
        {
            grid.ColumnDefinitions.Add(new (){ Width = GridLength.Auto });
            var back = new Image{ Source = UI.Back1 };
            var image = new Image{ Source = hand.Images[i], 
                                        Margin = new (0,12,12,0) };

            // 添加手势
            int index = i;
            var swipeUp = new SwipeGestureRecognizer{ 
                Direction = SwipeDirection.Up,
                Threshold = 20
            };
            swipeUp.Swiped += (sender, e) =>
            {
                VibrateDevice();
                _handTile.Modify1(index, true);
                _handTile.Sort();
                UpdateHandtileToGrid(_handTile, TilesOne);
                CalculateTileValue();
                CalculateHandTileWin();
            };

            var swipeDown = new SwipeGestureRecognizer{ 
                Direction = SwipeDirection.Down,
                Threshold = 20,
            };
            swipeDown.Swiped += (sender, e) =>
            {
                VibrateDevice();
                _handTile.Modify1(index, false);
                _handTile.Sort();
                UpdateHandtileToGrid(_handTile, TilesOne);
                CalculateTileValue();
                CalculateHandTileWin();
            };

            image.GestureRecognizers.Add(swipeUp);
            image.GestureRecognizers.Add(swipeDown);

            // 加入Grid
            grid.Children.Add(back);
            grid.Children.Add(image);
            Grid.SetColumn(back, i);
            Grid.SetColumn(image, i);
        }
    }

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
            _deck.DrawTile(ref _handTile, tong, tiao, wan);
        }
        else
        {
            _deck.DrawTile(ref _handTile, TongCount, TiaoCount, WanCount);
        }

        _handTile.Sort();
        UpdateHandtileToGrid(_handTile, TilesOne);
        CalculateTileValue();
        CalculateHandTileWin();
    }

    [RelayCommand]
    private void DrawRandom(string countStr)
    {
        VibrateDevice();

        int count = Int32.Parse(countStr);
        _deck.Initialize();
        _handTile.Clear();
        _deck.DrawTile(ref _handTile, count);
        _handTile.Sort();
        UpdateHandtileToGrid(_handTile, TilesOne);
        CalculateTileValue();
        CalculateHandTileWin();
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
