namespace SpecTech.Domain.Extensions;

public static class ListExtension
{
    public static string TryGet(this List<string> list, int key)
    {
        if (list.Count() - 1 < key) return "";
        return list[key];
    }
}