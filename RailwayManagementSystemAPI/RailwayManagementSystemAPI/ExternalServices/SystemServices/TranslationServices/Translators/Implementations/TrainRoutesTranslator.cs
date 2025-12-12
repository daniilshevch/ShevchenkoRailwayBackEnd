using RailwayManagementSystemAPI.ExternalServices.SystemServices.TranslationServices.Translators.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RailwayManagementSystemAPI.ExternalServices.SystemServices.TranslationServices.Translators.Implementations
{
    public class TrainRoutesTranslator : ITrainRoutesTranslator
    {
        private readonly Dictionary<string, string> translations;
        public TrainRoutesTranslator(IWebHostEnvironment web_host_environment)
        {
            string file_path =
                Path.Combine(web_host_environment.ContentRootPath, "ExternalServices",
                "SystemServices", "TranslationServices", "Dictionaries", "TrainRoutesLetterCodes.json");
            if (File.Exists(file_path))
            {
                string json_string = File.ReadAllText(file_path);
                JsonSerializerOptions options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                Dictionary<string, string>? dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(json_string, options);
                translations = dictionary ?? new Dictionary<string, string>();
            }
            else
            {
                translations = new Dictionary<string, string>();
            }
        }
        public string? TranslateTrainRouteIdIntoUkrainian(string english_train_route_id)
        {
            if (string.IsNullOrWhiteSpace(english_train_route_id))
            {
                return english_train_route_id;
            }
            int split_index = -1;
            for (int i = 0; i < english_train_route_id.Length; i++)
            {
                if (!char.IsDigit(english_train_route_id[i]))
                {
                    split_index = i;
                    break;
                }
            }

            if (split_index == -1)
            {
                return english_train_route_id;
            }

            string number_part = english_train_route_id[..split_index];
            string letter_part = english_train_route_id[split_index..];

            if (translations.TryGetValue(letter_part, out string? ukrainian_letter))
            {
                return number_part + ukrainian_letter;
            }
            return english_train_route_id;
        }
    }
}
