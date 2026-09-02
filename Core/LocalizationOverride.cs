namespace VersusPlus;

internal sealed class LocalizationOverride
{
    private readonly LocalizationDictionary dictionary;

    public LocalizationOverride(LocalizationDictionary dictionary, string id)
    {
        ArgumentNullException.ThrowIfNull(id);

        this.dictionary = dictionary;
        this.Id = id;
        this.OriginalFormat = dictionary.Get(id);
    }

    public string Id { get; }

    public string OriginalFormat { get; }

    public void Set(string format)
    {
        ArgumentNullException.ThrowIfNull(format);

        this.dictionary.Set(this.Id, format);
    }
}
