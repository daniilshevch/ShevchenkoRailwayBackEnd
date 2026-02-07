using RailwayManagementSystemAPI.ExternalServices.SystemServices.TranslationServices.Translators.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RailwayManagementSystemAPI.ExternalServices.SystemServices.TranslationServices.Translators.Implementations
{
    public class TicketStatusPair
    {
        [JsonPropertyName("ukrainian")]
        public string Ukrainian_Name { get; set; } = null!;
        [JsonPropertyName("english")]
        public string English_Name { get; set; } = null!;
    }
    public class TicketStatusTranslator : ITicketStatusTranslator
    {
        private readonly Dictionary<string, string> translations;
        public TicketStatusTranslator(IWebHostEnvironment web_host_environment)
        {
            string file_path =
                Path.Combine(web_host_environment.ContentRootPath, "ExternalServices",
                "SystemServices", "TranslationServices", "Dictionaries", "TicketTypes.json");
            if (File.Exists(file_path))
            {
                string json_string = File.ReadAllText(file_path);
                List<TicketStatusPair>? ticket_status_dictionary = JsonSerializer.Deserialize<List<TicketStatusPair>>(json_string);
                translations = ticket_status_dictionary is not null ? ticket_status_dictionary.ToDictionary(key => key.English_Name, value => value.Ukrainian_Name) : new Dictionary<string, string>();
            }
            else
            {
                translations = new Dictionary<string, string>();
            }
        }
        public string? TranslateTicketStatusIntoUkrainian(string english_title)
        {
            return translations.TryGetValue(english_title, out string? ukrainian_name) ? ukrainian_name : null;
        }
    }
}