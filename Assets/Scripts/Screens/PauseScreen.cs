using Assets.Scripts.IO;
using Assets.Scripts.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Screens
{
    public class PauseScreen : GameScreen
    {
        static readonly ILog _log = Log.GetLogger(typeof(PauseScreen));

        public override void Initialize()
        {
            Time.timeScale = 0;
        }
        
        public override void HandleInput()
        {
            if (InputHelp.AnyButtonDown())
            {
                InputHelp.ResetInputAxes();
                ExitScreen();
                Time.timeScale = 1;
            }
        }
    }
}
