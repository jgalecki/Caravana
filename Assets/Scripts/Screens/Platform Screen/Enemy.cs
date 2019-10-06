using Assets.Scripts.Logging;
using Assets.Scripts.Screens.Desert;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Screens.Platform_Screen
{
    public class Enemy : Body
    {
        static readonly ILog _log = Log.GetLogger(typeof(Enemy));

        public enum Attitude { Wander, Idle }
        public Attitude AttitudeType;

        public const float QUICKER_FALLING = 1.5f;

        public bool HasTarget;
        public Body Target;
        public float TargetDistance;

        public float AggroRange;
        public float AttackRange;

        public bool IsOverlapSlowing;
        private float _overlapSlowingTimer;
        private const float OVERLAP_SLOW_TIME = 0.75f;
        private const float OVERLAP_SLOWING_MAGNITUDE = 0.75f;
        private const float OVERLAP_THRESHOLD = 0.3f;
        private float _checkOverlapTimer;
        private const float CHECK_OVERLAP_CYCLE = 0.75f;

        public float MovementModifier;

        public Vector3 Destination;
        public bool HasDestination;
        public float CLOSE_ENOUGH_TO_DESTINATION = 1.5f;
        private float _shortIdleTimer;
        private const float SHORT_IDLE_TIME = 1;

        public List<Enemy> Buddies;
        public List<Camel> Camels;
        public PlayerController Player;

        public void SetTargets(List<Enemy> enemies, List<Camel> camels, PlayerController player)
        {
            Player = player;
            Buddies = enemies.Where(x => x.name != name).ToList();
            Camels = camels;
        }

        public override void ControlledUpdate()
        {
            base.ControlledUpdate();

            Falling();

            if (_checkOverlapTimer <= 0)
            {
                Buddies.RemoveAll(x => x == null);

                _checkOverlapTimer = CHECK_OVERLAP_CYCLE;
                bool tooClose = Buddies.Count(x => !x.IsOverlapSlowing && Math.Abs(x.transform.position.x - transform.position.x) <= OVERLAP_THRESHOLD) > 0;
                if (tooClose)
                {
                    IsOverlapSlowing = true;
                    _overlapSlowingTimer = OVERLAP_SLOW_TIME;
                    MovementModifier = OVERLAP_SLOWING_MAGNITUDE;
                }
            }
            else
            {
                _checkOverlapTimer -= Time.deltaTime;
            }

            if (_overlapSlowingTimer > 0)
            {
                _overlapSlowingTimer -= Time.deltaTime;
                if (_overlapSlowingTimer <= 0)
                {
                    IsOverlapSlowing = false;
                    MovementModifier = 1;
                }
            }
        
            if (HasTarget)
            {
                if (Target == null)
                {
                    Debug.Log("HasTarget but Target is null. Target killed?");
                    FindTarget();
                }
                return;
            }

            if (AttitudeType == Attitude.Idle)
            {
                // sit around until Target is in aggro range.
                FindTarget();
                return;
            }

            if (HasDestination)
            {
                if (Math.Abs(Destination.x - transform.position.x) <= CLOSE_ENOUGH_TO_DESTINATION)
                {
                    HasDestination = false;
                    _shortIdleTimer = SHORT_IDLE_TIME;
                    GetComponent<Rigidbody2D>().velocity = new Vector2(0, GetComponent<Rigidbody2D>().velocity.y);
                    Animator.SetFloat("Speed", 0);
                }
                else
                {
                    int xDirection = transform.position.x - Destination.x > 0 ? -1 : 1;
                    _faceLeft = xDirection < 0;
                    GetComponent<Rigidbody2D>().velocity = new Vector2(xDirection * RunSpeed * MovementModifier / 2f, GetComponent<Rigidbody2D>().velocity.y);

                    Animator.SetFloat("Speed", RunSpeed * MovementModifier / 2f);
                }
            }
            else
            {
                if (_shortIdleTimer > 0)
                {
                    _shortIdleTimer -= Time.deltaTime;
                }
                else
                {
                    HasDestination = true;
                    Destination = new Vector3(PlayerModel.Random.Next(DesertScreen.MAX_X * 2 * 10) / 10 - DesertScreen.MAX_X, 0);
                }
            }

            FindTarget();
        }

        private void FindTarget()
        {
            var bodies = new List<Body>();
            bodies.AddRange(Camels);
            bodies.Add(Player);
            bodies = bodies.OrderBy(x => Vector3.Distance(x.transform.position, transform.position)).ToList();

            //_log.Write("{0} is looking for a target", name);

            if (Vector3.Distance(bodies[0].transform.position, transform.position) < AggroRange)
            {
                Target = bodies[0];
                HasTarget = true;
                _log.Write("{0} found target {1}", name, Target.name);
            }
        }

        private void Falling()
        {
            var velocity = GetComponent<Rigidbody2D>().velocity;

            if (velocity.y >= 0) return;

            velocity += Vector2.up * Physics2D.gravity * QUICKER_FALLING * Time.deltaTime;
            Animator.SetBool("IsFalling", true);
            GetComponent<Rigidbody2D>().velocity = velocity;
        }

        private void PlayerWeaponHit(PlayerWeapon weapon)
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


            FindTarget();
        }


        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "PlayerWeapon")
            {
                PlayerWeaponHit(other.GetComponent<PlayerWeapon>());
            }
        }


        public void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "PlayerWeapon")
            {
                PlayerWeaponHit(collision.GetComponent<PlayerWeapon>());
            }
        }
    }
}
