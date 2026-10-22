namespace VersusPlus.Helper;
// RaptorRush
using System.Diagnostics.Contracts;

using Il2CppReloaded;
using Il2CppReloaded.Services;

using Il2CppSystem.Reflection;

using Il2CppTekly.Localizations;

using CppCollections = Il2CppSystem.Collections.Generic;

internal static class SeedPacketToolTipApi
{
    public static IReadOnlyList<SeedPacketToolTipOverride> ToolTipsOverrides => field ??= GetToolTipsOverrides();

    [Pure]
    private static IReadOnlyList<SeedPacketToolTipOverride> GetToolTipsOverrides()
    {
        var dataService = AppCore.GetService<IDataService>();

        var dictionary = GetLocalizationDictionary();

        return [.. dataService.PlantDefinitions.AsEnumerable()
            .Where(p => !string.IsNullOrEmpty(p.PlantToolTip))
            .Select(p => new SeedPacketToolTipOverride(dictionary.Override(p.PlantToolTip), p)),];
    }

    [Pure]
    private static LocalizationDictionary GetLocalizationDictionary()
    {
        var localizerType = Il2CppSystem.Type.GetType("Tekly.Localizations.Localizer, Tekly.Localizations.Runtime")
            ?? throw new TypeLoadException("Failed to get Localizer type.");

        const string InstancePropertyName = nameof(Localizer.Instance);
        var instanceProperty = localizerType.BaseType.GetProperty(InstancePropertyName)
            ?? throw new MissingMemberException(localizerType.BaseType.FullName, InstancePropertyName);

        const string StringsFieldName = nameof(Localizer.m_strings);
        var stringsField = localizerType.GetField(StringsFieldName, BindingFlags.NonPublic | BindingFlags.Instance)
            ?? throw new MissingFieldException(localizerType.FullName, StringsFieldName);

        var localizerInstance = instanceProperty.GetValue(obj: null);

        var dictionary = stringsField.GetValue(localizerInstance)
            .Cast<CppCollections.Dictionary<string, LocalizationString>>();

        return new(dictionary);
    }
}
