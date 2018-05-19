using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;

namespace Auxiliary
{
    public class PerformanceCounter
    {
        public static PerformanceCounter Instance = new PerformanceCounter();
        public int FPSSoFar;
        public int UPSSoFar;
        public int OPSSoFar; 
        public int OPS;
        public int FPS;
        public int UPS;
        private string upsDataSoFar;
        private string fpsDataSoFar;
        private List<PerformanceGroup> allPerformanceGroups = new List<PerformanceGroup>();
        private Dictionary<PerformanceGroup, Stopwatch> watches = new Dictionary<PerformanceGroup, Stopwatch>();
        private Dictionary<PerformanceGroup, long> total = new Dictionary<PerformanceGroup, long>();
        private Dictionary<PerformanceGroup, long> maximum = new Dictionary<PerformanceGroup, long>();

        private PerformanceCounter()
        {
            foreach (PerformanceGroup performanceGroup in (PerformanceGroup[])Enum.GetValues(typeof(PerformanceGroup)))
            {
                allPerformanceGroups.Add(performanceGroup);
                watches.Add(performanceGroup, new Stopwatch());
                total.Add(performanceGroup, 0);
                maximum.Add(performanceGroup, 0);
            }
        }
        public static void AddUPSData(string line)
        {
            Instance.upsDataSoFar += line;
        }

        public static void OtherCycleBegins()
        {
            Interlocked.Increment(ref Instance.OPSSoFar);
        }

        public static void StartMeasurement(PerformanceGroup group)
        {
            Instance.watches[group].Restart();
        }
        public static void EndMeasurement(PerformanceGroup group)
        {
            Instance.watches[group].Stop();
            Instance.total[group] += Instance.watches[group].ElapsedTicks;
        }
        
        private string fpsUpsString;
        public DateTime SecondElapsesIn = DateTime.Now;
        private string performanceGroupData;
        private bool changePerformanceGroupDataDisplayNow = true;
        public void DrawSelf(Vector2 where)
        {

            if (changePerformanceGroupDataDisplayNow)
            {
                performanceGroupData = "";
                foreach (var key in this.allPerformanceGroups)
                {
                    performanceGroupData += key + ": " + total[key] + " (max " + maximum[key] + ")\n";
                }

                changePerformanceGroupDataDisplayNow = true;
            }

            foreach (var key in this.allPerformanceGroups)
            {
                if (this.total[key] > this.maximum[key])
                {
                    this.maximum[key] = this.total[key];
                }

                if (Root.Keyboard_NewState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.F6))
                {
                    this.maximum[key] = 0;
                }
                this.total[key] = 0;
            }

            Primitives.DrawSingleLineText(fpsUpsString + "\n" + upsDataSoFar + "\n" + fpsDataSoFar + "\n" + performanceGroupData,
                new Vector2(where.X + 1 ,where.Y - 1 ), Color.Black, Library.FontTinyBold);
            Primitives.DrawSingleLineText(fpsUpsString + "\n" + upsDataSoFar + "\n" + fpsDataSoFar + "\n" + performanceGroupData,
                where, Color.White, Library.FontTinyBold);
        }
        public void DrawCycleBegins()
        {
            FPSSoFar++;
            fpsDataSoFar = "";
        }
        public void UpdateCycleBegins()
        {
            upsDataSoFar = "";
            UPSSoFar++;
            if (DateTime.Now > SecondElapsesIn)
            {
                UPS = UPSSoFar;
                FPS = FPSSoFar;
                OPS = OPSSoFar;
                UPSSoFar = 0;
                FPSSoFar = 0;
                OPSSoFar = 0;
                fpsUpsString = "FPS: "+ FPS +"; UPS: "+ UPS + "; OPS: " + OPS;
                changePerformanceGroupDataDisplayNow = true;
                SecondElapsesIn = DateTime.Now.AddSeconds(1);
            }
        }

        public static void AddFPSData(string line)
        {
            Instance.fpsDataSoFar += line;
        }
    }

    public enum PerformanceGroup
    {
        DrawCycle,
        SpriteBatchEnd,
        Minimap,
        UpdateCycle,
        GetTilesVisibleOnScreen,
        CopySession,
        FogOfWarReveal,
        AI,
        Pathfinding
    }
}
