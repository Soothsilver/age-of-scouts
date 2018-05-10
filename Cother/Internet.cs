using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Net;

namespace Cother
{
    /// <summary>
    /// Simple class that allows you to ping internet addresses to, for example, report usage statistics.
    /// </summary>
    public static class Internet
    {
        private static readonly object internetMutex = new object();
        private static readonly List<Uri> urisToRequestNextBurst = new List<Uri>();

        /// <summary>
        /// Adds a request to access the specified URI.
        /// </summary>
        /// <param name="uri">The URI to access.</param>
        /// <param name="sendImmediately">Call SendInternetBurst() immediately.</param>
        public static void SendCommand(Uri uri, bool sendImmediately = true)
        {
            lock (internetMutex)
            {
                urisToRequestNextBurst.Add(uri);
            }
            if (sendImmediately)
            {
                SendInternetBurst();
            }
        }
        /// <summary>
        /// Call all URIs specified via SendCommand now, in order, asynchronously (starts a new thread).
        /// </summary>
        public static void SendInternetBurst()
        {
            lock (internetMutex)
            {
                List<Uri> currentBurstRequestedUris = urisToRequestNextBurst.ToList();
                urisToRequestNextBurst.Clear();
                if (currentBurstRequestedUris.Count > 0)
                {
                    Thread thread = new Thread(requestUris);
                    thread.Start(currentBurstRequestedUris);
                }
            }
        }

        /// <summary>
        /// This method is always run in a background thread. It does not report success or failure.
        /// </summary>
        /// <param name="listOfUris">Must be List&lt;Uri&gt;. These addresses will be pinged.</param>
        /// <exception cref="ArgumentException"></exception>
        private static void requestUris(object listOfUris)
        {
            try
            {
                List<Uri> list = (List<Uri>)listOfUris;
                foreach (Uri uri in list)
                {
                    try
                    {
                        WebClient wc = new WebClient();
                        wc.DownloadString(uri.ToString());
                    }
                    catch
                    {
                        // Suppress HTTP error.
                        errorOccuredInThread();
                    }
                }
            }
            catch
            {
                throw new ArgumentException("Function parameters must be a List<Uri>.", "listOfUris");
            }
        }

        private static void errorOccuredInThread()
        {
        }
    }
}
