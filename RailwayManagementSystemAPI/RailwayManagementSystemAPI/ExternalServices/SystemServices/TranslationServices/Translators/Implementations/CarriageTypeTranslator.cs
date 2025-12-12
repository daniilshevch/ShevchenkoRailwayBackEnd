using RailwayManagementSystemAPI.ExternalServices.SystemServices.TranslationServices.Translators.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RailwayManagementSystemAPI.ExternalServices.SystemServices.TranslationServices.Translators.Implementations
{
    public class CarriageTypePair
    {
        [JsonPropertyName("ukrainian")]
        public string Ukrainian_Name { get; set; } = null!;
        [JsonPropertyName("english")]
        public string English_Name { get; set; } = null!;
    }
    public class CarriageTypeTranslator : ICarriageTypeTranslator
    {
        private readonly Dictionary<string, string> translations;
        public CarriageTypeTranslator(IWebHostEnvironment web_host_environment)
        {
            string file_path =
                Path.Combine(web_host_environment.ContentRootPath, "ExternalServices",
                "SystemServices", "TranslationServices", "Dictionaries", "CarriageTypes.json");
            if (File.Exists(file_path))
            {
                string json_string = File.ReadAllText(file_path);
                List<CarriageTypePair>? carriage_type_dictionary = JsonSerializer.Deserialize<List<CarriageTypePair>>(json_string);
                translations = carriage_type_dictionary is not null ? carriage_type_dictionary.ToDictionary(key => key.English_Name, value => value.Ukrainian_Name) : new Dictionary<string, string>();
            }
            else
            {
                translations = new Dictionary<string, string>();
            }
        }
        public string? TranslateCarriageTypeIntoUkrainian(string english_title)
        {
            return translations.TryGetValue(english_title, out string? ukrainian_name) ? ukrainian_name : null;
        }
    }
}
