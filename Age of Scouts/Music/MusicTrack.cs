using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Age.Music
{
    class MusicTrack
    {
        public string Name;
        public Song Song;
        public MusicTrack(string name, Song song)
        {
            Name = name;
            Song = song;
        }
    }
}
