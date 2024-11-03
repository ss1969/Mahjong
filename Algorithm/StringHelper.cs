namespace Algorithm;

public static class StringHelper
{
    public static bool RemoveCharsIfPossible(ref string source, string toRemove)
    {
        // 将字符串A和B转换为List<char>
        List<char> listA = [.. source];
        List<char> listB = [.. toRemove];

        // 对于B中的每一个字符
        foreach (char charToRemove in listB)
        {
            // 尝试在listA中找到并移除第一个匹配的字符
            if (!listA.Remove(charToRemove))
            {
                // 如果移除失败，不修改原始字符串source
                return false;
            }
        }

        // 如果所有字符顺利移除，则source修改为移除后的集合转化后的新字符串
        source = new string(listA.ToArray());
        return true;
    }

    public static bool HasDuplicate(this string str)
    {
        var seen = new HashSet<char>();
        foreach (char c in str)
        {
            if (!seen.Add(c)) // 如果添加失败，说明c已经在seen中了
            {
                return true; // 有重复值
            }
        }
        return false; // 没有重复值
    }
}
