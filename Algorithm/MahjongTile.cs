namespace Algorithm;

public enum TILE_TYPE
{
    Tong = 0x10,
    Tiao = 0x20,
    Wan = 0x40,
}

public enum TILE_STATUS // 暗牌，选中，翻开的牌
{
    HIDDEN,
    SELECTED,
    OPEN
}

public class MahjongTile(int number)
{
    #region Properties
    // 完整值
    private int number = number;
    public int Number
    {
        get => number;
        set
        {
            if ((value > 0x10 && value <= 0x19) || (value > 0x20 && value <= 0x29) || (value > 0x40 && value <= 0x49))
                number = value;
            else
                throw new InvalidDataException($"Invalid tile number set {number}");
        }
    }

    //位于一副牌中的位置
    public int Index { get; set; } = -1;

    // 状态
    public TILE_STATUS Status { get; set; } = TILE_STATUS.HIDDEN;

    // 简易值，只有1-9
    public int NumberSimple => number & 0xF;

    // 牌名
    public string Name
    {
        get
        {
            string name = number switch
            {
                _ when number < 0x1A => "筒",
                _ when number > 0x40 => "万",
                _ => "条",
            };
            return (number & 0xF).ToString() + name;
        }
    }
    #endregion

    // 是否某种花色
    public bool IsType(TILE_TYPE type) => (number & (int)type) > 0;

    // 是否同花色
    public bool IsSameType(MahjongTile tileToCompare) => Math.Abs(number - tileToCompare.Number) < 10;

    // 选中、取消 (仅限暗牌)
    public bool Select()
    {
        Status = Status == TILE_STATUS.HIDDEN ? TILE_STATUS.SELECTED : TILE_STATUS.HIDDEN;

        return true;
    }

    // 明一张牌
    public void Open()
    {
        if (Status != TILE_STATUS.OPEN) Status = TILE_STATUS.OPEN;
    }

    // Override
    public override string ToString() => $"[{number:X2}]";

    public override bool Equals(object? obj)
    {
        if (obj == null) return false;
        return number == (obj as MahjongTile)!.number;
    }

    public override int GetHashCode() => Number.GetHashCode();

}
