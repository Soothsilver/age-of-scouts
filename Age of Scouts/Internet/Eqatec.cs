using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Age.Core;

namespace Age.Internet
{
    class Eqatec
    {
        public const string FEEDBACK = "FEEDBACK";

        private static Eqatec instance;
        private volatile int onlineWorkerInProgress;
        private bool DoNotTrack;
        private BackgroundWorker OnlineWorker = new BackgroundWorker();
        private System.Collections.Concurrent.ConcurrentQueue<EqatecMessage> OutboxMessages = new System.Collections.Concurrent.ConcurrentQueue<EqatecMessage>();
        
        private Eqatec()
        {
            // singleton
        }

        public static void Start(bool doNotTrack)
        {
            instance = new Eqatec();
            instance.OnlineWorker.DoWork += instance.OnlineWorker_DoWork;
            instance.DoNotTrack = doNotTrack;
        }

        public static void ScheduleSendMessage(string key, string data)
        {
            if (instance.DoNotTrack && key != FEEDBACK)
            {
                return;
            }
            instance.OutboxMessages.Enqueue(new EqatecMessage(key, data));
            if (Interlocked.CompareExchange(ref instance.onlineWorkerInProgress, 1, 0) == 0)
            {
                instance.OnlineWorker.RunWorkerAsync();
            }
        }

        private void OnlineWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                while (OutboxMessages.TryDequeue(out EqatecMessage msg))
                {
                    WebClient wc = new WebClient();
                    wc.DownloadString("https://hudecekpetr.cz/other/eqatec.php?key=" + Sanitize(msg.Key) + "&data=" + Sanitize(msg.Data) + "&game=AoS");
                }
            }
            catch
            {

            }
            instance.onlineWorkerInProgress = 0;
        }

        internal static string Identify(Session session)
        {
            return session.LevelName + " (" + session.Map.Width + "x" + session.Map.Height + ")";
        }

        private string Sanitize(string data)
        {
            return WebUtility.UrlEncode(data);
        }
    }

    class EqatecMessage
    {
        public string Key { get; }
        public string Data { get; }
        public EqatecMessage(string key, string data)
        {
            Key = key;
            Data = data;
        }
    }
}
