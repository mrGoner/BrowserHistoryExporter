using System;
using System.Collections.Generic;

namespace BrowserHistoryExportApi
{
    [Serializable]
    public class ExportModel : List<KeyValuePair<Uri, string>>
    {
        
    }
}