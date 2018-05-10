using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Cother
{
    /// <summary>
    /// WebClient with shorter timeout and that acts as a mobile device
    /// </summary>
    class UberWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest wrq = base.GetWebRequest(address);
            if (wrq.GetType() == typeof(HttpWebRequest))
            {
                wrq.Timeout = 5000;
                ((HttpWebRequest)wrq).ReadWriteTimeout = 5000;
                ((HttpWebRequest)wrq).UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows Phone OS 7.0; Trident/3.1; IEMobile/7.0) HTC;WP7";
            }
            return wrq;
        }
    }
    /// <summary>
    /// Data structure containing the downloaded image.
    /// </summary>
    public class GoogleImage
    {
        /// <summary>
        /// URL of the thumbnail image.
        /// </summary>
        public string ThumbnailUrl { get; private set; }
        /// <summary>
        /// URL of the full-size image.
        /// </summary>
        public string FullsizeUrl { get; private set; }
        /// <summary>
        /// Downloads the full-size image to local path.
        /// </summary>
        public void DownloadFullsizeTo(string path)
        {
            uberWebClient.DownloadFile(FullsizeUrl, path);
        }
        /// <summary>
        /// Downloads the thumbnail image to local path.
        /// </summary>
        public void DownloadThumbnailTo(string path)
        {
            uberWebClient.DownloadFile(ThumbnailUrl, path);
        }
        private GoogleImage(string thumbnail, string fullsize)
        {
            ThumbnailUrl = thumbnail;
            FullsizeUrl = fullsize;
        }
        /// <summary>
        /// Returns the full-size image url.
        /// </summary>
        public override string ToString()
        {
            return FullsizeUrl;
        }
        private static readonly UberWebClient uberWebClient = new UberWebClient();
        /// <summary>
        /// Asks the Google search engine to return images based on given query, starting at a specified image number.
        /// </summary>
        /// <param name="keywords">The search query (for example, "forest glade").</param>
        /// <param name="startat">The number of image from which to start.</param>
        /// <returns></returns>
        public static IEnumerable<GoogleImage> GetImagesForKeyword(string keywords, int startat = 1)
        {
            string page = getHtmlFromKeyword(keywords, startat);
            Regex regex = new Regex(@"http://www.google.com/imgres\?imgurl=([^&]*)&amp;[^<]*<img src=""([^""]*)""");
            var finds = regex.Matches(page);
            return
                from Match find in finds
                select 
                    new GoogleImage(find.Groups[2].Value, find.Groups[1].Value);
        }
        private static string getHtmlFromKeyword(string keywords, int startat)
        {
            return uberWebClient.DownloadString("https://www.google.com/search?q=" + keywords.Replace(" ", "+") + "&tbm=isch&start=" + startat);
        }
    }
}
