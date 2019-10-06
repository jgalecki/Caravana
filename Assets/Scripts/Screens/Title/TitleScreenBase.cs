using Assets.Scripts.Logging;
using UnityEngine;

namespace Assets.Scripts.Screens
{
    /// <summary>
    /// The base object for the title screen. Guaranteed to call ScreenManager each frame, regardless of which Screens are 
    /// active or not. Something similar should be implemented for every new screen we make.
    /// </summary>
    public class TitleScreenBase : MonoBehaviour
    {
        static readonly ILog _log = Log.GetLogger(typeof(TitleScreenBase));

        public TitleScreen TitleScreen;


        // Start is called before the first frame update
        void Start()
        {
            ScreenManager.Initialize();
            ScreenManager.AddScreen(TitleScreen);
            ScreenManager.TraceEnabled = true;

            PlayerModel.Initialize();
        }

        // Update is called once per frame
        void Update()
        {
            if (!ScreenManager.UpdateCalled)
            {
                ScreenManager.Update();
            }
        }
    }
}
