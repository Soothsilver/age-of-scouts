using Age.Phases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Age.Core
{
    class MinimapWarnings
    {
        private Session Session;
        public List<MinimapWarning> Warnings = new List<MinimapWarning>(); 

        public MinimapWarnings(Session session)
        {
            this.Session = session;
        }

        internal void Emit(AttackableEntity yourEntity)
        {
            Tile tile = Session.Map.GetTileFromStandardCoordinates(yourEntity.FeetStdPosition);
            foreach(var w in Warnings)
            {
                if (TooClose(w, tile))
                {
                    return;
                }
            }
            Warnings.Add(new MinimapWarning(DateTime.Now.AddSeconds(7), DateTime.Now.AddSeconds(30), tile.X, tile.Y));
        }

        private bool TooClose(MinimapWarning w, Tile tile)
        {
            return Math.Abs(w.TileX - tile.X) + Math.Abs(w.TileY - tile.Y) < 8; 
        }

        public class MinimapWarning
        {
            public DateTime StopWarningAt;
            public DateTime StopPreventingWarningsAt;
            public int TileX;
            public int TileY;

            public MinimapWarning(DateTime stopWarningAt, DateTime stopPreventingWarningsAt, int tileX, int tileY)
            {
                StopWarningAt = stopWarningAt;
                StopPreventingWarningsAt = stopPreventingWarningsAt;
                TileX = tileX;
                TileY = tileY;
            }
        }
    }
}
