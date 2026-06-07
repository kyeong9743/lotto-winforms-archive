using System;
using System.Data.SQLite;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace lotto
{
    public static class SharedUtils
    {
        public static string ConnectionString = "Data Source=lotto.db;Version=3;";
        public static string WebhookUrl = ""; //디스코드 웹훅 url

        public static void InitializeDatabase()
        {
            if (!File.Exists("lotto.db"))
            {
                SQLiteConnection.CreateFile("lotto.db");
            }

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string tableQuery = @"
                    CREATE TABLE IF NOT EXISTS lottodata (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        idnumber TEXT,
                        num1 INTEGER,
                        num2 INTEGER,
                        num3 INTEGER,
                        num4 INTEGER,
                        num5 INTEGER,
                        timetable TEXT
                    )";
                using (var command = new SQLiteCommand(tableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void SendWebhook(string title, string username, string lottoNums, string time, string extraInfo)
        {
            var payload = new
            {
                username = $"[ {title} ]",
                embeds = new[]
                {
                    new
                    {
                        title = title,
                        description = $"구매자: {username} \r\n\r\n입력번호: {lottoNums} \r\n\r\n판매시간: {time} \r\n\r\n\r\n\r\n{extraInfo}",
                        color = "669681",
                        footer = new
                        {
                            text = "[컴퓨터공학과]",
                            icon_url = ""
                        },
                        timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
                    }
                }
            };

            string json = JsonConvert.SerializeObject(payload);
            
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(WebhookUrl);
                request.ContentType = "application/json";
                request.Method = "POST";

                using (var writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(json);
                }

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                }
            }
            catch { }
        }
    }
}
