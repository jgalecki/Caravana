using Assets.Scripts.IO;
using Assets.Scripts.Logging;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Screens
{
    public class TitleScreen : GameScreen
    {
        static readonly ILog _log = Log.GetLogger(typeof(TitleScreen));

        public int _selectionIndex;
        public Image SelectionArrow;

        private float _inputTimer;
        private const float INPUT_DELAY = 0.25f;
        private const int BUTTON_X_OFFSET = -20;

        public List<Text> Selectables;

        public AudioClip TitleSong;

        public override void Initialize()
        {
            _inputTimer = 0;
            _selectionIndex = 0;
            MoveSelectionTriangle();
            SoundManager.PlayMusic(TitleSong);
        }

        private void MoveSelectionTriangle()
        {
            var selPos = Selectables[_selectionIndex].transform.position;
            int offset = _selectionIndex == 0 ? -170 : -80;
            SelectionArrow.transform.position = new Vector3(selPos.x + offset, selPos.y, 0);
        }

        void Start()
        {
            ScreenManager.Initialize();
            ScreenManager.TraceEnabled = true;
        }


        public override void HandleInput()
        {
            base.HandleInput();

            if (_inputTimer > 0)
            {
                _inputTimer = Mathf.Max(_inputTimer - Time.deltaTime, 0);
            }

            if (_inputTimer == 0 && InputHelp.GetButtonDown(InputHelp.Buttons.Left))
            {
                AdjustSelectedIndex(-1);
            }
            else if (_inputTimer == 0 && InputHelp.GetButtonDown(InputHelp.Buttons.Right))
            { 
                AdjustSelectedIndex(1);
            }

            if (InputHelp.GetButtonDown(InputHelp.Buttons.Jump) || InputHelp.GetButtonDown(InputHelp.Buttons.Attack))
            {
                switch (_selectionIndex)
                {
                    case 0:
                        PlayerModel.DialogCount = 1;
                        ScreenManager.LoadScene("Dialog");
                        ExitScreen();
                        break;
                    case 1:
                        Application.Quit();
                        break;
                }
                SoundManager.PlayConfirm();
                InputHelp.ResetInputAxes();
            }
        }

        public void AdjustSelectedIndex(int adjustment)
        {
            if (_selectionIndex + adjustment < 0 || _selectionIndex + adjustment >= Selectables.Count)
            {
                SoundManager.PlayCancel();
                return;
            }
            _selectionIndex = _selectionIndex + adjustment;
            MoveSelectionTriangle();
            _inputTimer = INPUT_DELAY;
            SoundManager.PlayMoveUI();
        }

        // Update is called once per frame
        void Update()
        {
            if (!ScreenManager.UpdateCalled)
            {
                ScreenManager.Update();
            }

        }


        public void LateUpdate()
        {
            ScreenManager.LateUpdate();
        }
    }
}
