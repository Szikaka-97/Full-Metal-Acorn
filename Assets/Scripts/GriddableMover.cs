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

		void OnTileClicked(TileClickEvent e) {
			if (e.affectedTile.occupant is Moveable) {
				if (this.current == e.affectedTile.occupant) {
					this.current = null;
				}
				else {
					this.current = e.affectedTile.occupant as Moveable;
				}
			}
			else if (this.current && e.affectedTile.IsFree()) {
				GroundController ground = Level.Instance.Ground;

				GroundTile[] path = ground.FindPath(this.current.Tile, e.affectedTile, this.current.MovementRange);

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

		void OnTileHovered(TileHoverEvent e) {
			if (this.current && e.affectedTile.IsFree()) {
				GroundController ground = Level.Instance.Ground;

				GroundTile[] path = ground.FindPath(this.current.Tile, e.affectedTile, this.current.MovementRange);

				if (path != null) {
					string pathAsString = path[0].gridPosition.ToString();

					for (int i = 1; i < path.Length; i++) {
						pathAsString += " > " + path[i].gridPosition.ToString();
					}

					Debug.Log(pathAsString);

					DrawPath(path, Color.green);
				}
			}
		}

		void Update() {
			GroundController ground = Level.Instance.Ground;

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

				float smoothFrac = Mathf.SmoothStep(0.0f, 1.0f, frac);
				
				Vector3 nextPos = Vector3.Lerp(
					activePath.tiles[startPoint].position,
					activePath.tiles[endPoint].position,
					frac
				);

				nextPos += Vector3.up * (-frac * (frac - 1) * frac);

				activePath.mover.LastMovement = nextPos - activePath.mover.transform.position;
				activePath.mover.transform.position = nextPos;

				if (activePath.tiles.Length == activePath.mover.MovementRange + 1) {
					DrawPath(activePath.tiles, Color.red, startPoint + frac);
				}
				else {
					DrawPath(activePath.tiles, Color.green, startPoint + frac);
				}
			}

			foreach (GridPath removed in pathsToRemove) {
				this.activePaths.Remove(removed);
			}
		}

		void OnEnable() {
			TileEventManager.Subscribe(OnTileClicked);
			TileEventManager.Subscribe(OnTileHovered);
		}

		void OnDisable() {
			TileEventManager.Unsubscribe(OnTileClicked);
			TileEventManager.Subscribe(OnTileHovered);
		}
	}
}
