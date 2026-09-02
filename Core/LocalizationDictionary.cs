using System.Diagnostics.Contracts;

using Il2CppTekly.Localizations;

using CppCollections = Il2CppSystem.Collections.Generic;
namespace VersusPlus;

internal sealed class LocalizationDictionary
{
    private readonly CppCollections.Dictionary<string, LocalizationString> dictionary;

    public LocalizationDictionary(
        CppCollections.Dictionary<string, LocalizationString> dictionary)
    {
        ArgumentNullException.ThrowIfNull(dictionary);
        this.dictionary = dictionary;
    }

    [Pure]
    public string Get(string id)
    {
        ArgumentNullException.ThrowIfNull(id);

        string key = GetKey(id);
        return this.dictionary[key].Format;
    }

    public void Set(string id, string format)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(format);

        string key = GetKey(id);
        this.SetByKey(key, format);
    }

    [Pure]
    public LocalizationOverride Override(string id) => new(this, id);

    [Pure]
    private static string GetKey(string id) => $"${id}";

    private void SetByKey(string key, string format)
    {
        this.dictionary[key] = new LocalizationString(key, format, null);
    }
}
