using UnityEngine;

namespace FullMetalAcorn {
	public class OneShotAnimation : MonoBehaviour {
		private float progress = 0;
		public Sprite[] frames;
		public SpriteRenderer sprite;

        public ActiveAttackType actorAttackType ;
        private void Start()
        {

            actorAttackType = FindFirstObjectByType<ActiveAttackType>();

            if (actorAttackType == null)
                Debug.LogError("No ActiveAttackType found in scene!");
        }


        void Update() {
			progress += Time.deltaTime;

			int frameIndex = (int) (progress * 10);

			if (frameIndex >= frames.Length) {
				this.gameObject.SetActive(false);
				
				return;
			}

			switch(actorAttackType.selected_attack_type)
            {
                case AttackType.MELEE:
                    break;
                case AttackType.RANGED:
                    sprite.color = Color.pink;
                    break;
                case AttackType.SPECIAL:
                    sprite.color = Color.purple;
                    break;
            }

            sprite.sprite = frames[frameIndex];
		}

		void OnEnable() {
			progress = 0;
		}
	}
}
