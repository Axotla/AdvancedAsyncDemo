using System;

namespace WebSiteRetriever.Model
{
    public class WebSite
    {
        public Uri Uri { get; }

        public int SizeInBytes { get; }

        public WebSite(Uri uri, int sizeInBytes)
        {
            Uri = uri;
            SizeInBytes = sizeInBytes;
        }

        public override string ToString() =>  $"{Uri.AbsoluteUri}: {SizeInBytes:N0} bytes";
    }
}
