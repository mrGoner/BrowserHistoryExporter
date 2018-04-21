using System;
using Microsoft.Data.Sqlite;
using System.IO;

namespace BrowserHistoryExportApi
{
    public class ChromeExporter : IExporter
    {
        private readonly DateTime m_epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public string BrowserName => "Chrome";

        public HistoryCollection Export(string _pathToFile, DateTime _from, DateTime _until)
        {
            var connection = new SqliteConnection($"DataSource = {_pathToFile}");

            connection.Open();

            var exportCommand = connection.CreateCommand();
            exportCommand.CommandText =
                $"select date, url, title from " +
                "(select datetime(last_visit_time/1000000-11644473600,'unixepoch') 'date', " +
                "url, title from urls) " +
                "where date >= datetime($dateFrom, 'unixepoch') " +
                "and date <= datetime($dateUntil, 'unixepoch') order by date";

            exportCommand.Parameters.AddWithValue("$dateFrom", DateTimeToUnix(_from));
            exportCommand.Parameters.AddWithValue("$dateUntil", DateTimeToUnix(_until));

            var historyCollection = new HistoryCollection();

            using (var reader = exportCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    var date = DateTime.Parse(reader["date"].ToString());
                    var url = reader["url"].ToString();
                    var title = reader["title"].ToString();

                    historyCollection.Add(new History(url, title, date));
                }
            }

            connection.Close();

            return historyCollection;
        }

        private long DateTimeToUnix(DateTime _datetime)
        {
            return (long)(_datetime.ToUniversalTime() - m_epoch).TotalSeconds;
        }

        private DateTime UnixToDateTime(long _unixTime)
        {
            return m_epoch.AddSeconds(_unixTime);
        }
    }
}