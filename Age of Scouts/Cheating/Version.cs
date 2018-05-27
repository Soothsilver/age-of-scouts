using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Age.Debug
{
    static class Version
    {
        static Version()
        {
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            AsString = version.Major + "." + version.Minor + "." + version.Build;
        }
        public static string AsString { get; private set; }
    }
}
