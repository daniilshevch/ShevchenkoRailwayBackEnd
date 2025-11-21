using QRCoder;
using RailwayManagementSystemAPI.ExternalServices.SystemServices.Interfaces;
namespace RailwayManagementSystemAPI.ExternalServices.SystemServices.Implementations
{
    public class QRCodeGeneratorService : IQRCodeGeneratorService
    {
        public string GenerateQrCodeBase64(string content)
        {
            using (QRCodeGenerator qr_code_generator = new QRCodeGenerator())
            {
                QRCodeData qr_code_data = qr_code_generator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
                PngByteQRCode qr_code = new PngByteQRCode(qr_code_data);
                byte[] qr_code_bytes = qr_code.GetGraphic(20);
                string base64 = Convert.ToBase64String(qr_code_bytes);
                return $"data:image/png;base64,{base64}";
            }
        }
    }
}
