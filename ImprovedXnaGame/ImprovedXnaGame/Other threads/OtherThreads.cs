using System.Threading;
using System.Threading.Tasks;
using Age.Core;

namespace Age.Phases
{
    class OtherThreads
    {
        private object sessionModificationLock = new object();
        private Session sessionModifiedFromRealtime;
        private volatile bool terminateSelf = false;
        private Session sessionUsedInOtherThreads;

        public OtherThreads()
        {
            sessionUsedInOtherThreads = new Session();
            sessionModifiedFromRealtime = new Session();
        }

        public void StartWorking()
        {
            Task.Factory.StartNew(() =>
            {
                while (!terminateSelf)
                {
                    OtherThreadCycle();
                }
            });
        }

        public void EndWorking()
        {
            terminateSelf = true;
        }

        public void OtherThreadCycle()
        {
            lock (sessionModificationLock)
            {
                Session swap = sessionUsedInOtherThreads;
                sessionUsedInOtherThreads = sessionModifiedFromRealtime;
                sessionModifiedFromRealtime = swap;
            }

        }

        public void UpdateCycleBegins(Session session)
        {
            lock (sessionModificationLock)
            {
                sessionModifiedFromRealtime.CopyValuesFrom(session);
            }
        }
    }
}