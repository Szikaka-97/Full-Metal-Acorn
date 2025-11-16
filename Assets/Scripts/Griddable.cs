using UnityEngine;

namespace FullMetalAcorn {
	public class Griddable : MonoBehaviour {
		[SerializeField]
		private int m_size = 1;

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

		public int Size {
			get => m_size;
			set => m_size = value;
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

			TileEventManager.Emit(new GriddableMovementEvent() {
				actor = this,
				to = this.Tile
			});
		}

		public virtual void OnEnterHover() { }

		public virtual void OnExitHover() { }

		public virtual void OnClick() { }
	}
}
