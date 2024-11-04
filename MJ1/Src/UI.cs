using System.Runtime.InteropServices;
using Algorithm;

namespace MJ1;

public static class UI
{
    public static readonly ImageSource Tile0x11 = ImageSource.FromFile("Tong1.png");
    public static readonly ImageSource Tile0x12 = ImageSource.FromFile("Tong2.png");
    public static readonly ImageSource Tile0x13 = ImageSource.FromFile("Tong3.png");
    public static readonly ImageSource Tile0x14 = ImageSource.FromFile("Tong4.png");
    public static readonly ImageSource Tile0x15 = ImageSource.FromFile("Tong5.png");
    public static readonly ImageSource Tile0x16 = ImageSource.FromFile("Tong6.png");
    public static readonly ImageSource Tile0x17 = ImageSource.FromFile("Tong7.png");
    public static readonly ImageSource Tile0x18 = ImageSource.FromFile("Tong8.png");
    public static readonly ImageSource Tile0x19 = ImageSource.FromFile("Tong9.png");
    public static readonly ImageSource Tile0x21 = ImageSource.FromFile("Tiao1.png");
    public static readonly ImageSource Tile0x22 = ImageSource.FromFile("Tiao2.png");
    public static readonly ImageSource Tile0x23 = ImageSource.FromFile("Tiao3.png");
    public static readonly ImageSource Tile0x24 = ImageSource.FromFile("Tiao4.png");
    public static readonly ImageSource Tile0x25 = ImageSource.FromFile("Tiao5.png");
    public static readonly ImageSource Tile0x26 = ImageSource.FromFile("Tiao6.png");
    public static readonly ImageSource Tile0x27 = ImageSource.FromFile("Tiao7.png");
    public static readonly ImageSource Tile0x28 = ImageSource.FromFile("Tiao8.png");
    public static readonly ImageSource Tile0x29 = ImageSource.FromFile("Tiao9.png");
    public static readonly ImageSource Tile0x41 = ImageSource.FromFile("Wan1.png");
    public static readonly ImageSource Tile0x42 = ImageSource.FromFile("Wan2.png");
    public static readonly ImageSource Tile0x43 = ImageSource.FromFile("Wan3.png");
    public static readonly ImageSource Tile0x44 = ImageSource.FromFile("Wan4.png");
    public static readonly ImageSource Tile0x45 = ImageSource.FromFile("Wan5.png");
    public static readonly ImageSource Tile0x46 = ImageSource.FromFile("Wan6.png");
    public static readonly ImageSource Tile0x47 = ImageSource.FromFile("Wan7.png");
    public static readonly ImageSource Tile0x48 = ImageSource.FromFile("Wan8.png");
    public static readonly ImageSource Tile0x49 = ImageSource.FromFile("Wan9.png");

    public static readonly ImageSource Back1 = ImageSource.FromFile("Back1.png");
    public static readonly ImageSource Back2 = ImageSource.FromFile("Back2.png");
    public static readonly ImageSource Back3 = ImageSource.FromFile("Back3.png");
    public static readonly ImageSource Back4 = ImageSource.FromFile("Back4.png");
    public static readonly ImageSource Back5 = ImageSource.FromFile("Back5.png");
    public static readonly ImageSource Back6 = ImageSource.FromFile("Back6.png");

    public static ImageSource TileImage(MahjongTile t) => t.Number switch {
            0x11 => Tile0x11,
            0x12 => Tile0x12,
            0x13 => Tile0x13,
            0x14 => Tile0x14,
            0x15 => Tile0x15,
            0x16 => Tile0x16,
            0x17 => Tile0x17,
            0x18 => Tile0x18,
            0x19 => Tile0x19,
            0x21 => Tile0x21,
            0x22 => Tile0x22,
            0x23 => Tile0x23,
            0x24 => Tile0x24,
            0x25 => Tile0x25,
            0x26 => Tile0x26,
            0x27 => Tile0x27,
            0x28 => Tile0x28,
            0x29 => Tile0x29,
            0x41 => Tile0x41,
            0x42 => Tile0x42,
            0x43 => Tile0x43,
            0x44 => Tile0x44,
            0x45 => Tile0x45,
            0x46 => Tile0x46,
            0x47 => Tile0x47,
            0x48 => Tile0x48,
            0x49 => Tile0x49,
            _ => throw new NotImplementedException(),
        };
}
