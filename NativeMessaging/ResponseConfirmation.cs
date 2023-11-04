using System.Text.Json;
using System.Text.Json.Nodes;

namespace NativeMessaging
{
    internal class ResponseConfirmation
    {
        public string Message { get; set; }
        public JsonObject Data { get; set; }

        public ResponseConfirmation(JsonObject data)
        {
            Data = data;
            Message = "Confirmation of received data";
        }

        public JsonObject? GetJObject()
        {
            return JsonSerializer.Deserialize<JsonObject>(
                JsonSerializer.Serialize(this, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                }),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}