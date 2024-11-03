using System.Collections.ObjectModel;
using System.Diagnostics;
using Algorithm;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MJ1;

public partial class MahjongHand : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<ImageSource> images = [];

    public List<MahjongTile> Tiles { get; private set; } = [];

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
    
    public int Count => Tiles.Count;
    
    public List<MahjongTile> GetTilesByType(TileType type) 
        => Tiles.Where(t => t.IsType(type) )
                .ToList();
    
    public int GetScoreByType(TileType type)
    {
        var s = GetTilesByType(type).Calculate();
        Trace.Write($"Calculate Type: {type} : {s}");
        return s;
    }

    public void Sort()
    {
        MahjongTileHelper.Sort(Tiles);
        Images = new (Tiles.Select(UI.TileImage));
    }

    public void AddTile(MahjongTile tile)
    {
        Tiles.Add(tile);
        Images.Add(UI.TileImage(tile));
    }

    public override string ToString() => Tiles.Info();
}
