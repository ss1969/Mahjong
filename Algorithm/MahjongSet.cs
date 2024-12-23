﻿using System;
using System.Collections;
using System.Diagnostics;

namespace Algorithm;

public class MahjongSet : IEnumerable<MahjongTile>
{
    // 属性
    public List<MahjongTile> Tiles { get; set; }
    public int Count => Tiles.Count;

    // IEnumerable
    public IEnumerator<MahjongTile> GetEnumerator()
    {
        return Tiles.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    // Ctor
    public MahjongSet()
    {
        Tiles = [];
    }

    public MahjongSet(IEnumerable<MahjongTile> tiles)
    {
        Tiles = tiles.ToList();
    }

    // indexer
    public MahjongTile this[int index]
    {
        get => Tiles[index];
        set => Tiles[index] = value;
    }

    // Name
    public string Name
        //=> $"[{ string.Join(",", tiles.Select(t => $"{t.Number:X2}")) }]";
        => string.Join("", Tiles.Select(t => t.Name));

    // Sort, Copy, Remove, RemoveAt, Add, Clear
    public void Sort()
    {
        if (Tiles.Count == 0)
        {
            throw new ArgumentException("Tile list empty.");
        }

        Tiles.Sort(new MahjongTileComparer());
    }

    public MahjongSet Clone()
    {
        MahjongSet set = new()
        {
            Tiles = new(Tiles)
        };
        return set;
    }

    public void Add(MahjongTile tile) => Tiles.Add(tile);

    public bool Remove(MahjongTile tile) => Tiles.Remove(tile);

    public void RemoveAt(int index) => Tiles.RemoveAt(index);

    public void Clear() => Tiles.Clear();

    public int IndexOf(MahjongTile tile) => Tiles.IndexOf(tile);


    #region 实际操作相关方法
    // 统计花色
    public int CountType()
    {
        int sum = 0;
        foreach (var t in Tiles)
        {
            int type = (t.Number >> 4) & 0x0F; // 取出 bit 7:4
            sum |= type; // 将类型按位或到 sum 中
        }

        return sum switch
        {
            0x1 => 1,
            0x2 => 1,
            0x4 => 1,
            0x3 => 2,
            0x5 => 2,
            0x6 => 2,
            0x7 => 3,
            _ => 0,
        };
    }

    // 获取某具体牌数量
    public int CountSpecific(int tileNumber)
        => Tiles.Where(t => t.Number == tileNumber).Count();

    // 统计4张数量
    private int CountSame(int n)
        => Tiles.GroupBy(t => t.Number)
                .Count(g => g.Count() == n);

    public int CountSame4() => CountSame(4);
    public int CountSame3() => CountSame(3);

    // 获取明牌暗牌
    public MahjongSet GetOpenTiles() => new(Tiles.Where(t => t.Status == TILE_STATUS.OPEN));
    public int CountOpenTile() => Tiles.Where(t => t.Status == TILE_STATUS.OPEN).Count();
    public MahjongSet GetHiddenTiles() => new(Tiles.Where(t => t.Status == TILE_STATUS.HIDDEN));
    public int CountHiddenTile() => Tiles.Where(t => t.Status == TILE_STATUS.HIDDEN).Count();

    // 获取某类牌
    public MahjongSet GetTilesByType(TILE_TYPE type) => new(Tiles.Where(t => t.IsType(type)));

    // 获取关联牌（相差不超过1）
    public MahjongSet GetConnectedTiles()
    {
        bool[] isPresent = new bool[0x4A]; // 用于标记数字 0x11 到 0x49 是否在结果中

        foreach (var t in Tiles.Distinct())
        {
            isPresent[t.Number] = true; // 标记数字本身
            if (t.NumberSimple != 1) isPresent[t.Number - 1] = true; // 标记 num - 1
            if (t.NumberSimple != 9) isPresent[t.Number + 1] = true; // 标记 num + 1
        }

        // 根据 isPresent 数组生成结果列表
        MahjongSet result = [];
        for (int i = 1; i < 0x4A; i++)
        {
            if (isPresent[i])
                result.Add(new MahjongTile(i));
        }

        return result;
    }

    // 选中一张牌，取消其他；或者取消选中当前已选中
    // forceSelect 一定选中这张，取消其他
    public bool Select(int index, bool forceSelect = false)
    {
        if(index < 0 || index >= Tiles.Count)
            return false;

        for(int i = 0; i < Tiles.Count; i++)
        {
            switch (Tiles[i].Status)
            {
                case TILE_STATUS.OPEN:
                    break;
                case TILE_STATUS.HIDDEN:
                    if(index == i)
                        Tiles[i].Status = TILE_STATUS.SELECTED;
                    break;
                case TILE_STATUS.SELECTED:
                    if (index == i)
                    {
                        if(!forceSelect)
                            Tiles[i].Status = TILE_STATUS.HIDDEN;
                    }
                    else
                    {
                        Tiles[i].Status = TILE_STATUS.HIDDEN;
                    }
                    break;

            }
        }

        index = Tiles.FindIndex(t => t.Status == TILE_STATUS.SELECTED);
        return index != -1;
    }

    // 选择一张特定牌；未测
    public bool Select(MahjongTile tile)
    {
        int i;
        if ((i = Tiles.IndexOf(tile)) != -1)
        {
            Select(i, true);
            return true;
        }

        return false;
    }

    //保存和恢复选中牌
    private int _selectedSave;
    public void SaveSelected()
    {
        _selectedSave = Tiles.FindIndex(t => t.Status == TILE_STATUS.SELECTED);
#if DEBUG
        Trace.WriteLine($"SaveSelected: {_selectedSave}");
#endif
    }

    public void LoadSelected()
    {
        if (_selectedSave == -1)
        {
            foreach (var t in Tiles)
                if (t.Status == TILE_STATUS.SELECTED )
                    t.Status = TILE_STATUS.HIDDEN;
            return;
        }
        Tiles[_selectedSave].Status = TILE_STATUS.SELECTED;
        Trace.WriteLine($"LoadSelected: {_selectedSave}");
    }

    #endregion
}