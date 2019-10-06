using Assets.Scripts.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    static class PlayerModel
    {
        static readonly ILog _log = Log.GetLogger(typeof(PlayerModel));

        public static Random Random;

        static public int Gems;
        
        public static int DialogCount;

        public static void Initialize()
        {
            if (Random != null) return;

            // TODO: allow setting of the seed somehow
            Random = new System.Random();
            int seed = Random.Next();
            _log.Write("Awake, seed is {0}", seed);
            Random = new System.Random(seed);
        }
    }
}
