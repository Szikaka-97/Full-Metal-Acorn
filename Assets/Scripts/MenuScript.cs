using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Codice.Client.Common;

namespace FullMetalAcorn
{
    public class MenuScript : MonoBehaviour
    {

        [SerializeField] private Slider healthbar;

        public Scene current_scene;
        public string next_scene;
        public Button next_scene_button;
        private int maxHealth;

        private void OnEnable()
        {
            GameController.OnPlayerSpawned += SetupHealthbar;
            PlayerHealth.OnPlayerTakeDamage += UpdateHealthbar;
        }

        private void SetupHealthbar(GameObject player)
        {
            healthbar.value = healthbar.maxValue;
            maxHealth = player.GetComponent<PlayerHealth>().maxHealth;
        }

        private void UpdateHealthbar(int currentHealth)
        {
            healthbar.value = (float)currentHealth / maxHealth;
            healthbar.value = Mathf.Clamp01(healthbar.value);
        }

        private void OnDisable()
        {
            GameController.OnPlayerSpawned -= SetupHealthbar;
            PlayerHealth.OnPlayerTakeDamage -= UpdateHealthbar;
        }

        public void LoadNextScene()
        {
            SceneManager.LoadScene(next_scene);
        }

        public void activeMeleeAttack()
        {
        }
        public void activeRangedAttack()
        {
        }
        public void activeSpecialAttack()
        {
        }
    }
}