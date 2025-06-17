using System.Collections.Generic;

namespace Data
{
    public static class LevelConfig
    {
        internal static List<Levels> _levels = new List<Levels>
        {
            new Levels { camSize = 5, doneTragetNum = 2, colorNum = 2 },
            new Levels { camSize = 8, doneTragetNum = 57, colorNum = 8 },
            new Levels { camSize = 8, doneTragetNum = 57, colorNum = 12 }
        };

        internal class Levels
        {
            internal float camSize;
            internal int   doneTragetNum;
            internal int   colorNum;
        }
    }
}
