using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WebSiteRetriever.Interfaces;
using WebSiteRetriever.Model;

namespace WebSiteRetriever.Services
{
    public class WebSiteService : IWebSiteService
    {
        public static readonly List<Uri> AvailableSites = new List<Uri>
        {
            new Uri("https://www.yahoo.com"),
            new Uri("https://www.google.com"),
            new Uri("https://www.microsoft.com"),
            new Uri("https://www.cnn.com"),
            new Uri("https://www.amazon.com"),
            new Uri("https://www.facebook.com"),
            new Uri("https://www.codeproject.com"),
            new Uri("https://www.stackoverflow.com"),
            new Uri("https://en.wikipedia.org/wiki/.NET_Framework")
        };

        public async Task<WebSite> DownloadSiteAsync(Uri address, CancellationToken cancellationToken)
        {
            var responseContentSizeInBytes = -1;

            var client = new HttpClient();

            try
            {
                var response = await client.GetAsync(address, cancellationToken);

                if (response.IsSuccessStatusCode)
                    responseContentSizeInBytes = (await response.Content.ReadAsByteArrayAsync()).Length;
            }
            catch
            {
                // We don't really care if download fails at this point and how/why it fails
                // so just keep responseContentSizeInBytes at -1 indicating that download failed.
            }

            return new WebSite(address, responseContentSizeInBytes);
        }

        public async Task<IEnumerable<WebSite>> DownloadAllSitesAsync(IProgress<DownloadProgressReport> progress = null, CancellationToken cancellationToken = default)
        {
            var result = new List<WebSite>();

            var progressScale =  1.0 / AvailableSites.Count;

            foreach (var uri in AvailableSites)
            {
                var site = await DownloadSiteAsync(uri, cancellationToken);

                if (cancellationToken.IsCancellationRequested) break;

                result.Add(site);

                progress?.Report(new DownloadProgressReport
                {
                    WebSite = site,
                    Progress = result.Count * progressScale
                });
            }

            return result;
        } 

        public async Task<IEnumerable<WebSite>> DownloadAllSitesParallelAsync(IProgress<DownloadProgressReport> progress = null, CancellationToken cancellationToken = default)
        {
            var sites = new ConcurrentQueue<WebSite>();

            var progressScale = 1.0 / AvailableSites.Count;

            var tasks = new Task[AvailableSites.Count];

            // Parallel.ForEach solution required wrapping DownloadSiteAsync into another Task.Run().Result,
            // as without .Result (i.e. with await) threads would allow the ViewModel to continue as if
            // the download was finished, messing up the timing and output to UI.
            // By using Task array we can actually wait until all tasks are actually finished by 
            // using await Task.WhenAll(tasks).
            var idx = 0;
            foreach (var uri in AvailableSites)
            {
                tasks[idx++] = Task.Run(async () =>
                {
                    var site = await DownloadSiteAsync(uri, cancellationToken);

                    if (cancellationToken.IsCancellationRequested) return;

                    sites.Enqueue(site);

                    progress?.Report(new DownloadProgressReport
                    {
                        WebSite = site,
                        Progress = sites.Count * progressScale
                    });
                }, cancellationToken);
            }

            await Task.WhenAll(tasks);

            return sites;
        } 
    }
}
