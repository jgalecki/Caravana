using UnityEngine;
using UnityEditor;
using System;

namespace Assets.Scripts.Screens
{
    // Trying to combine familiar screens with Unity scenes... Many ideas here are indebted to Microsoft / Monogame Screen class .
    public abstract class GameScreen : MonoBehaviour
    {
        //public bool HasScript;
        //public string ScriptFile;

        // Active goes through Updates and takes input, Covered updates Paused displays screen but takes no input or Updates, and Hidden takes no input and doesn't display.
        public enum ScreenState
        {
            Active,         // Updates, takes input, and displays
            Covered,        // Updates, no input, displays - screen on top IsPopup
            Paused,         // No updates, no input, displays - screen on top IsPauseScreen
            Hidden          // No updates, no input, no display - screen on top IsBaseScreen    
        }

        public ScreenState State
        {
            get
            {
                if (IsHandlingInput)
                {
                    return ScreenState.Active;
                }
                else if (IsUpdating)
                {
                    return ScreenState.Covered;
                }
                else if (IsDisplayed)
                {
                    return ScreenState.Paused;
                }
                else
                {
                    return ScreenState.Hidden;
                }
            }
        }

        // Popups do not pause the screen below and keep the screen below displayed
        public bool IsPopup;
        // Pause screens pause the screen below and keep the screen below displayed
        public bool IsPauseScreen;
        // Base screens pause and hide the screen below
        public bool IsBaseScreen;

        /// <summary>
        /// Checks whether this screen can respond to user input.
        /// </summary>
        public bool IsHandlingInput
        {
            get
            {
                return !_higherScreenTakesInput;
            }
        }
        protected bool _higherScreenTakesInput;
        protected bool _previousHigherScreenTakesInput;

        /// <summary>
        /// Checks whether this screen should update or not
        /// </summary>
        public bool IsUpdating
        {
            get
            {
                return !_higherScreenIsPauseScreen;
            }

        }
        protected bool _higherScreenIsPauseScreen;
        protected bool _previousHigherScreenIsPauseScreen;

        /// <summary>
        /// Checks whether this screen is hidden by another BaseScreen
        /// </summary>
        public bool IsDisplayed
        {
            get
            {
                return !_higherScreenIsBaseScreen;
            }
        }
        protected bool _higherScreenIsBaseScreen;
        protected bool _previousHigherScreenIsBaseScreen;

        /// <summary>
        /// 
        /// </summary>
        public virtual void UpdateScreenState(bool coveredByPopUp, bool coveredByPause, bool coveredByBaseScreen)
        {
            _previousHigherScreenTakesInput = _higherScreenTakesInput;
            _previousHigherScreenIsPauseScreen = _higherScreenIsPauseScreen;
            _previousHigherScreenIsBaseScreen = _higherScreenIsBaseScreen;

            _higherScreenTakesInput = coveredByPopUp;
            _higherScreenIsPauseScreen = coveredByPause;
            _higherScreenIsBaseScreen = coveredByBaseScreen;

            if (_higherScreenIsBaseScreen && !_previousHigherScreenIsBaseScreen)
            {
                // Newly covered - disable screen
                gameObject.SetActive(false);
            }
            else if (!_higherScreenIsBaseScreen && _previousHigherScreenIsBaseScreen)
            {
                // Newly uncovered - reactivate screen
                gameObject.SetActive(true);
            }


        }

        /// <summary>
        /// ConstantUpdate() will be called every Update frame
        /// </summary>
        public virtual void ConstantUpdate() { }

        /// <summary>
        /// ControlledUpdate() will only be called if screen.IsUpdating is true. Things should go in here by default.
        /// </summary>
        public virtual void ControlledUpdate() { }

        /// <summary>
        /// Only called when screen is active and has focus. Called automatically when otherScreenHasFocus is set by ScreenManager
        /// </summary>
        public virtual void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        /// <summary>
        /// Call any necessary setup here. Useful because Start() and Awake() can't be relied on - an object might intentionally start as disabled
        /// </summary>
        public virtual void Initialize()
        {
            //if (HasScript)
            //{
            //    LoadScript();
            //}
        }

        public virtual void LoadScript() { }

        /// <summary>
        /// TODO: There's no transition animation between screens yet.
        /// </summary>
        public virtual void ExitScreen()
        {
            ScreenManager.RemoveScreen(this);
        }

        public Action OnRemoval;
    }
}