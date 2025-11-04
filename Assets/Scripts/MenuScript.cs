using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace FullMetalAcorn
{
    public class MenuScript : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public Scene current_scene;
        public string next_scene;
        public Button next_scene_button;

        public void LoadNextScene() {
            
            SceneManager.LoadScene(next_scene);
        }
    }
}
