using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Screens.Platform_Screen
{
    public class Camel : Body
    {
        public Body FollowThis;
        public float FollowDistance;

        public override void ControlledUpdate()
        {
            base.ControlledUpdate();

            if (FollowThis.transform.position.x - FollowDistance > transform.position.x)
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(RunSpeed, GetComponent<Rigidbody2D>().velocity.y);
                Animator.SetFloat("Speed", RunSpeed);
            }
            else
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(0, GetComponent<Rigidbody2D>().velocity.y);
                Animator.SetFloat("Speed", 0);
            }
        }

        private void EnemyWeaponHit(Weapon weapon)
        {
            if (!weapon.Active || InvicibilityTimer > 0)
            {
                return;
            }
            
            Damage(weapon.Damage, weapon.HitStun);

            weapon.Active = false;
            weapon.WeaponHit = true;
        }


        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "EnemyWeapon")
            {
                EnemyWeaponHit(other.GetComponent<Weapon>());
            }
        }

        public void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "EnemyWeapon")
            {
                EnemyWeaponHit(collision.GetComponent<Weapon>());
            }
        }

    }
}
