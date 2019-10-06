using System;
using Assets.Scripts.Logging;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Screens
{
    // <3 screens over scenes. My own level management
    public class ScreenManager
    {
        static readonly ILog _log = Log.GetLogger(typeof(ScreenManager));
        public static bool TraceEnabled = true;
        public static bool TraceEachFrameEnabled;

        public static string CurrentScreenName;

        private static List<GameScreen> _screens;
        private static List<GameScreen> _screensToUpdate;
        private static List<GameScreen> _miniScreensToUpdate;

        public static bool UpdateCalled { get; set; }

        public static void Initialize()
        {
            if (_screens == null) _screens = new List<GameScreen>();
            if (_screensToUpdate == null) _screensToUpdate = new List<GameScreen>();
            if (_miniScreensToUpdate == null) _miniScreensToUpdate = new List<GameScreen>();
        }

        /// <summary>
        /// Call this from the "main" screen - the conversation screen or the world screen;
        /// </summary>
        public static void Update()
        {
            try
            {
                if (UpdateCalled) return; // Guard clause. This should only be called once per update by some sort of ScreenBase. Regular Screens should defer to this.
                UpdateCalled = true;

                // Work off of a copy of Screens in case one screen adds or removes another
                // TODO: Necessary now that screens are updated in UpdateScreenState? Maybe a local variable?
                _screensToUpdate.Clear();
                foreach (var screen in _screens)
                {
                    _screensToUpdate.Add(screen);
                }

                while (_screensToUpdate.Count > 0)
                {
                    int topScreenIndex = _screensToUpdate.Count - 1;
                    var screen = _screensToUpdate[topScreenIndex];
                    _screensToUpdate.RemoveAt(topScreenIndex);

                    screen.ConstantUpdate();

                    if (screen.IsHandlingInput)
                    {
                        screen.HandleInput();
                    }
                    if (screen.IsUpdating)
                    {
                        screen.ControlledUpdate();
                    }
                }

                if (TraceEachFrameEnabled)
                {
                    TraceScreens();
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Unhandled Exception!");
            }
        }

        public static void LateUpdate()
        {
            UpdateCalled = false;
        }

        public static void TraceScreens()
        {
            List<string> screenNames = new List<string>();
            foreach (var screen in _screens)
            {
                // TODO: if called right as a screen is added or removed, this doesn't change the underlying screen states
                screenNames.Add(screen.name + ": " + screen.State.ToString());
            }

            _log.Write("Screens: [" + string.Join("], [", screenNames.ToArray()) + "]");
        }

        public static void AddScreen(GameScreen screen)
        {
            _screens.Add(screen);
            screen.gameObject.SetActive(true);
            screen.Initialize();

            UpdateScreenState();


            if (TraceEnabled)
            {
                _log.Write("Adding screen " + screen.name);
                TraceScreens();
            }
        }

        /// <summary>
        /// Removes a screen from the screen manager. You should normally
        /// use GameScreen.ExitScreen instead of calling this directly, so
        /// the screen can gradually transition off rather than just being
        /// instantly removed.
        /// </summary>
        internal static void RemoveScreen(GameScreen screen)
        {
            if (screen.OnRemoval != null)
            {
                screen.OnRemoval();
            }

            _screens.Remove(screen);
            _screensToUpdate.Remove(screen);
            screen.gameObject.SetActive(false);

            UpdateScreenState();

            if (TraceEnabled)
            {
                _log.Write("Removing screen " + screen.name);
                TraceScreens();
            }
        }

        public static void UpdateScreenState()
        {
            // Work off of a copy of Screens in case one screen adds or removes another
            _miniScreensToUpdate.Clear();
            foreach (var screen in _screens)
            {
                _miniScreensToUpdate.Add(screen);
            }

            bool otherScreenIsTakingInput = false; // 'Time.timeScale == 0;' is not equivalent to Game.IsActive
            bool otherScreenIsPausing = false;
            bool otherScreenIsBaseScreen = false;

            while (_miniScreensToUpdate.Count > 0)
            {
                int topScreenIndex = _miniScreensToUpdate.Count - 1;
                var screen = _miniScreensToUpdate[topScreenIndex];
                _miniScreensToUpdate.RemoveAt(topScreenIndex);

                screen.UpdateScreenState(otherScreenIsTakingInput, otherScreenIsPausing, otherScreenIsBaseScreen);

                if (!otherScreenIsTakingInput)
                {
                    otherScreenIsTakingInput = true;
                }
                if (!otherScreenIsPausing)
                {
                    if (screen.IsPauseScreen)
                    {
                        otherScreenIsPausing = true;
                    }
                }
                if (screen.IsBaseScreen)
                {
                    otherScreenIsPausing = true; // Since we control our own updates, we want to stop them if there is a BaseScreen on top.
                    otherScreenIsBaseScreen = true;
                }
            }

        }


        /// <summary>
        /// Expose an array holding all the screens. We return a copy rather
        /// than the real master list, because screens should only ever be added
        /// or removed using the AddScreen and RemoveScreen methods.
        /// </summary>
        public static GameScreen[] GetScreens()
        {
            return _screens.ToArray();
        }

        /// <summary>
        /// Since Unity handles all our scene loading for us, we might be caught holding a reference to a deleted screen.
        /// To prevent that, each time we load a new scene, we'll clear the Screens array
        /// </summary>
        public static void LoadScene(string scene)
        {
            _log.Write("Loading new scene {0}, dropping all current scenes", scene);
            _screens.Clear();
            SceneManager.LoadScene(scene);
        }
    }
}
