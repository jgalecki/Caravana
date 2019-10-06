using Assets.Scripts.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Screens
{
    public class GameOver : GameScreen
    {
        public override void HandleInput()
        {
            base.HandleInput();

            if (InputHelp.AnyButtonDown())
            {
                ScreenManager.LoadScene("Title");
            }
        }

    }
}
