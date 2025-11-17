using UnityEngine;

namespace FullMetalAcorn {
	public class OneShotAnimation : MonoBehaviour {
		private float progress = 0;
		public Sprite[] frames;
		public SpriteRenderer sprite;

		void Update() {
			progress += Time.deltaTime;

			int frameIndex = (int) (progress * 10);

			if (frameIndex >= frames.Length) {
				this.gameObject.SetActive(false);
				
				return;
			}

			sprite.sprite = frames[frameIndex];
		}

		void OnEnable() {
			progress = 0;
		}
	}
}
