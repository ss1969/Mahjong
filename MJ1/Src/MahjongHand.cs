using System.Diagnostics;
using Algorithm;

namespace MJ1;

public class MahjongHand
{
    public List<MahjongTile> Tiles { get; private set; }

    public List<ImageSource> Images() => Tiles.Select( UI.TileImage ).ToList();

    public MahjongHand() 
    {
        Tiles = [];
    }

    public void SetWhole(List<MahjongTile> init) => Tiles = init;

    public void Clear() => Tiles.Clear();
    
    public void RemoveTile(MahjongTile tile) => Tiles.Remove(tile);
    
    public int Count => Tiles.Count;
    
    public List<MahjongTile> GetTileType(TileType type) => Tiles.Where(t => t.IsType(type) ).ToList();
    
    public int Score(TileType type)
    {
        Trace.Write($"Calculate Type: {type} : ");
        return GetTileType(type).CalculateScore();
    }

    public void Sort() => Tiles.SortTiles();

    public void AddTile( MahjongTile tile ) => Tiles.Add( tile );

    public override string ToString()
    {
        return string.Join(", ", Tiles);
    }
}
