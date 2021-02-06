using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using MyApp.Annotations;
using MyApp.Interfaces;

namespace MyApp.Helpers
{
    /// <summary>
    /// Used to generate strings for reporting progress to UI.
    /// Designed to ensure thread-safety, when multiple threads
    /// spawn reports that need to be collected and outputted to UI.
    /// </summary>
    public class ReportBuilder : IReportBuilder
    {
        private string _header;

        private ConcurrentQueue<string> _bodyPosts;

        private string _footer;

        public string Report => ToString();

        public ReportBuilder()
        {
            Reset();
        }

        public void PostHeader(string header)
        {
            _header = header;
            OnPropertyChanged(nameof(Report));
        }

        public void PostFooter(string footer)
        {
            _footer = footer;
            OnPropertyChanged(nameof(Report));
        }

        public void PostToBody(string post)
        {
            _bodyPosts.Enqueue(post);
            OnPropertyChanged(nameof(Report));
        }

        public void Reset()
        {
            _header = null;
            _bodyPosts = new ConcurrentQueue<string>();
            _footer = null;
            OnPropertyChanged(nameof(Report));
        }

        public override string ToString()
        {
            return Build();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string Build()
        {
            var sb = new StringBuilder();

            if (_header != null) sb.AppendLine(_header);

            foreach (var post in _bodyPosts)
            {
                sb.AppendLine(post);
            }

            if (_footer != null) sb.AppendLine(_footer);

            return sb.ToString();
        }
    }
}
