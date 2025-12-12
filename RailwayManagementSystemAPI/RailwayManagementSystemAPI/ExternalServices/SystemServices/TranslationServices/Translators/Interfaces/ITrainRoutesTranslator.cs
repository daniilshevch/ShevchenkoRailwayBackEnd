namespace RailwayManagementSystemAPI.ExternalServices.SystemServices.TranslationServices.Translators.Interfaces
{
    public interface ITrainRoutesTranslator
    {
        string? TranslateTrainRouteIdIntoUkrainian(string english_train_route_id);
    }
}