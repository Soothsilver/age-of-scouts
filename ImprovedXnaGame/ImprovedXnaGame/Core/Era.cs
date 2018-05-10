using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Age.Core
{
    enum Era
    {
        EraSestniku,
        EraRadcu,
        EraVudcu,
        EraNacelniku
    }

    static class EraExtensions
    {
        public static string ToCzech(this Era era, Gender gender)
        {
            switch (gender)
            {
                case Gender.Boy:
                    switch (era)
                    {
                        case Era.EraSestniku: return "Věk šestníků";
                        case Era.EraRadcu: return "Věk rádců";
                        case Era.EraVudcu: return "Věk vůdců";
                        case Era.EraNacelniku: return "Věk náčelníků";
                    }
                    break;
                case Gender.Girl:
                    switch (era)
                    {
                        case Era.EraSestniku: return "Věk velkých světlušek";
                        case Era.EraRadcu: return "Věk rádkyň";
                        case Era.EraVudcu: return "Věk vůdkyň";
                        case Era.EraNacelniku: return "Věk náčelnic";
                    }
                    break;
            }
            return "Neznámý věk";
           
        }
    }

}
