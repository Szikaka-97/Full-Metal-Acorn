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

		void Start() {
			SnapToGrid();
		}

		void Update() {

		}

        public override void OnClick() {
            
        }
	}
}
