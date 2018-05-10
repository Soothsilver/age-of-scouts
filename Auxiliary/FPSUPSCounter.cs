using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Auxiliary
{
    public class PerformanceCounter
    {
        public static PerformanceCounter Instance = new PerformanceCounter();
        public int FPSSoFar;
        public int UPSSoFar;
        public int FPS;
        public int UPS;
        private string upsDataSoFar;
        private string fpsDataSoFar;
        public static void AddUPSData(string line)
        {
            Instance.upsDataSoFar += line;
        }

        private string fpsUpsString;
        public DateTime SecondElapsesIn = DateTime.Now;

        public void DrawSelf(Vector2 where)
        {

            Primitives.DrawSingleLineText(fpsUpsString + "\n" + upsDataSoFar + "\n" + fpsDataSoFar,
                new Vector2(where.X +1 ,where.Y - 1), Color.Black, Library.FontTinyBold);
            Primitives.DrawSingleLineText(fpsUpsString + "\n" + upsDataSoFar + "\n" + fpsDataSoFar,
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
                UPSSoFar = 0;
                FPSSoFar = 0;
                fpsUpsString = "FPS: "+ FPS +"; UPS: "+ UPS;
                SecondElapsesIn = DateTime.Now.AddSeconds(1);
            }
        }

        public static void AddFPSData(string line)
        {
            Instance.fpsDataSoFar += line;
        }
    }
}
