using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Screens.Platform_Screen
{
    class PlayerPickupBox : MonoBehaviour
    {
        public PlayerController Player;

        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Pickup")
            {
                var pickup = collision.GetComponent<Pickup>();
                if (!pickup.Takable) return;

                pickup.Taken = true;

                switch (pickup.Type)
                {
                    case PickupType.Heart:
                        Player.ChangeHealth(1);
                        SoundManager.PlaySingle(pickup.HeartPickup);
                        break;
                    case PickupType.Gem:
                        PlayerModel.Gems += 1;
                        SoundManager.PlaySingle(pickup.GemPickup);
                        break;
                    default:
                        throw new Exception("Unexpected pickup type: " + pickup.Type.ToString());
                }
            }
        }

        public void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Pickup")
            {
                var pickup = collision.GetComponent<Pickup>();
                if (!pickup.Takable) return;

                pickup.Taken = true;

                switch (pickup.Type)
                {
                    case PickupType.Heart:
                        Player.ChangeHealth(1);
                        SoundManager.PlaySingle(pickup.HeartPickup);
                        break;
                    case PickupType.Gem:
                        PlayerModel.Gems += 1;
                        SoundManager.PlaySingle(pickup.GemPickup);
                        break;
                    default:
                        throw new Exception("Unexpected pickup type: " + pickup.Type.ToString());
                }
            }
        }
    }
}
