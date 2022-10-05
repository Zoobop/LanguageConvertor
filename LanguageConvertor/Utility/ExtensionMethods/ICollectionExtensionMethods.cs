namespace LanguageConvertor.Utility;

internal static class ICollectionExtensionMethods
{
    public static bool IsEmpty<T>(this ICollection<T> collection)
    {
        return collection.Count == 0;
    }
}