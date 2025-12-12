namespace RailwayManagementSystemAPI.ExternalServices.SystemServices.TranslationServices.Translators.Interfaces
{
    public interface IStationTranslator
    {
        string? TranslateStationTitleIntoUkrainian(string english_title);
    }
}