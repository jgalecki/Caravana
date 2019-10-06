using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Screens.Platform_Screen
{
    public class Healthbar : MonoBehaviour
    {
        public SpriteRenderer Foreground;
        private float _maxHealth;

        public void Initialize(int maxHealth)
        {
            _maxHealth = maxHealth;
            Set(maxHealth);
        }

        public void Set(float currentHealth)
        {
            Foreground.gameObject.transform.localScale = new Vector2(currentHealth / _maxHealth, 1);
        }
    }
}
