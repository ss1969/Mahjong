namespace Algorithm;

public class MahjongTile(int number, MahjongTile.TileType type = MahjongTile.TileType.Wan, string? image = null)
{
    public enum TileType
    {
        Wan,
        Tong,
        Tiao
    }
    public TileType Type { get; set; } = type;
    public int Number { get; set; } = number;
    public string? TileImage { get; set; } = image;
}
