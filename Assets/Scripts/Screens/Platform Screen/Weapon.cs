using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Screens.Platform_Screen
{
    public class Weapon : MonoBehaviour
    {
        public int Damage;
        public float HitStun;
        public float LaunchX;
        public float LaunchY;

        public Vector3 ParentPosition { get { return transform.parent.transform.position; } }
        public bool Active;     // True if the weapon should be doing damage in its hitbox.
                                // Unity collisions are bug-a-licious. Kludge.
        public bool WeaponHit;  // Unless otherwise noted, a weapon can only hit one enemy per swing.
    }
}
