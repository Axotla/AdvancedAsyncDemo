using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebSiteRetriever.Model;

namespace WebSiteRetriever.Interfaces
{
    public interface IWebSiteService
    {
        Task<WebSite> DownloadSiteAsync(Uri address, CancellationToken cancellationToken);
        Task<IEnumerable<WebSite>> DownloadAllSitesAsync(IProgress<DownloadProgressReport> progress = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<WebSite>> DownloadAllSitesParallelAsync(IProgress<DownloadProgressReport> progress = null, CancellationToken cancellationToken = default);
    }
}