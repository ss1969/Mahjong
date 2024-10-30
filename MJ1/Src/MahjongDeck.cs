using System.Diagnostics;
using Algorithm;

namespace MJ1;

public class MahjongDeck
{
    private List<MahjongTile> tiles;
    private static readonly Random random = new ();

    public string TileBack { get; set; } = "Back1.png";

    public MahjongDeck()
    {
        tiles = [];
        Initialize();
    }
    
    public void Initialize()
    {
        tiles.Clear();
        foreach (MahjongTile.TileType type in Enum.GetValues(typeof(MahjongTile.TileType)))
        {
            for (int number = 1; number <= 9; number++)
            {
                for (int i = 0; i < 4; i++)
                {
                    tiles.Add(new MahjongTile
                    {
                        Type = type,
                        Number = number,
                        TileImage = $"{type.ToString().ToLower()}{number}.png" // 假设图片命名为 wan1.png, tong1.png, tiao1.png
                    });
                }
            }
        }
    }

    // 摸牌
    public void DrawTile(ref MahjongHand dest, int count)
    {
        Debug.Assert(dest != null); 
        dest.SetWhole(tiles.OrderBy(x => random.Next()).Take(count).ToList());
    }

    public void DrawTile(ref MahjongHand dest)
    {
        dest.AddTile(tiles.OrderBy(x => random.Next()).First());
    }

    public void DrawTile(ref MahjongHand dest, int wanCount, int tongCount, int tiaoCount)
    {
        // Helper function to draw tiles of a specific type
        List<MahjongTile> DrawSpecificTiles(MahjongTile.TileType type, int count)
        {
            var tilesOfType = tiles.Where(tile => tile.Type == type).OrderBy(tile => random.Next()).Take(count).ToList();
            if (tilesOfType.Count < count)
            {
                throw new InvalidOperationException($"Not enough {type} tiles in the deck.");
            }
            return tilesOfType;
        }

        try
        {
            var wanTiles = DrawSpecificTiles(MahjongTile.TileType.Wan, wanCount);
            var tongTiles = DrawSpecificTiles(MahjongTile.TileType.Tong, tongCount);
            var tiaoTiles = DrawSpecificTiles(MahjongTile.TileType.Tiao, tiaoCount);

            // Add drawn tiles to the destination hand and remove them from the deck
            foreach (var tile in wanTiles.Concat(tongTiles).Concat(tiaoTiles))
            {
                dest.AddTile(tile);
                tiles.Remove(tile);
            }
        }
        catch (InvalidOperationException ex)
        {
            throw new InvalidOperationException($"AddTile to dest failed");
        }
    }
}
