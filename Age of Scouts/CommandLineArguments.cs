using Auxiliary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Age
{
    class CommandLineArguments
    {
        public Resolution Resolution { get; private set; } = new Resolution(1024, 768);
        public DisplayModus DisplayMode { get; private set; } = DisplayModus.Windowed;
        public bool DoNotTrack { get; private set; } = true;

        public CommandLineArguments(string[] args)
        {
            if (args.Length >= 1)
            {
                string res = args[0];
                try
                {
                    string[] split = res.Split('x');
                    int w = int.Parse(split[0]);
                    int h = int.Parse(split[1]);
                    Resolution = new Resolution(w, h);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("The first commandline argument must be WIDTHxHEIGHT, e.g. 1024x768. " + ex.Message);
                }
                if (args.Length >= 2)
                {
                    switch (args[1])
                    {
                        case "fullscreen":
                            DisplayMode = DisplayModus.Fullscreen;
                            break;
                        case "window":
                            DisplayMode = DisplayModus.Windowed;
                            break;
                        case "borderless":
                            DisplayMode = DisplayModus.BorderlessWindow;
                            break;
                        default:
                            throw new ArgumentException("The second commandline argument must be 'fullscreen', 'window' or 'borderless'.");
                    }
                    if (args.Length >= 3)
                    {
                        if (args[2] == "donottrack")
                        {
                            DoNotTrack = true;
                        }
                        else if (args[2] == "trackatwill")
                        {
                            DoNotTrack = false;
                        }
                        else
                        {
                            throw new ArgumentException("The third commandline argument must be 'donottrack' or 'trackatwill'.");
                        }
                    }
                }
            }
        }
    }
}
