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

    private MahjongSet _set = [];
    public MahjongSet Set => _set;

    public int Count => _set.Count;
    #endregion
    public void SetWhole(MahjongSet initSet)
    {
        _set = initSet;
        Images = new(_set.Select(UI.TileImage));
    }

    public void Clear()
    {
        _set.Clear();
        Images.Clear();
    }

    public void AddTile(MahjongTile tile)
    {
        _set.Add(tile);
        Images.Add(UI.TileImage(tile));
    }

    public void RemoveTile(MahjongTile tile)
    {
        var index = _set.IndexOf(tile);
        _set.Remove(tile);
        Images.RemoveAt(index);
    }

    public void RemoveTile(int index)
    {
        _set.RemoveAt(index);
        Images.RemoveAt(index);
    }

    public void ChangeTile(int index, MahjongTile newOne)
    {
        if (index >= _set.Count)
            return;
        _set[index] = newOne;
        Images[index] = UI.TileImage(newOne);
    }

    public int GetScoreByType(TILE_TYPE type) => _set.GetTilesByType(type).CalculateValue();

    public void Sort()
    {
        _set.Sort();
        Images = new(_set.Select(UI.TileImage));
    }

    public void Modify1(int index, bool minus)    // 修改一个手牌中某一张
    {
        if (index >= _set.Count) return;

        var t = _set[index];
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
        if (_set.CountSpecific(newValue) == 4) return;

        ChangeTile(index, new(newValue));
    }

    public bool Select1(int index)
    {
        if (index >= _set.Count) 
            return false;
        return _set.Select(index);
    }

    public bool IsSelected(int index) => _set[index].Status == TILE_STATUS.SELECTED;
}
