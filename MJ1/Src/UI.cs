using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Algorithm;

namespace MJ1;

public static class UI
{
    private static GCHandle handle;

    static UI()
    {
        var instance = new object();

        // 固定实例,防止被垃圾回收
        handle = GCHandle.Alloc( instance, GCHandleType.Normal );
    }

    private static readonly Dictionary<int, ImageSource> TileImageTable = new()
    {
        { 0x11, ImageSource.FromFile("Tong1.png") },
        { 0x12, ImageSource.FromFile("Tong2.png") },
        { 0x13, ImageSource.FromFile("Tong3.png") },
        { 0x14, ImageSource.FromFile("Tong4.png") },
        { 0x15, ImageSource.FromFile("Tong5.png") },
        { 0x16, ImageSource.FromFile("Tong6.png") },
        { 0x17, ImageSource.FromFile("Tong7.png") },
        { 0x18, ImageSource.FromFile("Tong8.png") },
        { 0x19, ImageSource.FromFile("Tong9.png") },

        { 0x21, ImageSource.FromFile("Tiao1.png") },
        { 0x22, ImageSource.FromFile("Tiao2.png") },
        { 0x23, ImageSource.FromFile("Tiao3.png") },
        { 0x24, ImageSource.FromFile("Tiao4.png") },
        { 0x25, ImageSource.FromFile("Tiao5.png") },
        { 0x26, ImageSource.FromFile("Tiao6.png") },
        { 0x27, ImageSource.FromFile("Tiao7.png") },
        { 0x28, ImageSource.FromFile("Tiao8.png") },
        { 0x29, ImageSource.FromFile("Tiao9.png") },

        { 0x41, ImageSource.FromFile("Wan1.png") },
        { 0x42, ImageSource.FromFile("Wan2.png") },
        { 0x43, ImageSource.FromFile("Wan3.png") },
        { 0x44, ImageSource.FromFile("Wan4.png") },
        { 0x45, ImageSource.FromFile("Wan5.png") },
        { 0x46, ImageSource.FromFile("Wan6.png") },
        { 0x47, ImageSource.FromFile("Wan7.png") },
        { 0x48, ImageSource.FromFile("Wan8.png") },
        { 0x49, ImageSource.FromFile("Wan9.png") }
    };

    public static ImageSource TileImage(MahjongTile t) => TileImageTable[t.Number];

}
