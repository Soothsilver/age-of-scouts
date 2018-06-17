using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Age.Core;
using Auxiliary;
using PerformanceCounter = Auxiliary.PerformanceCounter;

namespace Age.Phases
{
    class OtherThreads
    {
        private readonly LevelPhase levelPhase;
        private object sessionModificationLock = new object();
        private Session sessionModifiedFromRealtime;
        private volatile bool terminateSelf = false;
        private Session sessionUsedInOtherThreads;
        bool firstTimeLaunch;

        public OtherThreads(LevelPhase levelPhase)
        {
            this.levelPhase = levelPhase;
            sessionUsedInOtherThreads = new Session();
            sessionModifiedFromRealtime = new Session();
        }
        
        public void StartWorking(Session session)
        {
            firstTimeLaunch = true;
            sessionUsedInOtherThreads.CopyValuesFrom(session);
            SafeCopyInRealtime(session, 0);
            Process process = Process.GetCurrentProcess();
            int tcountBefore = process.Threads.Count;
            var memory = RememberAllThreads(process);

            Thread anotherThread = new Thread(FullOuterThread)
            {
                IsBackground = true
            };
            anotherThread.Start();

            process = Process.GetCurrentProcess();
            int tcount = process.Threads.Count;
            System.Diagnostics.Debug.Print("TCOUNT: " + tcount + " > " + tcountBefore);
            int processorCount = Environment.ProcessorCount;
            int targetProcessor = 2;

            for (int ti = 0; ti < tcount; ti++)
            {
                var t = process.Threads[ti];
                if (memory.Contains(t.Id))
                {
                    memory.Remove(t.Id);
                }
                else
                {
                    t.IdealProcessor = targetProcessor;
                    if (targetProcessor < processorCount)
                    {
                        t.ProcessorAffinity = (IntPtr)(1 << (targetProcessor - 1));
                    }
                    break;
                }
            }
        }

        private HashSet<int> RememberAllThreads(Process process)
        {
            HashSet<int> memory = new HashSet<int>();
            int tcount = process.Threads.Count;
            for (int ti = 0; ti < tcount; ti++)
            {
                var t = process.Threads[ti];
                memory.Add(t.Id);
            }

            return memory;
        }

        [HandleProcessCorruptedStateExceptions]
        private void FullOuterThread()
        {
            try
            {
                while (!terminateSelf)
                {
                    OtherThreadCycle();
                }
            }
            catch (Exception ex) when (ex is AccessViolationException || ex is ObjectDisposedException)
            {
                // Shikata ga nai...
            }
        }

        public void EndWorking()
        {
            terminateSelf = true;
        }

        float elapsedSecondsSinceLastCycle = 0;
        object elapsedSecondsLock = new object();

        public void OtherThreadCycle()
        {
            PerformanceCounter.OtherCycleBegins();
            float elapsedSeconds = 0;
            lock (elapsedSecondsLock)
            {
                elapsedSeconds = elapsedSecondsSinceLastCycle;
                elapsedSecondsSinceLastCycle = 0;
            }
            lock (sessionModificationLock)
            {
                Session swap = sessionUsedInOtherThreads;
                sessionUsedInOtherThreads = sessionModifiedFromRealtime;
                sessionModifiedFromRealtime = swap;
            }

            FogOfWarMechanics.PerformFogOfWarReveal(sessionUsedInOtherThreads, elapsedSeconds, firstTimeLaunch);
            firstTimeLaunch = false;
            levelPhase.Minimap.UpdateTexture(sessionUsedInOtherThreads.Map);
            
        }

        public static volatile int counter = 0;

        public void UpdateCycleBegins(Session session, float elapsedSeconds)
        {
            PerformanceCounter.StartMeasurement(PerformanceGroup.CopySession);
            SafeCopyInRealtime(session, elapsedSeconds);
            PerformanceCounter.EndMeasurement(PerformanceGroup.CopySession);
        }

        private void SafeCopyInRealtime(Session session, float elapsedSeconds)
        {
            lock (elapsedSecondsLock)
            {
                elapsedSecondsSinceLastCycle += elapsedSeconds;
            }
            lock (sessionModificationLock)
            {
                sessionModifiedFromRealtime.CopyValuesFrom(session);
            }
            FogOfWarMechanics.AcceptRevealChanges(session);
        }
    }
}