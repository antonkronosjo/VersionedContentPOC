namespace VersionedContentPOC.Extensions;

public static class ICollectionExtensions
{
    public static bool AddIfNotAny<T>(this ICollection<T> source, T item, Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);

        if (source.Any(predicate))
            return false;
        

        source.Add(item);
        return true;
    }
}
