using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Screens.Platform_Screen
{
    public class Faded : Enemy
    {
        public const int MAX_HEALTH = 5;


        public EnemyWeapon Weapon;
        public const float WEAPON_X_OFFSET_LEFT = -0.36f;
        public const float WEAPON_X_OFFSET_RIGHT = 0.37588f;
        public const float WEAPON_Y_OFFSET = 0.19093f;
        
        public override void Initialize()
        {
            base.Initialize();
            AttackRange -= PlayerModel.Random.Next(3) / 10f;
        }

        public override void ControlledUpdate()
        {
            base.ControlledUpdate();

            if (_attackTimer > 0)
            {
                _attackTimer -= Time.deltaTime;
                if (_attackTimer <= 0)
                {
                    Animator.SetBool("IsAttacking", false);
                    Weapon.Active = false;
                }
            }

            if (HitStunTimer > 0) return;
            if (!HasTarget) return;

            TargetDistance = Vector2.Distance(Target.transform.position, transform.position);
            if (AttackRange < TargetDistance)
            {
                ChaseTarget();
            }
            else if (TargetDistance <= AttackRange)
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(0, GetComponent<Rigidbody2D>().velocity.y);
                AttackTarget();
            }
            else
            {
                // chill
            }
            Animator.SetFloat("Speed", Math.Abs(GetComponent<Rigidbody2D>().velocity.x));
        }

        private void AttackTarget()
        {
            if (_attackTimer <= 0)
            {
                Animator.SetBool("IsAttacking", true);
                _attackTimer = AttackCooldown;
                Weapon.Active = false;
            }
            else if (_attackTimer <= AttackCooldown - AttackDuration)
            {
                Weapon.Active = false;
                Weapon.WeaponHit = false;
            }
            else if (_attackTimer < AttackCooldown - HitBoxDelay && !Weapon.WeaponHit)
            {
                Weapon.Active = true;
                Weapon.GetComponent<Collider2D>().offset = new Vector2(_faceLeft ? WEAPON_X_OFFSET_LEFT : WEAPON_X_OFFSET_RIGHT, WEAPON_Y_OFFSET);
            }

        }

        private void ChaseTarget()
        {
            int xDirection = transform.position.x - Target.transform.position.x > 0 ? -1 : 1;
            _faceLeft = xDirection < 0;
            GetComponent<Rigidbody2D>().velocity = new Vector2(xDirection * RunSpeed * MovementModifier, GetComponent<Rigidbody2D>().velocity.y);

            Animator.SetFloat("Speed", RunSpeed * MovementModifier);
        }
    }
}
