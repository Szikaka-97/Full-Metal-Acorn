using UnityEngine;

namespace FullMetalAcorn {
	public class Moveable : Griddable {
		[SerializeField]
		private int movementRange;

		void Start() {
			SnapToGrid();
		}

		void Update() {

		}

        public override void OnClick() {
            
        }
	}
}
