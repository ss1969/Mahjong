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
    private List<MahjongTile> myTiles = [];

    [ObservableProperty] 
    private MahjongDeck deck;

    [ObservableProperty]
    private string scoreLabel;

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
        ScoreLabel = "";
        HandTiles = new();
    }

    [RelayCommand]
    private void Draw()
    {
        Deck.Initialize();
        HandTiles.Clear();
        Deck.DrawTile(ref HandTiles, WanCount, TongCount, TiaoCount);
        HandTiles.Sort();

        MyTiles = new(HandTiles.Tiles);
        Calculate();
    }

    [RelayCommand]
    private void DrawRandom()
    {
        Deck.Initialize();
        HandTiles.Clear();
        Deck.DrawTile(ref HandTiles, 13);
        HandTiles.Sort();

        MyTiles = new(HandTiles.Tiles);
        Calculate();
    }
    [RelayCommand]
    private void Calculate()
    {
        ScoreLabel = $"Wan: {HandTiles.Score(MahjongTile.TileType.Wan)}, Tong: {HandTiles.Score(MahjongTile.TileType.Tong)}, Tiao: {HandTiles.Score(MahjongTile.TileType.Tiao)}";
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
