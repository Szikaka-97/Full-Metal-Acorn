using UnityEngine;

namespace FullMetalAcorn {
	public class GroundTile {
		public Vector2 position;
		public Vector2Int gridPosition;
		public bool walkable;
		public int layer;
		public Griddable occupant;
		public bool highlighted;

		public bool IsFree() {
			return this.walkable && !this.occupant;
		}

		public static implicit operator bool(GroundTile tile) {
			return tile != null;
		}
	}
}