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

        public OtherThreads(LevelPhase levelPhase)
        {
            this.levelPhase = levelPhase;
            sessionUsedInOtherThreads = new Session();
            sessionModifiedFromRealtime = new Session();
        }
        
        public void StartWorking(Session session)
        {
            sessionUsedInOtherThreads.CopyValuesFrom(session);
            SafeCopyInRealtime(session, 0);
            Process process = Process.GetCurrentProcess();
            int tcountBefore = process.Threads.Count;
          //  var memory = RememberAllThreads(process);

            Thread anotherThread = new Thread(FullOuterThread);
            anotherThread.IsBackground = true;
            anotherThread.Start();

            process = Process.GetCurrentProcess();
            int tcount = process.Threads.Count;
            System.Diagnostics.Debug.Print("TCOUNT: " + tcount + " > " + tcountBefore);
            ProcessThread newThread = null;
            var field = typeof(Thread).GetField("DONT_USE_InternalThread",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            Object value = field.GetValue(anotherThread);
            IntPtr pointer = (IntPtr) value;
            var nativeId = System.Runtime.InteropServices.Marshal.ReadInt32(pointer, 0x0160);
            for (int ti = 0; ti < tcount; ti++)
            {
                var t = process.Threads[ti];
                if (t.Id == (int) nativeId)
                {
                    newThread = process.Threads[ti];
                    break;
                }
            }
            int processorCount = Environment.ProcessorCount;
            int targetProcessor = 2;
            if (newThread != null)
            {
                newThread.IdealProcessor = targetProcessor;
                //newThread.ProcessorAffinity = (IntPtr)(1 << (targetProcessor -1));
            }
            else
            {
                throw new Exception("Did not find created thread.");
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

            FogOfWarMechanics.PerformFogOfWarReveal(sessionUsedInOtherThreads, elapsedSeconds);
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