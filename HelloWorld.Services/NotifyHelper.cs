using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace HelloWorld.Services
{
    public static class NotifyHelper
    {
        private static readonly HttpClient httpClient = new HttpClient();
        public async static Task<bool> CheckStatusUpdated(string responsePath)
        {
            var responseContent = string.Empty;
            using (StreamReader r = new StreamReader(responsePath))
            {
                responseContent = r.ReadToEnd();
            }
            if (!string.IsNullOrEmpty(responseContent))
            {
                using MultipartFormDataContent multipartContent = new MultipartFormDataContent()
                    {
                        { new StringContent("C240135209", Encoding.UTF8, MediaTypeNames.Text.Plain), "claim_no" },
                        { new StringContent("0825524693", Encoding.UTF8, MediaTypeNames.Text.Plain), "tel" },
                        { new StringContent("saka", Encoding.UTF8, MediaTypeNames.Text.Plain), "saka" }
                    };
                using var response = await httpClient.PostAsync("https://www.advice.co.th/claimstatus/getClaim", multipartContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseJSON = await response.Content.ReadAsStringAsync();
                    var result = Minify(responseContent) == Minify(responseJSON);
                    if (!result)
                    {
                        File.WriteAllText("Response/response.json", responseJSON);
                        return true;
                    }
                }
            }
            return false;
        }

        private static string Minify(string json)
        {
            var options =
                new JsonWriterOptions
                {
                    Indented = false,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
            using var document = JsonDocument.Parse(json);
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream, options);
            document.WriteTo(writer);
            writer.Flush();
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        public async static Task NotifyLine(string message)
        {
            using MultipartFormDataContent multipartContent = new MultipartFormDataContent()
                    {
                        { new StringContent(message, Encoding.UTF8, MediaTypeNames.Text.Plain), "message" },
                    };
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "cckXcpd5ZwV0k9d7jnGulLv8lCdbrG4mZEG5eydA5LH");
            using var response = await httpClient.PostAsync("https://notify-api.line.me/api/notify", multipartContent);
        }
    }
}
