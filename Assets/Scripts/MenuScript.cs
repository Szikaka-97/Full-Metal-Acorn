using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Codice.Client.Common;

namespace FullMetalAcorn
{
    public class MenuScript : MonoBehaviour
    {

        public Scene current_scene;
        public string next_scene;
        public Button next_scene_button;

        public void LoadNextScene() {
            SceneManager.LoadScene(next_scene);
        }

        public void activeMeleeAttack() {
        }
        public void activeRangedAttack() {
        }
        public void activeSpecialAttack() {
        }
    }
}
