using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FullMetalAcorn {
	public class GriddableMover : MonoBehaviour {
		private Moveable current;

		private LinkedList<GridPath> activePaths = new LinkedList<GridPath>();

		private InputAction mouseClickAction;

		class GridPath {
			public GroundTile[] tiles;
			public Moveable mover;
			public float progress;

			public GridPath(GroundTile[] tiles, Moveable mover) {
				this.tiles = tiles;
				this.mover = mover;
				this.progress = 0;
			}

			public bool Finished => progress >= tiles.Length - 1;
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
						GroundTile[] path = ground.FindPath(this.current.Tile, tile);

						if (path != null) {
							this.activePaths.AddLast(new GridPath(
								path,
								this.current
							));

							path[path.Length - 1].walkable = false;

							this.current.Tile = null;

							this.current = null;
						}
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

			List<GridPath> pathsToRemove = new List<GridPath>();

			foreach (var activePath in this.activePaths) {
				activePath.progress += Time.deltaTime * activePath.mover.MovementSpeed;

				if (activePath.Finished) {
					activePath.mover.transform.position = activePath.tiles[activePath.tiles.Length - 1].position;
					activePath.tiles[activePath.tiles.Length - 1].walkable = true;
					activePath.mover.Tile = activePath.tiles[activePath.tiles.Length - 1];

					pathsToRemove.Add(activePath);

					continue;
				}

				int startPoint = Mathf.FloorToInt(activePath.progress);
				int endPoint = Mathf.CeilToInt(activePath.progress);
				float frac = activePath.progress % 1.0f;

				activePath.mover.transform.position = Vector3.Lerp(
					activePath.tiles[startPoint].position,
					activePath.tiles[endPoint].position,
					Mathf.SmoothStep(0.0f, 1.0f, frac)
				);
			}

			foreach (GridPath removed in pathsToRemove) {
				this.activePaths.Remove(removed);
			}
		}
	}
}
