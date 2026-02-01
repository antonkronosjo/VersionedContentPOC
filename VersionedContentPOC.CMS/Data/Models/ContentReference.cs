using System.Text.Json.Serialization;

namespace VersionedContentPOC.CMS.Data.Models;

public readonly struct ContentReference : IEquatable<ContentReference>
{
    public int ContentId { get; }

    [JsonConstructor]
    public ContentReference(int contentId)
    {
        ContentId = contentId;
    }

    public static ContentReference Empty => new ContentReference(0);

    public bool Equals(ContentReference other)
        => ContentId == other.ContentId;

    public override bool Equals(object? obj)
        => obj is ContentReference other && Equals(other);

    public override int GetHashCode()
        => ContentId.GetHashCode();

    public override string ToString()
        => ContentId.ToString();
}
