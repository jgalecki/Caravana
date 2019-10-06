using Assets.Scripts.IO;
using Assets.Scripts.Logging;
using Assets.Scripts.Screens.Platform_Screen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Screens.Desert
{
    public class DesertScreen : GameScreen
    {
        static readonly ILog _log = Log.GetLogger(typeof(DesertScreen));

        public PlayerController Player;
        public List<Enemy> Enemies;
        public List<Pickup> Pickups;
        public List<Camel> Camels;

        static Faded _fadedPrefab;

        static Pickup _heartPrefab;
        static Pickup _gemPrefab;
        private const int GEM_CHANCE = 80; // chances of 100 to get gems on beating enemies
        private const int DOUBLE_GEM_CHANCE = 20;
        private const int HEART_CHANCE = 15;

        public Text GemCounter;

        public GameOver GameOverScreen;
        public PauseScreen PauseScreen;

        public enum WinCondition { NoEnemies, SurviveTime };
        public WinCondition VictoryCondition;
        private float _onVictoryTimer;
        private const float VICTORY_LAP = 2;
        public Text VictoryText;

        public static int MAX_X = 20;
        public static int MIN_Y = -20;

        public AudioClip Malaguena;
        public AudioClip Combat1;
        public AudioClip Combat2;

        public override void Initialize()
        {
            base.Initialize();

            var song = PlayerModel.Random.Next(3);
            switch (song)
            {
                case 0:
                    SoundManager.PlayMusic(Malaguena);
                    break;
                case 1:
                    SoundManager.PlayMusic(Combat1);
                    break;
                case 2:
                    SoundManager.PlayMusic(Combat2);
                    break;

            }

            Player.Initialize();

            if (_fadedPrefab == null)
            {
                _fadedPrefab = Resources.Load<Faded>("Faded");
            }

            if (PlayerModel.DialogCount > 2)
            {
                for (int i = 0; i < PlayerModel.DialogCount; i++)
                {
                    var faded = Instantiate(_fadedPrefab);
                    faded.transform.parent = transform;
                    faded.transform.position = new Vector3(PlayerModel.Random.Next(12, 20), 0);
                    faded.AttitudeType = Enemy.Attitude.Wander;

                    Enemies.Add(faded);
                }
            }

            foreach (var enemy in Enemies)
            {
                enemy.Initialize();
                Physics2D.IgnoreCollision(enemy.GetComponent<BoxCollider2D>(), Player.GetComponent<BoxCollider2D>());

                enemy.SetTargets(Enemies, Camels, Player);

                foreach (var otherEnemy in Enemies)
                {
                    Physics2D.IgnoreCollision(otherEnemy.GetComponent<BoxCollider2D>(), enemy.GetComponent<BoxCollider2D>());
                }
            }

            foreach (var camel in Camels)
            {
                camel.Initialize();
                Physics2D.IgnoreCollision(camel.GetComponent<BoxCollider2D>(), Player.GetComponent<BoxCollider2D>());

                foreach (var enemy in Enemies)
                {
                    Physics2D.IgnoreCollision(camel.GetComponent<BoxCollider2D>(), enemy.GetComponent<BoxCollider2D>());
                }
            }

            if (_heartPrefab == null)
            {
                _heartPrefab = Resources.Load<Pickup>("HeartPickup");
            }
            if (_gemPrefab == null)
            {
                _gemPrefab = Resources.Load<Pickup>("Pickup Variant Gem");
            }
        }

        public override void ControlledUpdate()
        {
            base.ControlledUpdate();

            Player.ControlledUpdate();

            foreach (var enemy in Enemies)
            {
                enemy.ControlledUpdate();
            }

            foreach (var camel in Camels)
            {
                camel.ControlledUpdate();
            }

            foreach (var pickup in Pickups)
            {
                pickup.ControlledUpdate();
            }            

            GemCounter.text = "x " + PlayerModel.Gems.ToString();

            for (int i = 0; i < Enemies.Count; i++)
            {
                var enemy = Enemies[i];
                if (enemy.Health <= 0)
                {
                    SoundManager.PlaySingle(enemy.DeathSound);
                    CreatePickups(enemy.transform.position);
                    Enemies.Remove(enemy);
                    i--;
                    Destroy(enemy.gameObject);
                }
            }
            Enemies.RemoveAll(x => x.Health <= 0);

            for (int i = 0; i < Camels.Count; i++)
            {
                var camel = Camels[i];
                if (camel.Health <= 0)
                {
                    SoundManager.PlaySingle(camel.DeathSound);
                    Camels.Remove(camel);
                    i--;
                    Destroy(camel.gameObject);
                }
            }
            Enemies.RemoveAll(x => x.Health <= 0);

            for (int i = 0; i < Pickups.Count; i++)
            {
                var pickup = Pickups[i];
                if (pickup.Taken)
                {
                    Pickups.Remove(pickup);
                    i--;
                    Destroy(pickup.gameObject);
                }
            }

            if (Player.Health <= 0)
            {
                SoundManager.PlaySingle(Player.DeathSound);
                ScreenManager.AddScreen(GameOverScreen);
            }

            CheckVictoryCondition();
        }

        private void CreatePickups(Vector3 position)
        {
            if (PlayerModel.Random.Next(100) < GEM_CHANCE)
            {
                var gem = Instantiate(_gemPrefab);
                SpawnPickup(gem, position);
                if (PlayerModel.Random.Next(100) < DOUBLE_GEM_CHANCE)
                {
                    gem = Instantiate(_gemPrefab);
                    SpawnPickup(gem, position);
                }
            }
            if (PlayerModel.Random.Next(100) < HEART_CHANCE)
            {
                var heart = Instantiate(_heartPrefab);
                SpawnPickup(heart, position);
            }            
        }

        private void SpawnPickup(Pickup pickup, Vector3 position)
        {
            position = new Vector3(position.x, position.y, 0);
            pickup.transform.position = position;
            pickup.transform.parent = transform;
            int maxVelocity = 4;
            float xVel = PlayerModel.Random.Next(maxVelocity * 10 * 2) / 10f - maxVelocity;
            float yVel = PlayerModel.Random.Next(maxVelocity * 10) / 10f;
            pickup.GetComponent<Rigidbody2D>().velocity = new Vector2(xVel, yVel);
            Pickups.Add(pickup);
            pickup.Initialize();
        }

        public override void HandleInput()
        {
            base.HandleInput();

            Player.HandleInput();

            if (InputHelp.GetButtonDown(InputHelp.Buttons.Start))
            {
                ScreenManager.AddScreen(PauseScreen);
            }
        }

        public void CheckVictoryCondition()
        {
            if (_onVictoryTimer > 0)
            {
                _onVictoryTimer -= Time.deltaTime;
                if (_onVictoryTimer <= 0)
                {
                    ScreenManager.LoadScene("Dialog");
                }
                return;
            }
            switch(VictoryCondition)
            {
                case WinCondition.NoEnemies:
                    
                    if (Enemies.Count == 0)
                    {
                        // Play victory jingle
                        SoundManager.PlayConfirm();
                        VictoryText.gameObject.SetActive(true);
                        _onVictoryTimer = VICTORY_LAP;
                    }
                    break;
                case WinCondition.SurviveTime:
                    break;
            }
        }
    }
}
