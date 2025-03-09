using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class ChatbotService
{
    private static readonly string apiKey = "API-KEY-ChatGPT";
    private static readonly string apiUrl = "https://api.openai.com/v1/chat/completions";

    public static async Task<string> GetChatbotResponse(HttpClient httpClient, string userMessage)
    {
        var requestBody = new
        {
            model = "gpt-4",
            messages = new[]
            {
                new { role = "system", content = "You are a useful assistant" },
                new { role = "user", content = userMessage }
            },
            stream = true
        };

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        var response = await httpClient.PostAsJsonAsync(apiUrl, requestBody);

        if (!response.IsSuccessStatusCode)
            return "Error in thee answer";

        using var responseStream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(responseStream);
        var responseText = new StringBuilder();

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (!string.IsNullOrWhiteSpace(line) && line.StartsWith("data:"))
            {
                var json = line.Substring(5).Trim();
                var jsonDoc = JsonDocument.Parse(json);
                responseText.Append(jsonDoc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString());
            }
        }

        return responseText.ToString();
    }
}
