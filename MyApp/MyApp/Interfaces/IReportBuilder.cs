using System.ComponentModel;

namespace MyApp.Interfaces
{
    public interface IReportBuilder : INotifyPropertyChanged
    {
        string Report { get; }

        void PostHeader(string header);

        void PostFooter(string footer);

        void PostToBody(string post);

        void Reset();
    }
}