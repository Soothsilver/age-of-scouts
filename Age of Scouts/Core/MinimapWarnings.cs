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
            float TIME_UNTIL_STOP_WARNING = 7f;
            Tile tile = Session.Map.GetTileFromStandardCoordinates(yourEntity.FeetStdPosition);
            foreach(var w in Warnings)
            {
                if (TooClose(w, tile))
                {
                    w.StopPreventingWarningsAt = DateTime.Now.AddSeconds(TIME_UNTIL_STOP_WARNING);
                    w.StopWarningAt = DateTime.Now.AddSeconds(TIME_UNTIL_STOP_WARNING);
                    return;
                }
            }
            if (yourEntity is Building)
            {
                SFX.PlaySoundUnlessPlaying(Auxiliary.SoundEffectName.PrepadajiNasTabor);
            }
            else if (yourEntity is Unit)
            {
                SFX.PlaySoundUnlessPlaying(Auxiliary.SoundEffectName.NeprateleUtoci);
            }
            Warnings.Add(new MinimapWarning(DateTime.Now.AddSeconds(TIME_UNTIL_STOP_WARNING), DateTime.Now.AddSeconds(TIME_UNTIL_STOP_WARNING), tile.X, tile.Y));
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
