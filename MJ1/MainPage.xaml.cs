﻿using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Algorithm;
using System.Diagnostics;

namespace MJ1;

public partial class ViewModel : ObservableObject
{
    #region Properties
    [ObservableProperty]
    private MahjongHand handTiles;
    
    [ObservableProperty] 
    private MahjongDeck deck;

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

    private void CalculateTileValue()
    {
        var s1 = HandTiles.GetScoreByType(TileType.Tong);
        var s2 = HandTiles.GetScoreByType(TileType.Tiao);
        var s3 = HandTiles.GetScoreByType(TileType.Wan);
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
    private void CalculateHandTileWin()
    {
        var win = HandTiles.Tiles.Calculate(out int score, out string detail);
        WinLabel = win ? $"WIN {score} {detail}" : $"NO {detail}";
        WinLabelColor = win ? Colors.Red : Colors.Green;
    }

    #region Commands
    [RelayCommand]
    private void Draw(string countStr)
    {
        var p = countStr.Split(",");
        if (!string.IsNullOrEmpty(p[0])) TongCount = int.Parse(p[0]);
        if (!string.IsNullOrEmpty(p[1])) TiaoCount = int.Parse(p[1]);
        if (!string.IsNullOrEmpty(p[2])) WanCount = int.Parse(p[2]);

        Deck.Initialize();
        HandTiles.Clear();
        Deck.DrawTile(ref handTiles, TongCount, TiaoCount, WanCount);
        HandTiles.Sort();
        OnPropertyChanged(nameof(HandTiles)); // 触发属性变化通知更新绑定

        CalculateTileValue();
        CalculateHandTileWin();
    }

    [RelayCommand]
    private void DrawRandom(string countStr)
    {
        int count = Int32.Parse(countStr);
        Deck.Initialize();
        HandTiles.Clear();
        Deck.DrawTile(ref handTiles, count);
        HandTiles.Sort();
        OnPropertyChanged(nameof(HandTiles)); // 触发属性变化通知更新绑定

        CalculateTileValue();
        CalculateHandTileWin();
    }
    #endregion

    public void OnSwipe(ImageSource item, bool Up)
    {
        int index = HandTiles.Images.IndexOf( item );
        var t = HandTiles.Tiles[index];
        int newValue;

        if(Up)
        {
            Trace.WriteLine($"Item at index {index} swiped UP");
            // 如果已经是 9 了，退出
            if (t.NumberSimple == 9) return;
            // 尝试 +1
            newValue = t.Number + 1;
        }
        else
        {
            Trace.WriteLine($"Item at index {index} swiped down");
            // 如果已经是 1 了，退出
            if (t.NumberSimple == 1) return;
            // 尝试 -1
            newValue = t.Number - 1;
        }

        // 已经有4个牌了（ kiilii 算法有问题，不是很好，不能直接跳过 ）
        if (HandTiles.Tiles.GetSpecificCount(newValue) == 4) return;

        // 修改
        HandTiles.ChangeTile(index, new(newValue));
        HandTiles.Sort();
        //Trace.WriteLine($"Tiles #{index} => {newValue}");

        // 计算
        CalculateTileValue();
        CalculateHandTileWin();
    }
}


public partial class MainPage : ContentPage
{
    private double _panStartPositionY;
    private double _panTotalY;

    public MainPage()
    {
        InitializeComponent();
        BindingContext = new ViewModel();
    }

    private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        const double swipeMinimumDistance = 30;
        var grid = sender as Grid;

        switch (e.StatusType)
        {
            case GestureStatus.Started:
                Trace.WriteLine("G-Start");
                _panStartPositionY = e.TotalY;
                break;

            case GestureStatus.Running:
                _panTotalY = e.TotalY;
                break;

            case GestureStatus.Completed:
                double panDistance = _panTotalY - _panStartPositionY;
                Trace.WriteLine($"G-Complete {panDistance}");

                if (Math.Abs(panDistance) >= swipeMinimumDistance)
                {
                    var item = grid?.BindingContext as ImageSource;
                    if (panDistance < 0)
                    {
                        (BindingContext as ViewModel)!.OnSwipe(item!, true);
                    }
                    else
                    {
                        (BindingContext as ViewModel)!.OnSwipe(item!, false);
                    }
                }
                // 重置位置
                _panStartPositionY = 0;
                _panTotalY = 0;
                break;
            default:
                Trace.WriteLine($"G-{e.StatusType}");
                break;
        }
    }
}