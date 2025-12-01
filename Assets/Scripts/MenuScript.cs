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

        //private static Actor current;

        //public  AttackType selected_attack_type = AttackType.MELEE;
        public ActiveAttackType actorAttackType;

        private void Start()
        {

            actorAttackType = FindFirstObjectByType<ActiveAttackType>();

            if (actorAttackType == null)
                Debug.LogError("No ActiveAttackType found in scene!");
        }

        public void LoadNextScene()
        {

            SceneManager.LoadScene(next_scene);
        }

        public void activeMeleeAttack()
        {
            //selected_attack_type = AttackType.MELEE;
            //Debug.Log("Melee attack selected");
            if (actorAttackType != null)
                actorAttackType.ActivateMeleeAttack();
            else
                Debug.Log("actorAttackType is null");
        }
        public void activeRangedAttack()
        {
            //selected_attack_type = AttackType.RANGED;
            //Debug.Log("Ranged attack selected");
            if (actorAttackType != null)
                actorAttackType.ActivateRangedAttack();
        }
        public void activeSpecialAttack()
        {
            //selected_attack_type = AttackType.SPECIAL;
            //Debug.Log("Special attack selected");
            if (actorAttackType != null)
                actorAttackType.ActivateSpecialAttack();
        }
    }
}
