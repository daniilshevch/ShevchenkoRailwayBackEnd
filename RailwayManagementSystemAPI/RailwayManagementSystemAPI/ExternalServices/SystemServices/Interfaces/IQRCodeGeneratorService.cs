namespace RailwayManagementSystemAPI.ExternalServices.SystemServices.Interfaces
{
    public interface IQRCodeGeneratorService
    {
        string GenerateQrCodeBase64(string content);
    }
}