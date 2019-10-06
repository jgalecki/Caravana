using Assets.Scripts.IO;
using Assets.Scripts.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Screens.Dialog
{
    public class DialogScreen : GameScreen
    {
        static readonly ILog _log = Log.GetLogger(typeof(DialogScreen));

        public Image NameBox;
        public Text NameText;
        public Text DialogText;

        //public string DialogFileName;
        public string[] Lines;
        private int _lineIndex;

        public AudioClip DialogSong;

        public override void Initialize()
        {
            base.Initialize();

            Load();
            SoundManager.PlayMusic(DialogSong);
        }

        public void Load()
        {
            string fileName = "Assets/Dialogs/Dialog" + PlayerModel.DialogCount + ".txt";
            _log.Write("Loading dialog file {0}", fileName);
            Lines = File.ReadAllLines(fileName);
        }

        public override void ControlledUpdate()
        {
            base.ControlledUpdate();

            var line = Lines[_lineIndex];
            string speaker = line.Split(' ')[0];
            NameBox.gameObject.SetActive(speaker != "x");
            NameText.gameObject.SetActive(speaker != "z");
            NameText.text = speaker;
            DialogText.text = line.Substring(speaker.Length + 1);
        }

        public override void HandleInput()
        {
            base.HandleInput();

            if (InputHelp.GetButtonDown(InputHelp.Buttons.Start))
            {
                PlayerModel.DialogCount += 1;
                if (PlayerModel.DialogCount == 4)
                {
                    Application.Quit();
                }
                ScreenManager.LoadScene("Desert");
            }
            if (InputHelp.AnyButtonDown())
            {
                Input.ResetInputAxes();
                _lineIndex += 1;
            }

            if (_lineIndex >= Lines.Count())
            {
                PlayerModel.DialogCount += 1;
                if (PlayerModel.DialogCount == 4)
                {
                    Application.Quit();
                }
                ScreenManager.LoadScene("Desert");
            }
        }

    }
}
