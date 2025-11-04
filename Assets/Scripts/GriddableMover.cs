using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FullMetalAcorn {
	public class GriddableMover : MonoBehaviour {
		[SerializeField]
		private Mesh helperMesh;
		[SerializeField]
		private Material helperMaterial;

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

		void DrawPath(GroundTile[] path, Color color, float progress = 0) {
			for (int i = 0; i < path.Length - 1; i++) {
				GroundTile a = path[i];
				GroundTile b = path[i + 1];

				MaterialPropertyBlock props = new MaterialPropertyBlock();

				float rotation = 0;
				if ((b.position.x - a.position.x) * (b.position.y - a.position.y) > 0) {
					rotation = 1;
				}
				props.SetFloat("_Rotation", rotation);
				props.SetFloat("_Direction", b.position.x > a.position.x ? 0 : 1);
				props.SetColor("_BaseColor", color);
				props.SetFloat("_Progress", progress - i);

				RenderParams rp = new RenderParams(this.helperMaterial);
				rp.matProps = props;

				Graphics.RenderMesh(rp, this.helperMesh, 0, Matrix4x4.Translate((a.position + b.position) * 0.5f));
			}
		}

		void Update() {
			GroundController ground = Level.Instance.Ground;

			if (ground.TryGetTileUnderCursor(out var tile)) {
				if (this.current && tile.IsFree()) {
					GroundTile[] path = ground.FindPath(this.current.Tile, tile, this.current.MovementRange);

					if (path != null) {
						if (path.Length == this.current.MovementRange + 1) {
							DrawPath(path, Color.red);
						}
						else {
							DrawPath(path, Color.green);
						}

						ground.HighlightTile(tile);

						if (this.mouseClickAction.WasPressedThisFrame()) {
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
					else {
						ground.HighlightTile(null);
					}
				}
				else if (tile.occupant && tile.occupant is Moveable) {
					ground.HighlightTile(tile);

					if (this.mouseClickAction.WasPressedThisFrame()) {
						if (this.current == tile.occupant) {
							this.current = null;
						}
						else {
							this.current = tile.occupant as Moveable;
						}
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

				Vector3 nextPos = Vector3.Lerp(
					activePath.tiles[startPoint].position,
					activePath.tiles[endPoint].position,
					Mathf.SmoothStep(0.0f, 1.0f, frac)
				);

				activePath.mover.LastMovement = nextPos - activePath.mover.transform.position;
				activePath.mover.transform.position = nextPos;

				if (activePath.tiles.Length == activePath.mover.MovementRange + 1) {
					DrawPath(activePath.tiles, Color.red, startPoint + Mathf.SmoothStep(0.0f, 1.0f, frac));
				}
				else {
					DrawPath(activePath.tiles, Color.green, startPoint + Mathf.SmoothStep(0.0f, 1.0f, frac));
				}
			}

			foreach (GridPath removed in pathsToRemove) {
				this.activePaths.Remove(removed);
			}
		}
	}
}
