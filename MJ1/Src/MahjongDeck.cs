using System.Diagnostics;
using Algorithm;

namespace MJ1;

public class MahjongDeck
{
    #region Vars
    private readonly MahjongSet set = [];
    private static readonly Random random = new();
    #endregion

    #region Prop
    public Image TileBack { get; set; }
    #endregion

    #region Ctor
    public MahjongDeck()
    {
        Initialize();
        TileBack = new Image() { Source = UI.Back1 };
    }
    #endregion

    #region Methods
    public void Initialize()
    {
        set.Clear();
        for (int i = 0; i < 4; i++)
        {
            for (int number = 0x11; number <= 0x19; number++)
                set.Add(new(number));
            for (int number = 0x21; number <= 0x29; number++)
                set.Add(new(number));
            for (int number = 0x41; number <= 0x49; number++)
                set.Add(new(number));
        }
    }

    // 摸牌
    public MahjongHand DrawTile(ref MahjongHand dst, int count)
    {
        Debug.Assert(dst != null);
        dst.SetWhole(new(set.OrderBy(x => random.Next()).Take(count)));
        return dst;
    }

    public MahjongHand DrawTile(ref MahjongHand dest)
    {
        dest.AddTile(set.OrderBy(x => random.Next()).First());
        return dest;
    }

    public MahjongHand DrawTile(ref MahjongHand dest, int tongCount, int tiaoCount, int wanCount)
    {
        // Helper function to draw tiles of a specific type
        List<MahjongTile> DrawSpecificTiles(TILE_TYPE type, int count)
        {
            var tilesOfType = set.Where(t => t.IsType(type)).OrderBy(t => random.Next()).Take(count).ToList();
            if (tilesOfType.Count < count)
            {
                throw new InvalidOperationException($"Not enough {type} tiles in the deck.");
            }
            return tilesOfType;
        }

        try
        {
            var wanTiles = DrawSpecificTiles(TILE_TYPE.Wan, wanCount);
            var tongTiles = DrawSpecificTiles(TILE_TYPE.Tong, tongCount);
            var tiaoTiles = DrawSpecificTiles(TILE_TYPE.Tiao, tiaoCount);

            // Add drawn tiles to the destination hand and remove them from the deck
            foreach (var tile in wanTiles.Concat(tongTiles).Concat(tiaoTiles))
            {
                dest.AddTile(tile);
                set.Remove(tile);
            }
        }
        catch (InvalidOperationException)
        {
            throw new InvalidOperationException($"AddTile() to dest failed");
        }

        return dest;
    }
    #endregion

}
