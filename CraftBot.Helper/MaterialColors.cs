using DSharpPlus.Entities;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CraftBot.Helper
{
    public enum MaterialColorShade
    {
        N50,
        N100,
        N200,
        N300,
        N400,
        N500,
        N600,
        N700,
        N800,
        N900,
        A100,
        A200,
        A400,
        A700
    }

    public static class MaterialColors
    {
        public static readonly ReadOnlyDictionary<MaterialColorShade, DiscordColor> Amber = new ReadOnlyDictionary<MaterialColorShade, DiscordColor>(new Dictionary<MaterialColorShade, DiscordColor>()
        {
            { MaterialColorShade.N50, new DiscordColor("#FFEBEE") },
            { MaterialColorShade.N100, new DiscordColor("#FFCDD2") },
            { MaterialColorShade.N200, new DiscordColor("#EF9A9A") },
            { MaterialColorShade.N300, new DiscordColor("#E57373") },
            { MaterialColorShade.N400, new DiscordColor("#EF5350") },
            { MaterialColorShade.N500, new DiscordColor("#F44336") },
            { MaterialColorShade.N600, new DiscordColor("#E53935") },
            { MaterialColorShade.N700, new DiscordColor("#D32F2F") },
            { MaterialColorShade.N800, new DiscordColor("#C62828") },
            { MaterialColorShade.N900, new DiscordColor("#B71C1C") },
            { MaterialColorShade.A100, new DiscordColor("#FF8A80") },
            { MaterialColorShade.A200, new DiscordColor("#FF5252") },
            { MaterialColorShade.A400, new DiscordColor("#FF1744") },
            { MaterialColorShade.A700, new DiscordColor("#D50000") }
        });

        public static readonly DiscordColor Black = new DiscordColor("#000000");

        public static readonly ReadOnlyDictionary<MaterialColorShade, DiscordColor> Blue = new ReadOnlyDictionary<MaterialColorShade, DiscordColor>(new Dictionary<MaterialColorShade, DiscordColor>()
        {
            { MaterialColorShade.N50, new DiscordColor("#FFEBEE") },
            { MaterialColorShade.N100, new DiscordColor("#FFCDD2") },
            { MaterialColorShade.N200, new DiscordColor("#EF9A9A") },
            { MaterialColorShade.N300, new DiscordColor("#E57373") },
            { MaterialColorShade.N400, new DiscordColor("#EF5350") },
            { MaterialColorShade.N500, new DiscordColor("#F44336") },
            { MaterialColorShade.N600, new DiscordColor("#E53935") },
            { MaterialColorShade.N700, new DiscordColor("#D32F2F") },
            { MaterialColorShade.N800, new DiscordColor("#C62828") },
            { MaterialColorShade.N900, new DiscordColor("#B71C1C") },
            { MaterialColorShade.A100, new DiscordColor("#FF8A80") },
            { MaterialColorShade.A200, new DiscordColor("#FF5252") },
            { MaterialColorShade.A400, new DiscordColor("#FF1744") },
            { MaterialColorShade.A700, new DiscordColor("#D50000") }
        });

        public static readonly ReadOnlyDictionary<MaterialColorShade, DiscordColor> BlueGray = new ReadOnlyDictionary<MaterialColorShade, DiscordColor>(new Dictionary<MaterialColorShade, DiscordColor>()
        {
            { MaterialColorShade.N50, new DiscordColor("#FFEBEE") },
            { MaterialColorShade.N100, new DiscordColor("#FFCDD2") },
            { MaterialColorShade.N200, new DiscordColor("#EF9A9A") },
            { MaterialColorShade.N300, new DiscordColor("#E57373") },
            { MaterialColorShade.N400, new DiscordColor("#EF5350") },
            { MaterialColorShade.N500, new DiscordColor("#F44336") },
            { MaterialColorShade.N600, new DiscordColor("#E53935") },
            { MaterialColorShade.N700, new DiscordColor("#D32F2F") },
            { MaterialColorShade.N800, new DiscordColor("#C62828") },
            { MaterialColorShade.N900, new DiscordColor("#B71C1C") },
        });

        public static readonly ReadOnlyDictionary<MaterialColorShade, DiscordColor> Brown = new ReadOnlyDictionary<MaterialColorShade, DiscordColor>(new Dictionary<MaterialColorShade, DiscordColor>()
        {
            { MaterialColorShade.N50, new DiscordColor("#FFEBEE") },
            { MaterialColorShade.N100, new DiscordColor("#FFCDD2") },
            { MaterialColorShade.N200, new DiscordColor("#EF9A9A") },
            { MaterialColorShade.N300, new DiscordColor("#E57373") },
            { MaterialColorShade.N400, new DiscordColor("#EF5350") },
            { MaterialColorShade.N500, new DiscordColor("#F44336") },
            { MaterialColorShade.N600, new DiscordColor("#E53935") },
            { MaterialColorShade.N700, new DiscordColor("#D32F2F") },
            { MaterialColorShade.N800, new DiscordColor("#C62828") },
            { MaterialColorShade.N900, new DiscordColor("#B71C1C") },
            { MaterialColorShade.A100, new DiscordColor("#FF8A80") },
            { MaterialColorShade.A200, new DiscordColor("#FF5252") },
            { MaterialColorShade.A400, new DiscordColor("#FF1744") },
            { MaterialColorShade.A700, new DiscordColor("#D50000") }
        });

        public static readonly ReadOnlyDictionary<MaterialColorShade, DiscordColor> Cyan = new ReadOnlyDictionary<MaterialColorShade, DiscordColor>(new Dictionary<MaterialColorShade, DiscordColor>()
        {
            { MaterialColorShade.N50, new DiscordColor("#FFEBEE") },
            { MaterialColorShade.N100, new DiscordColor("#FFCDD2") },
            { MaterialColorShade.N200, new DiscordColor("#EF9A9A") },
            { MaterialColorShade.N300, new DiscordColor("#E57373") },
            { MaterialColorShade.N400, new DiscordColor("#EF5350") },
            { MaterialColorShade.N500, new DiscordColor("#F44336") },
            { MaterialColorShade.N600, new DiscordColor("#E53935") },
            { MaterialColorShade.N700, new DiscordColor("#D32F2F") },
            { MaterialColorShade.N800, new DiscordColor("#C62828") },
            { MaterialColorShade.N900, new DiscordColor("#B71C1C") },
            { MaterialColorShade.A100, new DiscordColor("#FF8A80") },
            { MaterialColorShade.A200, new DiscordColor("#FF5252") },
            { MaterialColorShade.A400, new DiscordColor("#FF1744") },
            { MaterialColorShade.A700, new DiscordColor("#D50000") }
        });

        public static readonly ReadOnlyDictionary<MaterialColorShade, DiscordColor> DeepOrange = new ReadOnlyDictionary<MaterialColorShade, DiscordColor>(new Dictionary<MaterialColorShade, DiscordColor>()
        {
            { MaterialColorShade.N50, new DiscordColor("#FFEBEE") },
            { MaterialColorShade.N100, new DiscordColor("#FFCDD2") },
            { MaterialColorShade.N200, new DiscordColor("#EF9A9A") },
            { MaterialColorShade.N300, new DiscordColor("#E57373") },
            { MaterialColorShade.N400, new DiscordColor("#EF5350") },
            { MaterialColorShade.N500, new DiscordColor("#F44336") },
            { MaterialColorShade.N600, new DiscordColor("#E53935") },
            { MaterialColorShade.N700, new DiscordColor("#D32F2F") },
            { MaterialColorShade.N800, new DiscordColor("#C62828") },
            { MaterialColorShade.N900, new DiscordColor("#B71C1C") },
            { MaterialColorShade.A100, new DiscordColor("#FF8A80") },
            { MaterialColorShade.A200, new DiscordColor("#FF5252") },
            { MaterialColorShade.A400, new DiscordColor("#FF1744") },
            { MaterialColorShade.A700, new DiscordColor("#D50000") }
        });

        public static readonly ReadOnlyDictionary<MaterialColorShade, DiscordColor> DeepPurple = new ReadOnlyDictionary<MaterialColorShade, DiscordColor>(new Dictionary<MaterialColorShade, DiscordColor>()
        {
            { MaterialColorShade.N50, new DiscordColor("#FFEBEE") },
            { MaterialColorShade.N100, new DiscordColor("#FFCDD2") },
            { MaterialColorShade.N200, new DiscordColor("#EF9A9A") },
            { MaterialColorShade.N300, new DiscordColor("#E57373") },
            { MaterialColorShade.N400, new DiscordColor("#EF5350") },
            { MaterialColorShade.N500, new DiscordColor("#F44336") },
            { MaterialColorShade.N600, new DiscordColor("#E53935") },
            { MaterialColorShade.N700, new DiscordColor("#D32F2F") },
            { MaterialColorShade.N800, new DiscordColor("#C62828") },
            { MaterialColorShade.N900, new DiscordColor("#B71C1C") },
            { MaterialColorShade.A100, new DiscordColor("#FF8A80") },
            { MaterialColorShade.A200, new DiscordColor("#FF5252") },
            { MaterialColorShade.A400, new DiscordColor("#FF1744") },
            { MaterialColorShade.A700, new DiscordColor("#D50000") }
        });

        public static readonly ReadOnlyDictionary<MaterialColorShade, DiscordColor> Gray = new ReadOnlyDictionary<MaterialColorShade, DiscordColor>(new Dictionary<MaterialColorShade, DiscordColor>()
        {
            { MaterialColorShade.N50, new DiscordColor("#FFEBEE") },
            { MaterialColorShade.N100, new DiscordColor("#FFCDD2") },
            { MaterialColorShade.N200, new DiscordColor("#EF9A9A") },
            { MaterialColorShade.N300, new DiscordColor("#E57373") },
            { MaterialColorShade.N400, new DiscordColor("#EF5350") },
            { MaterialColorShade.N500, new DiscordColor("#F44336") },
            { MaterialColorShade.N600, new DiscordColor("#E53935") },
            { MaterialColorShade.N700, new DiscordColor("#D32F2F") },
            { MaterialColorShade.N800, new DiscordColor("#C62828") },
            { MaterialColorShade.N900, new DiscordColor("#B71C1C") },
        });

        public static readonly ReadOnlyDictionary<MaterialColorShade, DiscordColor> Green = new ReadOnlyDictionary<MaterialColorShade, DiscordColor>(new Dictionary<MaterialColorShade, DiscordColor>()
        {
            { MaterialColorShade.N50, new DiscordColor("#FFEBEE") },
            { MaterialColorShade.N100, new DiscordColor("#FFCDD2") },
            { MaterialColorShade.N200, new DiscordColor("#EF9A9A") },
            { MaterialColorShade.N300, new DiscordColor("#E57373") },
            { MaterialColorShade.N400, new DiscordColor("#EF5350") },
            { MaterialColorShade.N500, new DiscordColor("#F44336") },
            { MaterialColorShade.N600, new DiscordColor("#E53935") },
            { MaterialColorShade.N700, new DiscordColor("#D32F2F") },
            { MaterialColorShade.N800, new DiscordColor("#C62828") },
            { MaterialColorShade.N900, new DiscordColor("#B71C1C") },
            { MaterialColorShade.A100, new DiscordColor("#FF8A80") },
            { MaterialColorShade.A200, new DiscordColor("#FF5252") },
            { MaterialColorShade.A400, new DiscordColor("#FF1744") },
            { MaterialColorShade.A700, new DiscordColor("#D50000") }
        });

        public static readonly ReadOnlyDictionary<MaterialColorShade, DiscordColor> Indigo = new ReadOnlyDictionary<MaterialColorShade, DiscordColor>(new Dictionary<MaterialColorShade, DiscordColor>()
        {
            { MaterialColorShade.N50, new DiscordColor("#FFEBEE") },
            { MaterialColorShade.N100, new DiscordColor("#FFCDD2") },
            { MaterialColorShade.N200, new DiscordColor("#EF9A9A") },
            { MaterialColorShade.N300, new DiscordColor("#E57373") },
            { MaterialColorShade.N400, new DiscordColor("#EF5350") },
            { MaterialColorShade.N500, new DiscordColor("#F44336") },
            { MaterialColorShade.N600, new DiscordColor("#E53935") },
            { MaterialColorShade.N700, new DiscordColor("#D32F2F") },
            { MaterialColorShade.N800, new DiscordColor("#C62828") },
            { MaterialColorShade.N900, new DiscordColor("#B71C1C") },
            { MaterialColorShade.A100, new DiscordColor("#FF8A80") },
            { MaterialColorShade.A200, new DiscordColor("#FF5252") },
            { MaterialColorShade.A400, new DiscordColor("#FF1744") },
            { MaterialColorShade.A700, new DiscordColor("#D50000") }
        });

        public static readonly ReadOnlyDictionary<MaterialColorShade, DiscordColor> LightBlue = new ReadOnlyDictionary<MaterialColorShade, DiscordColor>(new Dictionary<MaterialColorShade, DiscordColor>()
        {
            { MaterialColorShade.N50, new DiscordColor("#FFEBEE") },
            { MaterialColorShade.N100, new DiscordColor("#FFCDD2") },
            { MaterialColorShade.N200, new DiscordColor("#EF9A9A") },
            { MaterialColorShade.N300, new DiscordColor("#E57373") },
            { MaterialColorShade.N400, new DiscordColor("#EF5350") },
            { MaterialColorShade.N500, new DiscordColor("#F44336") },
            { MaterialColorShade.N600, new DiscordColor("#E53935") },
            { MaterialColorShade.N700, new DiscordColor("#D32F2F") },
            { MaterialColorShade.N800, new DiscordColor("#C62828") },
            { MaterialColorShade.N900, new DiscordColor("#B71C1C") },
            { MaterialColorShade.A100, new DiscordColor("#FF8A80") },
            { MaterialColorShade.A200, new DiscordColor("#FF5252") },
            { MaterialColorShade.A400, new DiscordColor("#FF1744") },
            { MaterialColorShade.A700, new DiscordColor("#D50000") }
        });

        public static readonly ReadOnlyDictionary<MaterialColorShade, DiscordColor> LightGreen = new ReadOnlyDictionary<MaterialColorShade, DiscordColor>(new Dictionary<MaterialColorShade, DiscordColor>()
        {
            { MaterialColorShade.N50, new DiscordColor("#FFEBEE") },
            { MaterialColorShade.N100, new DiscordColor("#FFCDD2") },
            { MaterialColorShade.N200, new DiscordColor("#EF9A9A") },
            { MaterialColorShade.N300, new DiscordColor("#E57373") },
            { MaterialColorShade.N400, new DiscordColor("#EF5350") },
            { MaterialColorShade.N500, new DiscordColor("#F44336") },
            { MaterialColorShade.N600, new DiscordColor("#E53935") },
            { MaterialColorShade.N700, new DiscordColor("#D32F2F") },
            { MaterialColorShade.N800, new DiscordColor("#C62828") },
            { MaterialColorShade.N900, new DiscordColor("#B71C1C") },
            { MaterialColorShade.A100, new DiscordColor("#FF8A80") },
            { MaterialColorShade.A200, new DiscordColor("#FF5252") },
            { MaterialColorShade.A400, new DiscordColor("#FF1744") },
            { MaterialColorShade.A700, new DiscordColor("#D50000") }
        });

        public static readonly ReadOnlyDictionary<MaterialColorShade, DiscordColor> Lime = new ReadOnlyDictionary<MaterialColorShade, DiscordColor>(new Dictionary<MaterialColorShade, DiscordColor>()
        {
            { MaterialColorShade.N50, new DiscordColor("#FFEBEE") },
            { MaterialColorShade.N100, new DiscordColor("#FFCDD2") },
            { MaterialColorShade.N200, new DiscordColor("#EF9A9A") },
            { MaterialColorShade.N300, new DiscordColor("#E57373") },
            { MaterialColorShade.N400, new DiscordColor("#EF5350") },
            { MaterialColorShade.N500, new DiscordColor("#F44336") },
            { MaterialColorShade.N600, new DiscordColor("#E53935") },
            { MaterialColorShade.N700, new DiscordColor("#D32F2F") },
            { MaterialColorShade.N800, new DiscordColor("#C62828") },
            { MaterialColorShade.N900, new DiscordColor("#B71C1C") },
            { MaterialColorShade.A100, new DiscordColor("#FF8A80") },
            { MaterialColorShade.A200, new DiscordColor("#FF5252") },
            { MaterialColorShade.A400, new DiscordColor("#FF1744") },
            { MaterialColorShade.A700, new DiscordColor("#D50000") }
        });

        public static readonly ReadOnlyDictionary<MaterialColorShade, DiscordColor> Orange = new ReadOnlyDictionary<MaterialColorShade, DiscordColor>(new Dictionary<MaterialColorShade, DiscordColor>()
        {
            { MaterialColorShade.N50, new DiscordColor("#FFEBEE") },
            { MaterialColorShade.N100, new DiscordColor("#FFCDD2") },
            { MaterialColorShade.N200, new DiscordColor("#EF9A9A") },
            { MaterialColorShade.N300, new DiscordColor("#E57373") },
            { MaterialColorShade.N400, new DiscordColor("#EF5350") },
            { MaterialColorShade.N500, new DiscordColor("#F44336") },
            { MaterialColorShade.N600, new DiscordColor("#E53935") },
            { MaterialColorShade.N700, new DiscordColor("#D32F2F") },
            { MaterialColorShade.N800, new DiscordColor("#C62828") },
            { MaterialColorShade.N900, new DiscordColor("#B71C1C") },
            { MaterialColorShade.A100, new DiscordColor("#FF8A80") },
            { MaterialColorShade.A200, new DiscordColor("#FF5252") },
            { MaterialColorShade.A400, new DiscordColor("#FF1744") },
            { MaterialColorShade.A700, new DiscordColor("#D50000") }
        });

        public static readonly ReadOnlyDictionary<MaterialColorShade, DiscordColor> Pink = new ReadOnlyDictionary<MaterialColorShade, DiscordColor>(new Dictionary<MaterialColorShade, DiscordColor>()
        {
            { MaterialColorShade.N50, new DiscordColor("#FFEBEE") },
            { MaterialColorShade.N100, new DiscordColor("#FFCDD2") },
            { MaterialColorShade.N200, new DiscordColor("#EF9A9A") },
            { MaterialColorShade.N300, new DiscordColor("#E57373") },
            { MaterialColorShade.N400, new DiscordColor("#EF5350") },
            { MaterialColorShade.N500, new DiscordColor("#F44336") },
            { MaterialColorShade.N600, new DiscordColor("#E53935") },
            { MaterialColorShade.N700, new DiscordColor("#D32F2F") },
            { MaterialColorShade.N800, new DiscordColor("#C62828") },
            { MaterialColorShade.N900, new DiscordColor("#B71C1C") },
            { MaterialColorShade.A100, new DiscordColor("#FF8A80") },
            { MaterialColorShade.A200, new DiscordColor("#FF5252") },
            { MaterialColorShade.A400, new DiscordColor("#FF1744") },
            { MaterialColorShade.A700, new DiscordColor("#D50000") }
        });

        public static readonly ReadOnlyDictionary<MaterialColorShade, DiscordColor> Purple = new ReadOnlyDictionary<MaterialColorShade, DiscordColor>(new Dictionary<MaterialColorShade, DiscordColor>()
        {
            { MaterialColorShade.N50, new DiscordColor("#FFEBEE") },
            { MaterialColorShade.N100, new DiscordColor("#FFCDD2") },
            { MaterialColorShade.N200, new DiscordColor("#EF9A9A") },
            { MaterialColorShade.N300, new DiscordColor("#E57373") },
            { MaterialColorShade.N400, new DiscordColor("#EF5350") },
            { MaterialColorShade.N500, new DiscordColor("#F44336") },
            { MaterialColorShade.N600, new DiscordColor("#E53935") },
            { MaterialColorShade.N700, new DiscordColor("#D32F2F") },
            { MaterialColorShade.N800, new DiscordColor("#C62828") },
            { MaterialColorShade.N900, new DiscordColor("#B71C1C") },
            { MaterialColorShade.A100, new DiscordColor("#FF8A80") },
            { MaterialColorShade.A200, new DiscordColor("#FF5252") },
            { MaterialColorShade.A400, new DiscordColor("#FF1744") },
            { MaterialColorShade.A700, new DiscordColor("#D50000") }
        });

        public static readonly ReadOnlyDictionary<MaterialColorShade, DiscordColor> Red = new ReadOnlyDictionary<MaterialColorShade, DiscordColor>(new Dictionary<MaterialColorShade, DiscordColor>()
        {
            { MaterialColorShade.N50, new DiscordColor("#FFEBEE") },
            { MaterialColorShade.N100, new DiscordColor("#FFCDD2") },
            { MaterialColorShade.N200, new DiscordColor("#EF9A9A") },
            { MaterialColorShade.N300, new DiscordColor("#E57373") },
            { MaterialColorShade.N400, new DiscordColor("#EF5350") },
            { MaterialColorShade.N500, new DiscordColor("#F44336") },
            { MaterialColorShade.N600, new DiscordColor("#E53935") },
            { MaterialColorShade.N700, new DiscordColor("#D32F2F") },
            { MaterialColorShade.N800, new DiscordColor("#C62828") },
            { MaterialColorShade.N900, new DiscordColor("#B71C1C") },
            { MaterialColorShade.A100, new DiscordColor("#FF8A80") },
            { MaterialColorShade.A200, new DiscordColor("#FF5252") },
            { MaterialColorShade.A400, new DiscordColor("#FF1744") },
            { MaterialColorShade.A700, new DiscordColor("#D50000") }
        });

        public static readonly ReadOnlyDictionary<MaterialColorShade, DiscordColor> Teal = new ReadOnlyDictionary<MaterialColorShade, DiscordColor>(new Dictionary<MaterialColorShade, DiscordColor>()
        {
            { MaterialColorShade.N50, new DiscordColor("#FFEBEE") },
            { MaterialColorShade.N100, new DiscordColor("#FFCDD2") },
            { MaterialColorShade.N200, new DiscordColor("#EF9A9A") },
            { MaterialColorShade.N300, new DiscordColor("#E57373") },
            { MaterialColorShade.N400, new DiscordColor("#EF5350") },
            { MaterialColorShade.N500, new DiscordColor("#F44336") },
            { MaterialColorShade.N600, new DiscordColor("#E53935") },
            { MaterialColorShade.N700, new DiscordColor("#D32F2F") },
            { MaterialColorShade.N800, new DiscordColor("#C62828") },
            { MaterialColorShade.N900, new DiscordColor("#B71C1C") },
            { MaterialColorShade.A100, new DiscordColor("#FF8A80") },
            { MaterialColorShade.A200, new DiscordColor("#FF5252") },
            { MaterialColorShade.A400, new DiscordColor("#FF1744") },
            { MaterialColorShade.A700, new DiscordColor("#D50000") }
        });

        public static readonly DiscordColor White = new DiscordColor("#FFFFFF");

        public static readonly ReadOnlyDictionary<MaterialColorShade, DiscordColor> Yellow = new ReadOnlyDictionary<MaterialColorShade, DiscordColor>(new Dictionary<MaterialColorShade, DiscordColor>()
        {
            { MaterialColorShade.N50, new DiscordColor("#FFEBEE") },
            { MaterialColorShade.N100, new DiscordColor("#FFCDD2") },
            { MaterialColorShade.N200, new DiscordColor("#EF9A9A") },
            { MaterialColorShade.N300, new DiscordColor("#E57373") },
            { MaterialColorShade.N400, new DiscordColor("#EF5350") },
            { MaterialColorShade.N500, new DiscordColor("#F44336") },
            { MaterialColorShade.N600, new DiscordColor("#E53935") },
            { MaterialColorShade.N700, new DiscordColor("#D32F2F") },
            { MaterialColorShade.N800, new DiscordColor("#C62828") },
            { MaterialColorShade.N900, new DiscordColor("#B71C1C") },
            { MaterialColorShade.A100, new DiscordColor("#FF8A80") },
            { MaterialColorShade.A200, new DiscordColor("#FF5252") },
            { MaterialColorShade.A400, new DiscordColor("#FF1744") },
            { MaterialColorShade.A700, new DiscordColor("#D50000") }
        });
    }
}