using System;
using Microsoft.Data.Sqlite;
using System.IO;
using System.Collections.Generic;

namespace BrowserHistoryExportApi
{
    public class ChromeExporter : IExporter
    {
        private DateTime m_epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public string BrowserName => "Chrome";

        public ExportModel Export(string _pathToFile, DateTime _from, DateTime _until)
        {
            if (File.Exists(_pathToFile))
            {
                var connection = new SqliteConnection($"DataSource = {_pathToFile}");

                connection.Open();

                var exportCommand = connection.CreateCommand();
                exportCommand.CommandText = 
                    $"Select url, title from urls " +
                    "where last_visit_time >= $dateFrom and last_visit_time <= $dateUntil";
                exportCommand.Parameters.AddWithValue("$dateFrom", DateTimeToUnix(_from));
                exportCommand.Parameters.AddWithValue("$dateUntil", DateTimeToUnix(_until));

                var exportModel = new ExportModel();

                using(var reader = exportCommand.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        var url = new Uri(reader["url"].ToString());
                        var title = reader["title"].ToString();

                        exportModel.Add(new KeyValuePair<Uri, string>(url, title));
                    }
                }

                return exportModel;
            }
            else
            {
                throw new FileNotFoundException($"DataBase {_pathToFile} not found!");  
            }
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