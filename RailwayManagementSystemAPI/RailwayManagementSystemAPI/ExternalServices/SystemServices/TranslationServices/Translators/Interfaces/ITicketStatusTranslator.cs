namespace RailwayManagementSystemAPI.ExternalServices.SystemServices.TranslationServices.Translators.Interfaces
{
    public interface ITicketStatusTranslator
    {
        string? TranslateTicketStatusIntoUkrainian(string english_title);
    }
}