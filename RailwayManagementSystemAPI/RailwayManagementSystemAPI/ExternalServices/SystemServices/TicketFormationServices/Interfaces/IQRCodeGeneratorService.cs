namespace RailwayManagementSystemAPI.ExternalServices.SystemServices.TicketFormationServices.Interfaces
{
    public interface IQRCodeGeneratorService
    {
        string GenerateQrCodeBase64(string content);
    }
}