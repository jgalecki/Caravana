using Assets.Scripts.IO;
using Assets.Scripts.Logging;
using Assets.Scripts.Screens.Platform_Screen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Screens
{
    public class PlayerController : Body
    {
        static readonly ILog _log = Log.GetLogger(typeof(PlayerController));
        
        public float JumpVelocity;
        public float JumpContinueVelocity;
        private float _jumpContinueTimer;
        public float MaxJumpContinue;
        public float QuickerFalling;
        
        public AudioClip JumpSound;
        public AudioClip DaggerSound;

        public PlayerWeapon Dagger;
        private const float DAGGER_X_OFFSET_RIGHT = -0.53799f;
        private const float DAGGER_X_OFFSET_LEFT = -1.48f;
        private const float DAGGER_Y_OFFSET = -0.38074f;
        
        public override void Initialize()
        {
            base.Initialize();

            //HealthBar.Initialize(MAX_HEALTH);
        }

        public override void ControlledUpdate()
        {
            base.ControlledUpdate();
        }

        public void HandleInput()
        {
            if (_jumpContinueTimer > 0)
            {
                _jumpContinueTimer -= Time.deltaTime;
            }
            if (_attackTimer > 0)
            {
                _attackTimer -= Time.deltaTime;
            }

            if (HitStunTimer > 0)
            {
                Animator.SetBool("IsAttacking", false);
                Dagger.Active = false;
                Dagger.WeaponHit = false;
            }

            Falling();

            if (HitStunTimer > 0) return;

            float horVel = Moving();
            var velocity = new Vector2(RunSpeed * horVel, GetComponent<Rigidbody2D>().velocity.y);

            velocity.y = Jumping(velocity);

            Animator.SetFloat("Vert Speed", velocity.y);
            GetComponent<Rigidbody2D>().velocity = velocity;

            Attacking();

            //if (InputHelp.GetButtonDown(InputHelp.Buttons.Down) && HitStunTimer <= 0)
            //{
            //    Damage(1);
            //}
        }

        private void Attacking()
        {
            if (_attackTimer > 0)
            {
                if (_attackTimer <= AttackCooldown - AttackDuration)
                {
                    Animator.SetBool("IsAttacking", false);
                    Dagger.Active = false;
                    Dagger.WeaponHit = false;
                }
                else if (_attackTimer <= AttackCooldown - HitBoxDelay && !Dagger.WeaponHit)
                {
                    Dagger.Active = true;
                    if (_faceLeft)
                    {
                        Dagger.GetComponent<BoxCollider2D>().offset = new Vector2(DAGGER_X_OFFSET_LEFT, DAGGER_Y_OFFSET);
                    }
                    else
                    {
                        Dagger.GetComponent<BoxCollider2D>().offset = new Vector2(DAGGER_X_OFFSET_RIGHT, DAGGER_Y_OFFSET);
                    }
                }
                else
                {
                    Dagger.Active = false;
                }

            }
            else if (InputHelp.GetButtonDown(InputHelp.Buttons.Attack))
            {
                _attackTimer = AttackCooldown;
                Animator.SetBool("IsAttacking", true);
                SoundManager.PlaySingle(DaggerSound);
            }
        }

        private float Moving()
        {
            float horVel;
            if (InputHelp.GetButton(InputHelp.Buttons.Left))
            {
                horVel = -1;
                _faceLeft = true;
            }
            else if (InputHelp.GetButton(InputHelp.Buttons.Right))
            {
                horVel = 1;
                _faceLeft = false;
            }
            else
            {
                horVel = 0;
            }

            Animator.SetFloat("Speed", Math.Abs(horVel));
            return horVel;
        }

        private void Falling()
        {
            var velocity = GetComponent<Rigidbody2D>().velocity;

            if (velocity.y >= -0.01) return;
            
            velocity += Vector2.up * Physics2D.gravity * QuickerFalling * Time.deltaTime;
            Animator.SetBool("IsFalling", true);
            GetComponent<Rigidbody2D>().velocity = velocity;
        }

        private float Jumping(Vector2 velocity)
        {
            if (InputHelp.GetButtonDown(InputHelp.Buttons.Jump) && _onGround)
            {
                velocity.y = JumpVelocity;
                SoundManager.PlaySingle(JumpSound);
                _jumpContinueTimer = MaxJumpContinue;

                Animator.SetBool("IsJumping", true);
                Animator.SetBool("IsStartOfJumpOver", false);
            }
            else if (_jumpContinueTimer > 0 && InputHelp.GetButton(InputHelp.Buttons.Jump))
            {
                velocity.y += JumpContinueVelocity * Time.deltaTime;
            }
            else
            {
                Animator.SetBool("IsStartOfJumpOver", true);
            }

            return velocity.y;
        }

        public override void OnSolidCollisionEnter(Collision2D collision)
        {
            base.OnSolidCollisionEnter(collision);
            
            Animator.SetBool("IsJumping", false);
            Animator.SetBool("IsFalling", false);
        }


        private void EnemyWeaponHit(EnemyWeapon weapon)
        {
            if (!weapon.Active || InvicibilityTimer > 0)
            {
                return;
            }
            
            Damage(weapon.Damage, weapon.HitStun);

            int xDirection = transform.position.x - weapon.ParentPosition.x > 0 ? 1 : -1;
            GetComponent<Rigidbody2D>().velocity = new Vector2(xDirection * weapon.LaunchX, weapon.LaunchY);
            _faceLeft = xDirection > 0;

            weapon.Active = false;
            weapon.WeaponHit = true;
        }


        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "EnemyWeapon")
            {
                EnemyWeaponHit(other.GetComponent<EnemyWeapon>());
            }
        }

        public void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "EnemyWeapon")
            {
                EnemyWeaponHit(collision.GetComponent<EnemyWeapon>());
            }
        }
    }
}
