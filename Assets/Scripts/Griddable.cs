using UnityEngine;

namespace FullMetalAcorn {
	public class Griddable : MonoBehaviour {
		private GroundTile m_tile;

		public GroundTile Tile {
			get => m_tile;
			set {
				if (m_tile) {
					m_tile.occupant = null;
				}
				if (value) {
					value.occupant = this;
				}
				m_tile = value;
			}
		}

		public void SnapToGrid() {
			GroundController ground = Level.Instance.Ground;

			if (ground.TryGetTileOnPosition(this.transform.position, out GroundTile tile)) {
				this.transform.position = tile.position;
				
				this.Tile = tile;
			}
			else {
				GroundTile closestTile = ground.GetClosestTile(this.transform.position);
				this.transform.position = closestTile.position;

				this.Tile = closestTile;
			}
		}

		public virtual void OnEnterHover() { }

		public virtual void OnExitHover() { }

		public virtual void OnClick() { }
	}
}
