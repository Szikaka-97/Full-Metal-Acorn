using UnityEngine;

namespace FullMetalAcorn
{
    public enum AttackType
    {
        MELEE,
        RANGED,
        SPECIAL
    }

    public class ActiveAttackType : MonoBehaviour
    {
        public AttackType selected_attack_type = AttackType.MELEE;
        public void ActivateMeleeAttack()
        {
            selected_attack_type = AttackType.MELEE;
            Debug.Log("Melee attack selected");
        }
        public void ActivateRangedAttack()
        {
            selected_attack_type = AttackType.RANGED;
            Debug.Log("Ranged attack selected");
        }
        public void ActivateSpecialAttack()
        {
            selected_attack_type = AttackType.SPECIAL;
            Debug.Log("Special attack selected");
        }
    }
}