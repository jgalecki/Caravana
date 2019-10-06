using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Screens.Platform_Screen
{
    public enum PickupType { Heart, Gem }


    public class Pickup : MonoBehaviour
    {
        public PickupType Type;
        public bool Taken;
        public bool Takable;

        public AudioClip GemPickup;
        public AudioClip HeartPickup;

        private float _disabledTimer;
        private const float DISABLED_ON_SPAWNING = 1f;

        public void Initialize()
        {
            _disabledTimer = DISABLED_ON_SPAWNING;
        }

        public void ControlledUpdate()
        {
            if (_disabledTimer > 0)
            {
                _disabledTimer -= Time.deltaTime;
                if (_disabledTimer <= 0)
                {
                    GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);
                    Takable = true;
                }
                else
                {
                    GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
                    Takable = false;
                }
            }
        }
    }
}
