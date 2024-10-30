using System.Diagnostics;
using Algorithm;

namespace MJ1;

public class MahjongHand
{
    public List<MahjongTile> Tiles { get; set; }

    public MahjongHand() 
    {
        Tiles = [];
    }

    public void SetWhole(List<MahjongTile> init) => Tiles = init;
    public void Clear() => Tiles.Clear();
    public void RemoveTile(MahjongTile tile) => Tiles.Remove(tile);
    public int Count => Tiles.Count;
    public List<MahjongTile> GetTileType(MahjongTile.TileType type) => Tiles.Where(tile => tile.Type == type).ToList();
    public int Score(MahjongTile.TileType type)
    {
        Trace.Write($"Calculate Type: {type} : ");
        return GetTileType(type).CalculateScore();
    }

    public void Sort() => Tiles.SortTiles();

    public void AddTile(MahjongTile tile)
    {
        if (Tiles.Count < 13)
        {
            Tiles.Add(tile);
        }
        else
        {
            throw new InvalidOperationException("Cannot add more than 13 tiles to a hand.");
        }
    }
    public override string ToString()
    {
        return string.Join(", ", Tiles);
    }
}
