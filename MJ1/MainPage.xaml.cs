using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Algorithm;

namespace MJ1;

public partial class ViewModel : ObservableObject
{
    #region Properties

    [ObservableProperty]
    private List<ImageSource> myTilesI = [];

    [ObservableProperty] 
    private MahjongDeck deck;

    [ObservableProperty]
    private string scoreLabel1 = "";
    [ObservableProperty]
    private string scoreLabel2 = "";
    [ObservableProperty]
    private string scoreLabel3 = "";
    [ObservableProperty]
    private Color scoreLabelColor1 = Colors.White;
    [ObservableProperty]
    private Color scoreLabelColor2 = Colors.White;
    [ObservableProperty]
    private Color scoreLabelColor3 = Colors.White;

    [ObservableProperty]
    private int wanCount = 4;
    [ObservableProperty]
    private int tongCount = 4;
    [ObservableProperty]
    private int tiaoCount = 5; 
    [ObservableProperty]
    private int totalTiles = 13;

    partial void OnWanCountChanged(int value)
    {
        TotalTiles = WanCount + TongCount + TiaoCount;
    }
    partial void OnTongCountChanged(int value)
    {
        TotalTiles = WanCount + TongCount + TiaoCount;
    }
    partial void OnTiaoCountChanged(int value)
    {
        TotalTiles = WanCount + TongCount + TiaoCount;
    }
    #endregion

    private MahjongHand HandTiles;

    public ViewModel()
    {
        Deck = new();
        HandTiles = new();
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

    [RelayCommand]
    private void Draw()
    {
        Deck.Initialize();
        HandTiles.Clear();
        Deck.DrawTile(ref HandTiles, TongCount, TiaoCount, WanCount);
        HandTiles.Sort();

        MyTilesI = HandTiles.Images();
        Calculate();
    }

    [RelayCommand]
    private void DrawRandom()
    {
        Deck.Initialize();
        HandTiles.Clear();
        Deck.DrawTile(ref HandTiles, 13);
        HandTiles.Sort();

        MyTilesI = HandTiles.Images();
        Calculate();
    }
    [RelayCommand]
    private void Calculate()
    {
        var s1 = HandTiles.Score( TileType.Tong );
        var s2 = HandTiles.Score( TileType.Tiao );
        var s3 = HandTiles.Score( TileType.Wan );
        FindMaxMin(s1, s2, s3, out int max, out int min);

        ScoreLabel1 = $"筒: {s1}";
        ScoreLabel2 = $"条: {s2}";
        ScoreLabel3 = $"万: {s3}";

        if ( s1 == max ) ScoreLabelColor1 = Colors.Red;
        else if ( s1 == min ) ScoreLabelColor1 = Colors.Green;
        else ScoreLabelColor1 = Colors.White;
        if ( s2 == max ) ScoreLabelColor2 = Colors.Red;
        else if ( s2 == min ) ScoreLabelColor2 = Colors.Green;
        else ScoreLabelColor2 = Colors.White;
        if ( s3 == max ) ScoreLabelColor3 = Colors.Red;
        else if ( s3 == min ) ScoreLabelColor3 = Colors.Green;
        else ScoreLabelColor3 = Colors.White;
    }

}


public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        BindingContext = new ViewModel();
    }
}
