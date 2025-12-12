using Microsoft.AspNetCore;
using RailwayManagementSystemAPI.ExternalServices.SystemServices.TranslationServices.Translators.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RailwayManagementSystemAPI.ExternalServices.SystemServices.TranslationServices.Translators.Implementations
{
    public class StationNamePair
    {
        [JsonPropertyName("ukrainian")]
        public string Ukrainian_Name { get; set; } = null!;
        [JsonPropertyName("english")]
        public string English_Name { get; set; } = null!;
    }
    public class StationTranslator : IStationTranslator
    {
        private readonly Dictionary<string, string> translations;
        public StationTranslator(IWebHostEnvironment web_host_environment)
        {
            string file_path =
                Path.Combine(web_host_environment.ContentRootPath, "ExternalServices",
                "SystemServices", "TranslationServices", "Dictionaries", "StationNames.json");
            if (File.Exists(file_path))
            {
                string json_string = File.ReadAllText(file_path);
                List<StationNamePair>? stations_dictionary = JsonSerializer.Deserialize<List<StationNamePair>>(json_string);
                translations = stations_dictionary is not null ? stations_dictionary.ToDictionary(key => key.English_Name, value => value.Ukrainian_Name) : new Dictionary<string, string>();
            }
            else
            {
                translations = new Dictionary<string, string>();
                Console.WriteLine("Not found");
            }
        }
        public string? TranslateStationTitleIntoUkrainian(string english_title)
        {
            return translations.TryGetValue(english_title, out string? ukrainian_name) ? ukrainian_name : null;
        }
    }
}
