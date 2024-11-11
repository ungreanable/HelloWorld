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
using System.Text.RegularExpressions;
using HtmlAgilityPack;

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
                var parameters = new Dictionary<string, string>
                {
                    { "ctl00$ScriptManager1", "ctl00$ContentPlaceHolder1$UpdatePanel1|ctl00$ContentPlaceHolder1$ButtonCheckdetail" },
                    { "ctl00$ddlLanguage", "TH" },
                    { "ctl00$ContentPlaceHolder1$tbNumberRepaire", "SN233641047442" },
                    { "ctl00$ContentPlaceHolder1$tbName", "" },
                    { "ctl00$ContentPlaceHolder1$tbTelephone", "" },
                    { "ctl00$ContentPlaceHolder1$tbCardNumbe", "" },
                    { "__EVENTTARGET", "" },
                    { "__EVENTARGUMENT", "" },
                    { "__LASTFOCUS", "" },
                    { "__VIEWSTATE", "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" },
                    { "__VIEWSTATEGENERATOR", "60E354D5" },
                    { "__EVENTVALIDATION", "Oy8JHdhDUsauqPJ1JMothjwSoRFcpMR2Ufl4oV+6j+K7n4lRYQrj5rujnKjuwvwjuyqCX4o83K9zZn4Mjo8YARdHQim2masETOxsM98EcpwrgiyZEh0hP6vEWr4h5wUJgN/nKjs8kI8/TKXmFutUkhltanX2q2OTmS7GiQH2cRx25yDinnqrPrx0KGfRbcYFJ8Xle7Qhquhf5PHZBcuOwKgkuMyGygf9+CWPZpfdkAXgFjqUp6YXvANpTl4CJWRyex1k2luWzYPZhIi0+OFyBMeK/tKWwvF6geNQF6+R+roowqUooSvQ4vSBmK6yvAbHthfi9xfx6SDSHxLh/iCdkA==" },
                    { "__ASYNCPOST", "true" },
                    { "ctl00$ContentPlaceHolder1$ButtonCheckdetail", "ตรวจสอบรายละเอียด" }
                };
                httpClient.DefaultRequestHeaders.Clear();	
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-microsoftajax", "Delta=true");	
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-requested-with", "XMLHttpRequest");	
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36");	
                using var response = await httpClient.PostAsync("https://www.synnex.co.th/Th/Components/Service/DetailRepaire", new FormUrlEncodedContent(parameters));

                if (response.IsSuccessStatusCode)
                {
                    var responseHTML = await response.Content.ReadAsStringAsync();
                    
                    var minifyResponse = MinifyHtml(responseHTML);
                    string jobStatus = GetSpanContentById(minifyResponse, "ContentPlaceHolder1_LabelJobStatas");
                    if (jobStatus != responseContent)
                    {
                        File.WriteAllText("Response/response.json", jobStatus);
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

        private static string MinifyHtml(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
                return html;

            // ลบช่องว่างที่ไม่จำเป็นและบรรทัดใหม่
            html = Regex.Replace(html, @"\s{2,}", " ");
            html = Regex.Replace(html, @">\s+<", "><"); // ลบช่องว่างระหว่าง tag
            html = Regex.Replace(html, @"<!--(.*?)-->", ""); // ลบคอมเมนต์ HTML

            return html.Trim();
        }

        private static string GetSpanContentById(string html, string spanId)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            // ค้นหา span ที่มี id ตรงกับที่ระบุ
            var spanNode = htmlDoc.DocumentNode.SelectSingleNode($"//span[@id='{spanId}']");

            // ตรวจสอบและคืนค่าข้อความภายใน span
            return spanNode?.InnerText ?? "ไม่พบ span ที่มี id นี้";
        }

        public async static Task NotifyLine(string message)
        {
            using MultipartFormDataContent multipartContent = new MultipartFormDataContent()
                    {
                        { new StringContent(message, Encoding.UTF8, MediaTypeNames.Text.Plain), "message" },
                    };
			httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "cckXcpd5ZwV0k9d7jnGulLv8lCdbrG4mZEG5eydA5LH");
            using var response = await httpClient.PostAsync("https://notify-api.line.me/api/notify", multipartContent);
        }
    }
}
