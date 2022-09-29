namespace LanguageConvertor.Utility;

public static class ICollectionExtensionMethods
{
    public static bool IsEmpty<T>(this ICollection<T> collection)
    {
        return collection.Count == 0;
    }
}