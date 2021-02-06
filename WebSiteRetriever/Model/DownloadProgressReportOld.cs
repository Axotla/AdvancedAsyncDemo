using System.Collections.Generic;

namespace WebSiteRetriever.Model
{
    public class DownloadProgressReportOld
    {
        public int Total { get; set; }

        public string Message { get; set; }

        public double DownloadProgress => DownloadedWebSites.Count / (double)Total;

        public IReadOnlyCollection<WebSite> DownloadedWebSites { get; set; }
    }
}
