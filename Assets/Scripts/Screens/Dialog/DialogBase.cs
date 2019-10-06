using Assets.Scripts.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Screens.Dialog
{
    public class DialogBase : MonoBehaviour
    {
        static readonly ILog _log = Log.GetLogger(typeof(DialogBase));

        public DialogScreen DialogScreen;


        void Awake()
        {
            Application.logMessageReceived += Log.GetLogCallback(_log);
        }

        void Start()
        {
            ScreenManager.Initialize();
            PlayerModel.Initialize();
            _log.Write("Adding this scene's Dialog Screen");
            ScreenManager.AddScreen(DialogScreen);
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
