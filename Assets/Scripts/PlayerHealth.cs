using System;
using UnityEngine;

namespace FullMetalAcorn
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private int health = 100;
        public int currentHealth { get; private set; }
        public int maxHealth { get; private set; }

        public static Action<int> OnPlayerTakeDamage;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Awake()
        {
            currentHealth = health;
            maxHealth = health;
        }
        

        public void TakeDamage(int damageAmmount)
        {
            currentHealth = -damageAmmount;
            OnPlayerTakeDamage?.Invoke(currentHealth);
            if (currentHealth <= 0)
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
