using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using MyApp.Interfaces;
using WebSiteRetriever.Interfaces;
using WebSiteRetriever.Model;
using Xamarin.Forms;

namespace MyApp.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        private readonly IWebSiteService _service;

        private CancellationTokenSource _cancellationTokenSource;

        private double _downloadProgress;

        private ObservableCollection<WebSite> _webSites;

        public double DownloadProgress
        {
            get => _downloadProgress;
            set
            {
                if (Math.Abs(_downloadProgress - value) < 1e-3) return;

                _downloadProgress = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<WebSite> WebSites
        {
            get => _webSites;
            set
            {
                if (_webSites == value) return;
                _webSites = value;
                OnPropertyChanged();
            }
        }

        public IReportBuilder ReportBuilder { get; }

        public ICommand DownloadCommand { get; }

        public ICommand DownloadAsyncCommand { get; }

        public ICommand DownloadParallelCommand { get; }

        public ICommand DownloadParallelAsyncCommand { get; }

        public ICommand Cancel { get; }

        public MainPageViewModel(IWebSiteService service, IReportBuilder reportBuilder)
        {
            _service = service;
            ReportBuilder = reportBuilder;

            DownloadCommand = new Command(Download);
            DownloadAsyncCommand = new Command(DownloadAsync);
            DownloadParallelCommand = new Command(DownloadParallel);
            DownloadParallelAsyncCommand = new Command(DownloadParallelAsync);
            Cancel = new Command(CancelDownload);
        }

        private void Download()
        {
            // Execute the call synchronously by wrapping it into Task.Run(...).Result
            // This way we don't need a sync method for executing and timing the download
            // Just calling DownloadMethod().Result creates deadlock, hence Task.Run(...).Result
            var sites = Task.Run(() => ExecuteAndTimeAsync(_service.DownloadAllSitesAsync)).Result;

            WebSites = new ObservableCollection<WebSite>(sites);
        }

        private async void DownloadAsync()
        {
            WebSites = new ObservableCollection<WebSite>(await ExecuteAndTimeAsync(_service.DownloadAllSitesAsync));
        }

        private void DownloadParallel()
        {
            // Execute the call synchronously by wrapping it into Task.Run(...).Result
            // This way we don't need a sync method for executing and timing the download
            // Just calling DownloadMethod().Result creates deadlock, hence Task.Run(...).Result
            var sites = Task.Run(() => ExecuteAndTimeAsync(_service.DownloadAllSitesParallelAsync)).Result;

            WebSites = new ObservableCollection<WebSite>(sites);
        }

        private async void DownloadParallelAsync()
        {
            WebSites = new ObservableCollection<WebSite>(await ExecuteAndTimeAsync(_service.DownloadAllSitesParallelAsync));
        }

        private async Task<IEnumerable<WebSite>> ExecuteAndTimeAsync(Func<Progress<DownloadProgressReport>, CancellationToken, Task<IEnumerable<WebSite>>> downloadMethod)
        {
            var sw = new Stopwatch();
            sw.Start();

            DownloadProgress = 0;
            ReportBuilder.Reset();

            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;

            ReportBuilder.PostHeader("Download started...");

            var sites = await downloadMethod.Invoke(CreateProgressReporter(), cancellationToken);

            var conclusionMessage = 
                (cancellationToken.IsCancellationRequested 
                    ? "Download was cancelled." 
                    : "Download finished.") 
                + Environment.NewLine
                + $"Time elapsed: {sw.ElapsedMilliseconds:N0} ms.";

            ReportBuilder.PostFooter(conclusionMessage);

            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;

            return sites;
        }

        private void CancelDownload() =>_cancellationTokenSource?.Cancel();

        private void Report(object sender, DownloadProgressReport report)
        {
            DownloadProgress = report.Progress;

            ReportBuilder.PostToBody(report.WebSite.ToString());
        }

        private Progress<DownloadProgressReport> CreateProgressReporter()
        {
            var progress = new Progress<DownloadProgressReport>();
            progress.ProgressChanged += Report;

            return progress;
        }
    }
}
