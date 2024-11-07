using System.Collections.ObjectModel;
using System.Diagnostics;
using Algorithm;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MJ1;

public partial class MahjongHand : ObservableObject
{
    #region Properties
    [ObservableProperty]
    private ObservableCollection<ImageSource> images = [];

    public List<MahjongTile> Tiles { get; private set; } = [];

    public int Count => Tiles.Count;
    #endregion

    public void SetWhole(List<MahjongTile> init)
    {
        Tiles = init;
        Images = new(Tiles.Select(UI.TileImage));
    }

    public void Clear()
    {
        Tiles.Clear();
        Images.Clear();
    }

    public void AddTile(MahjongTile tile)
    {
        Tiles.Add(tile);
        Images.Add(UI.TileImage(tile));
    }

    public void RemoveTile(MahjongTile tile)
    {
        var index = Tiles.IndexOf(tile);
        Tiles.Remove(tile);
        Images.RemoveAt(index);
    }

    public void ChangeTile(int index, MahjongTile newOne)
    {
        if (index >= Tiles.Count)
            return;
        Tiles[index] = newOne;
        Images[index] = UI.TileImage(newOne);
    }
    
    public int GetScoreByType(TILE_TYPE type) => Tiles.GetTilesByType(type).Calculate();

    public void Sort()
    {
        MahjongTileHelper.Sort(Tiles);
        Images = new (Tiles.Select(UI.TileImage));
    }

    public void Modify1(int index, bool minus)    // 修改一个手牌中某一张
    {
        if ( index >= Tiles.Count ) return;

        var t = Tiles[index];
        int newValue;

        if (minus)
        {
            if (t.NumberSimple == 1)        // 如果已经是 1 了，退出
                newValue = (t.Number & 0xF0) + 9;
            else
                newValue = t.Number - 1;
        }
        else
        {
            if (t.NumberSimple == 9)        // 如果已经是 9 了，退出
                newValue = (t.Number & 0xF0) + 1;
            else
                newValue = t.Number + 1;
        }

        // 已经有4个牌了（ kiilii 算法有问题，不是很好，不能直接跳过 ）
        if (Tiles.CountSpecific(newValue) == 4) return;

        ChangeTile(index, new(newValue));
    }

    public void Select1(int index)
    {
        if (index >= Tiles.Count) return;
        Tiles[index].Select();
    }
    
    public bool IsSelected(int index) => Tiles[index].Status == TILE_STATUS.SELECTED;
}
