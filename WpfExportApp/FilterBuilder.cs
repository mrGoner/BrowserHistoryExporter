using System;
using WpfExportApp.Properties;

namespace WpfExportApp
{
    public class FilterBuilder
    {
        public static string BuildLoadFilter(string[] _extentions)
        {
            if (_extentions == null)
                throw new System.ArgumentNullException(nameof(_extentions));

            string filter = Resources.SupportedFiles;

            foreach (var extention in _extentions)
            {
                filter += $"*{extention};";
            }

            return filter;
        }

        public static string BuildSaveFilter(string[] _extentions)
        {
            if (_extentions == null)
                throw new System.ArgumentNullException(nameof(_extentions));

            string filter = string.Empty;

            foreach (var extention in _extentions)
            {
                filter += $"({extention})|*{extention}|";
            }

            if (filter.EndsWith("|"))
                filter = filter.Remove(filter.LastIndexOf("|", StringComparison.Ordinal), 1);

            return filter;
        }
    }
}
