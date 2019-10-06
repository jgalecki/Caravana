using Assets.Scripts.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Screens.Desert
{
    class DesertBase : MonoBehaviour
    {
        static readonly ILog _log = Log.GetLogger(typeof(DesertBase));
        public static bool ExceptionSetup;

        public DesertScreen DesertScreen;


        void Awake()
        {
            if (!ExceptionSetup)
            {
                ExceptionSetup = true;
                Application.logMessageReceived += Log.GetLogCallback(_log);
            }
        }

        void Start()
        {
            ScreenManager.Initialize();
            PlayerModel.Initialize();
            _log.Write("Adding this scene's PlaceScreen");
            ScreenManager.AddScreen(DesertScreen);
            ScreenManager.TraceEnabled = true;
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
