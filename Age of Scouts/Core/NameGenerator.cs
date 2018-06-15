using System;

namespace Age.Core
{
    internal static class NameGenerator
    {
        private static string[] boyNames = new[]
        {
            "Adam",
            "Aleš",
            "Tonda",
            "Artur",
            "Boris",
            "Břeťa",
            "Cyril",
            "Dan",
            "David",
            "Dominik",
            "Ed",
            "Emil",
            "Filip",
            "Hynek",
            "Ivan",
            "Ivo",
            "Jáchym",
            "Kuba",
            "Honza",
            "Jarda",
            "Jirka",
            "Pepa",
            "Karel",
            "Kryštof",
            "Libor",
            "Lukáš",
            "Marcel",
            "Marek",
            "Martin",
            "Matěj",
            "Michal",
            "Milan",
            "Mirek",
            "Ondra",
            "Patrik",
            "Pavel",
            "Petr",
            "Radek",
            "Radim",
            "Roman",
            "Sam",
            "Šimon",
            "Štěpán",
            "Tomáš",
            "Vašek",
            "Vítek",
            "Vojta",
            "Zdeněk"
        };

        internal static string GenerateBoyName()
        {
            return boyNames[R.Next(boyNames.Length)];
        }
        private static string[] katapultNames = new[]
        {
            "Ničitel světů",
            "Zhouba stanů",
            "Povoz zkázy",
            "Turboň valivý",
            "Dřevěný tank",
            "Megavůz",
            "Marshmallow",
            "Bahnonos",
            "Ronny X120",
            "Pety PT450"
        };
        internal static string GenerateKatapultName()
        {
            return katapultNames[R.Next(katapultNames.Length)];
        }
    }
}