using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace FullMetalAcorn {
	[RequireComponent(typeof(GroundRenderer))]
	[ExecuteInEditMode]
	public class GroundController : MonoBehaviour {
		private class PathfindNode {
			public GroundTile tile;
			public PathfindNode previousNode;
			public int distance;

			public PathfindNode(GroundTile tile, PathfindNode previous, int distance) {
				this.tile = tile;
				this.previousNode = previous;
				this.distance = distance;
			}
		}

		[SerializeField]
		private Vector2 groundPosition;
		[SerializeField]
		private Vector2 groundSize;
		[SerializeField]
		private float tileStep = 0.25f;
		[SerializeField]
		private float tileTopCenter = 0.75f;
		private InputAction mousePosAction;
		private InputAction mouseClickAction;

		private Vector2Int tileCounts;
		private List<GroundTile> tiles = new List<GroundTile>();
		private GroundTile hoveredTile;

		public GroundTile GetTileAt(Vector2Int gridPos) {
			return GetTileAt(gridPos.x, gridPos.y);
		}

		public GroundTile GetTileAt(int x, int y) {
			int origY = y - x;
			int origX = x + (origY / 2);

			if (origX < 0 || origX >= this.tileCounts.x || origY < 0 || origY >= this.tileCounts.y) {
				return null;
			}

			int index = origY * this.tileCounts.x + origX;
			
			return this.tiles[index];
		}

		private Vector2Int GetTileCounts() {
			return new Vector2Int(Mathf.CeilToInt(this.groundSize.x), Mathf.CeilToInt(this.groundSize.y / this.tileStep) + 1);
		}

		public void RegenerateTerrain() {
			float currentHeight = this.groundPosition.y - (this.groundSize.y * 0.5f) + (this.tileTopCenter - 0.5f) * 0.5f;

			this.tileCounts = GetTileCounts();

			this.tiles.Clear();

			for (int tileY = 0; tileY < tileCounts.y; tileY++, currentHeight += this.tileStep) {
				for (int tileX = 0; tileX < this.tileCounts.x; tileX++) {
					float xOffset = tileX + (tileY % 2) * 0.5f - (this.groundSize.x * 0.5f);

					GroundTile tile = new GroundTile();
					tile.position = new Vector2(this.groundPosition.x + xOffset, currentHeight);
					tile.gridPosition = new Vector2Int(tileX - tileY / 2, tileY + tileX - tileY / 2);
					tile.walkable = true;
					tile.layer = tileCounts.y - tileY;

					this.tiles.Add(tile);
				}
			}
		}

		private void OnGriddableMove(GriddableMovementEvent e) {
			int griddableSize = e.actor.Size;

			if (e.from) {
				GroundTile from = e.from;

				for (int x = 0; x < griddableSize; x++) {
					for (int y = 0; y < griddableSize; y++) {
						GroundTile near = GetTileAt(from.gridPosition + new Vector2Int(-x, y));

						if (near) {
							near.occupant = null;
						}
					}
				}
			}

			if (e.to) {
				GroundTile to = e.to;

				for (int x = 0; x < griddableSize; x++) {
					for (int y = 0; y < griddableSize; y++) {
						GroundTile near = GetTileAt(to.gridPosition + new Vector2Int(-x, y));

						if (near) {
							near.occupant = e.actor;
						}
					}
				}
			}
		}

		void Awake() {
			Assert.AreNotEqual(this.tileStep, 0.0f);

			RegenerateTerrain();
			
			this.mousePosAction = InputSystem.actions.FindAction("Mouse Point");
			this.mouseClickAction = InputSystem.actions.FindAction("Mouse Click");
		}

		void Update() {
			if (GetTileCounts() != this.tileCounts) {
				RegenerateTerrain();
				
				this.mousePosAction = InputSystem.actions.FindAction("Point");
				this.mouseClickAction = InputSystem.actions.FindAction("Click");
			}

			if (TryGetTileUnderCursor(out GroundTile newHoveredTile)) {
				if (newHoveredTile != this.hoveredTile) {
					if (this.hoveredTile) {
						TileEventManager.Emit(new TileHoverEvent() {
							newState = TileHoverState.Exit,
							affectedTile = this.hoveredTile
						});
					}

					TileEventManager.Emit(new TileHoverEvent() {
						newState = TileHoverState.Enter,
						affectedTile = newHoveredTile
					});

					this.hoveredTile = newHoveredTile;
				}

				if (this.mouseClickAction.WasPressedThisFrame()) {
					TileEventManager.Emit(new TileClickEvent() {
						affectedTile = newHoveredTile
					});
				}
			}
			else if (this.hoveredTile) {
				TileEventManager.Emit(new TileHoverEvent() {
					newState = TileHoverState.Exit,
					affectedTile = this.hoveredTile
				});

				this.hoveredTile = null;
			}

			GetComponentInChildren<GroundRenderer>(true).UpdateVisual(this.tileCounts, this.tiles);
		}

		public GroundTile[] FindPath(Vector2Int start, Vector2Int end, Moveable actor, bool ignoreRange = false) {
			return FindPath(GetTileAt(start.x, start.y), GetTileAt(end.x, end.y), actor, ignoreRange);
		}

		public GroundTile[] FindPath(GroundTile start, Vector2Int end, Moveable actor, bool ignoreRange = false) {
			return FindPath(start, GetTileAt(end.x, end.y), actor, ignoreRange);
		}

		public GroundTile[] FindPath(Vector2Int start, GroundTile end, Moveable actor, bool ignoreRange = false) {
			return FindPath(GetTileAt(start.x, start.y), end, actor, ignoreRange);
		}

		public GroundTile[] FindPath(GroundTile start, GroundTile end, Moveable actor, bool ignoreRange = false) {
			int GetTileIndex(GroundTile t) {
				int origY = t.gridPosition.y - t.gridPosition.x;
				int origX = t.gridPosition.x + (origY / 2);

				return origY * this.tileCounts.x + origX;
			}

			if (start == null || end == null) {
				Debug.LogError("Start and end tiles cannot be null");

				return null;
			}

			if (start == end) {
				return new GroundTile[0];
			}

			bool[] visitedStates = new bool[this.tiles.Count];

			visitedStates[GetTileIndex(start)] = true;

			PathfindNode currentNode = new PathfindNode(start, null, 0);

			Queue<PathfindNode> movementQueue = new Queue<PathfindNode>();
			movementQueue.Enqueue(currentNode);

			Vector2Int[] neighbours = {
				Vector2Int.left,
				Vector2Int.right,
				Vector2Int.down,
				Vector2Int.up
			};

			do {
				currentNode = movementQueue.Dequeue();

				if (!ignoreRange && currentNode.distance > actor.MovementRange) {
					return null;
				}

				neighbours = neighbours.OrderBy( direction => -Vector2.Dot(direction, end.gridPosition - currentNode.tile.gridPosition) + Vector2.Dot(direction, Vector2.right) * 0.01f ).ToArray();
				
				for (int i = 0; i < 4; i++) {
					GroundTile next = GetTileAt(currentNode.tile.gridPosition + neighbours[i]);

					if (next && !visitedStates[GetTileIndex(next)]) {
						bool free = true;

						for (int x = 0; x < actor.Size && free; x++) {
							for (int y = 0; y < actor.Size && free; y++) {
								GroundTile near = GetTileAt(next.gridPosition + new Vector2Int(-x, y));

								if (near && !near.IsFree() && near.occupant != actor) {
									free = false;
								}
							}
						}

						if (!free) {
							continue;
						}

						visitedStates[GetTileIndex(next)] = true;

						movementQueue.Enqueue(new PathfindNode(next, currentNode, currentNode.distance + 1));
					}
				}
			} while (currentNode.tile != end && movementQueue.Count > 0);

			List<PathfindNode> pathNodes = new List<PathfindNode>();

			if (currentNode.tile == end) {
				while (currentNode != null) {
					pathNodes.Add(currentNode);

					currentNode = currentNode.previousNode;
				}

				GroundTile[] path = new GroundTile[pathNodes.Count];

				for (int i = 0; i < pathNodes.Count; i++) {
					path[i] = pathNodes[pathNodes.Count - 1 - i].tile;
				}

				return path.ToArray();
			}
			else {
				return null;
			}
		}

		public bool TryGetTileOnPosition(Vector2 position, out GroundTile targetTile) {
			bool found = false;
			targetTile = default;

			foreach (var tile in this.tiles) {
				float tileTopBorder = tile.position.y - this.tileTopCenter + 1.0f;

				float d1 = -2.0f * (tile.position.y - tileTopBorder) * (position.x - tile.position.x) + tileTopBorder;
				float d2 = 2.0f * (tile.position.y - tileTopBorder) * (position.x - tile.position.x) + tileTopBorder;
				float d3 = 2.0f * (tile.position.y - tileTopBorder) * (position.x - tile.position.x) - tileTopBorder + 2 * tile.position.y;
				float d4 = -2.0f * (tile.position.y - tileTopBorder) * (position.x - tile.position.x) - tileTopBorder + 2 * tile.position.y;

				if (
					position.y < d1
					&&
					position.y < d2
					&&
					position.y > d3
					&&
					position.y > d4
				) {
					targetTile = tile;
					found = true;
					
					break;
				}
			}

			return found;
		}

		public GroundTile GetClosestTile(Vector2 position) {
			float minDistance = float.PositiveInfinity;
			GroundTile targetTile = default;

			foreach (var tile in this.tiles) {
				float tileDistance = Vector2.Distance(position, targetTile.position);

				if (tileDistance < minDistance) {
					minDistance = tileDistance;
					targetTile = tile;
				}
			}

			return targetTile;
		}

		public bool TryGetTileUnderCursor(out GroundTile tile) {
			Vector3 mousePos = this.mousePosAction.ReadValue<Vector2>();

			Vector2 pointedPosition = Camera.main.ScreenToWorldPoint(mousePos);

			return TryGetTileOnPosition(pointedPosition, out tile);
		}

		public void OnEnable() {
			TileEventManager.Subscribe(OnGriddableMove);
		}

		public void OnDisable() {
			TileEventManager.Unsubscribe(OnGriddableMove);
		}

		public void OnDrawGizmos() {
// 			int tileIndex = 0;

// 			foreach (var tile in this.tiles) {
// 				Gizmos.DrawSphere(tile.position, 0.1f);
// #if UNITY_EDITOR
// 				UnityEditor.Handles.Label((Vector3) tile.position + Vector3.up * 0.15f, tileIndex.ToString() + ": " + tile.gridPosition.ToString() + " (" + (GetTileAt(tile.gridPosition) == tile).ToString() + ")");

// 				tileIndex++;
// #endif
// 			}
		}
	}
}
