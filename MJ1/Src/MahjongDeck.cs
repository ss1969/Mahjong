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
        for ( int i = 0; i < 4; i++ )
        {
            for ( int number = 0x11; number <= 0x19; number++ )
                tiles.Add( new MahjongTile( number ) );
            for ( int number = 0x21; number <= 0x29; number++ )
                tiles.Add( new MahjongTile( number ) );
            for ( int number = 0x41; number <= 0x49; number++ )
                tiles.Add( new MahjongTile( number ) );
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

    public void DrawTile(ref MahjongHand dest, int tongCount, int tiaoCount, int wanCount)
    {
        // Helper function to draw tiles of a specific type
        List<MahjongTile> DrawSpecificTiles(TileType type, int count)
        {
            var tilesOfType = tiles.Where( t => t.IsType( type ) ).OrderBy(t => random.Next()).Take(count).ToList();
            if (tilesOfType.Count < count)
            {
                throw new InvalidOperationException($"Not enough {type} tiles in the deck.");
            }
            return tilesOfType;
        }

        try
        {
            var wanTiles = DrawSpecificTiles(TileType.Wan, wanCount);
            var tongTiles = DrawSpecificTiles(TileType.Tong, tongCount);
            var tiaoTiles = DrawSpecificTiles(TileType.Tiao, tiaoCount);

            // Add drawn tiles to the destination hand and remove them from the deck
            foreach (var tile in wanTiles.Concat(tongTiles).Concat(tiaoTiles))
            {
                dest.AddTile(tile);
                tiles.Remove(tile);
            }
        }
        catch (InvalidOperationException)
        {
            throw new InvalidOperationException($"AddTile() to dest failed");
        }
    }
}
