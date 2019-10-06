using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Screens.Platform_Screen
{
    public class Body : MonoBehaviour
    {
        protected bool _faceLeft;
        protected bool _onGround;

        public bool ShowHealthBar;
        public Healthbar HealthBar;

        public int MaxHealth;
        public int Health;
        
        public BoxCollider2D HurtBox;

        public float RunSpeed;

        public float _attackTimer;
        public float HitBoxDelay;
        public float AttackCooldown;
        public float AttackDuration;

        public const float STANDARD_HIT_STUN = 0.5f;
        public float HitStunTimer;
        public const float INVICIBILITY_AFTER_HURT = 1;
        public float InvicibilityTimer;

        public const float HURT_FLASH_ALPHA = 0.5f;
        public const float FLASH_POINT_ONE = 0.8f;
        public const float FLASH_POINT_TWO = 0.6f;
        public const float FLASH_POINT_THREE = 0.4f;
        public const float FLASH_POINT_FOUR = 0.2f;

        public AudioClip HitSound;
        public AudioClip DeathSound;
        public Animator Animator;

        public virtual void Initialize()
        {
            Health = MaxHealth;
            HealthBar.Initialize(Health);
        }

        public virtual void ControlledUpdate()
        {
            GetComponent<SpriteRenderer>().flipX = _faceLeft;

            if (HitStunTimer > 0)
            {
                HitStunTimer -= Time.deltaTime;
                if (HitStunTimer <= 0)
                {
                    Animator.SetBool("IsHurt", false);
                }
            }
            if (InvicibilityTimer > 0)
            {
                InvicibilityTimer -= Time.deltaTime; 

                if ((InvicibilityTimer <= FLASH_POINT_ONE && InvicibilityTimer > FLASH_POINT_TWO)
                    || (InvicibilityTimer <= FLASH_POINT_THREE && InvicibilityTimer > FLASH_POINT_FOUR))
                {
                    var rend = GetComponent<SpriteRenderer>();
                    rend.color = new Color(1, 1, 1, HURT_FLASH_ALPHA);
                }
                else 
                {
                    var rend = GetComponent<SpriteRenderer>();
                    rend.color = new Color(1, 1, 1, 1);
                }
            }

            HealthBar.gameObject.SetActive(Health != MaxHealth);
        }

        public void Damage(int damage, float hurtStun = STANDARD_HIT_STUN)
        {
            if (InvicibilityTimer > 0) return;

            Animator.SetBool("IsHurt", true);
            ChangeHealth(-1 * damage);
            HitStunTimer = hurtStun;
            InvicibilityTimer = INVICIBILITY_AFTER_HURT;

            SoundManager.PlaySingle(HitSound);

            if (_attackTimer > 0)
            {
                _attackTimer = 0.000001f;       // Will turn off attack on next frame;
            }
        }

        public void ChangeHealth(int healthChange)
        {
            if (Health == MaxHealth && healthChange > 0) return;

            Health += healthChange;
            HealthBar.Set(Health);
        }

        public void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag == "Collidable")
            {
                OnSolidCollisionEnter(collision);
            }
            if (collision.gameObject.tag == "Player")
            {
                OnPlayerCollisionEnter(collision);
            }
            if (collision.gameObject.tag == "Enemy")
            {
                OnEnemyCollisionEnter(collision);
            }
            if (collision.gameObject.tag == "PlayerWeapon")
            {
                OnPlayerWeaponCollisionEnter(collision);
            }
        }

        public void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.tag == "Collidable")
            {
                OnSolidCollisionExit(collision);
            }
            if (collision.gameObject.tag == "Player")
            {
                OnPlayerCollisionExit(collision);
            }
            if (collision.gameObject.tag == "Enemy")
            {
                OnEnemyCollisionExit(collision);
            }
        }

        public void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.gameObject.tag == "PlayerWeapon")
            {
                OnPlayerWeaponCollisionStay(collision);
            }
        }


        public virtual void OnSolidCollisionEnter(Collision2D collision)
        {
            _onGround = true;
        }

        public virtual void OnSolidCollisionExit(Collision2D collision)
        {
            _onGround = false;

        }

        public virtual void OnPlayerCollisionExit(Collision2D collision) { }
        public virtual void OnEnemyCollisionExit(Collision2D collision) { }
        public virtual void OnPlayerWeaponCollisionEnter(Collision2D collision) { }

        public virtual void OnPlayerWeaponCollisionStay(Collision2D collision) { }

        public virtual void OnPlayerCollisionEnter(Collision2D collision) { }
        public virtual void OnEnemyCollisionEnter(Collision2D collision) { }
    }
}
