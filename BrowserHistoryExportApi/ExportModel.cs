using System;
using System.Collections.Generic;

namespace BrowserHistoryExportApi
{
    public class ExportModel
    {
        public List<Uri> Urls { get; set; }
        public List<string> Descriptions { get; set; }
    }
}