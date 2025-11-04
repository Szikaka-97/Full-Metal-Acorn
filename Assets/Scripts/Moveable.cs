using UnityEngine;

namespace FullMetalAcorn {
	public class Moveable : Griddable {
		[SerializeField]
		private int movementRange;

		[SerializeField]
		private float movementSpeed;

		public float MovementSpeed {
			get => movementSpeed;
		}

		public int MovementRange {
			get => movementRange;
		}

		public Vector2 LastMovement { get; set; }

		void Start() {
			SnapToGrid();
		}

		void Update() {
			if (this.TryGetComponent<SpriteRenderer>(out var sprite)) {
				sprite.flipX = LastMovement.x < 0;
			}
		}

        public override void OnClick() {
            
        }
	}
}
