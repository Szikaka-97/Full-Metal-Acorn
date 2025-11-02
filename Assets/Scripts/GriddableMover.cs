using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FullMetalAcorn {
	public class GriddableMover : MonoBehaviour {
		private Moveable current;

		private InputAction mouseClickAction;

		class GridPath {
			List<GroundController> tiles;
			Moveable mover;
		}

		public bool TryStartMoving(Moveable mover) {
			if (!this.current) {
				this.current = mover;
				
				return true;
			}

			return false;
		}

		void Awake() {
			mouseClickAction = InputSystem.actions.FindAction("Click");
		}

		void Update() {
			GroundController ground = Level.Instance.Ground;

			if (ground.TryGetTileUnderCursor(out var tile)) {
				if (this.current && tile.IsFree()) {
					ground.HighlightTile(tile);

					if (this.mouseClickAction.WasPressedThisFrame()) {
						this.current.transform.position = tile.position;
						this.current.Tile = tile;

						this.current = null;
					}
				}
				else if (!this.current && tile.occupant && tile.occupant is Moveable) {
					ground.HighlightTile(tile);

					if (this.mouseClickAction.WasPressedThisFrame()) {
						this.current = tile.occupant as Moveable;
					}
				}
				else {
					ground.HighlightTile(null);
				}
			}

			if (this.current) {

			}
		}
	}
}
