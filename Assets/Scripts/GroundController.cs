using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace FullMetalAcorn {
	public class GroundController : MonoBehaviour {
		private class PathfindNode {
			public GroundTile tile;
			public PathfindNode previousNode;

			public PathfindNode(GroundTile tile, PathfindNode previous) {
				this.tile = tile;
				this.previousNode = previous;
			}
		}

		[SerializeField]
		private GameObject groundTileSpritePrefab;

		[SerializeField]
		private Vector2 groundPosition;
		[SerializeField]
		private Vector2Int groundSize;
		[SerializeField]
		private float tileStep = 0.25f;
		[SerializeField]
		private float tileTopCenter = 0.75f;
		[SerializeField]
		private SpriteRenderer highlight;

		private InputAction mousePosAction;

		private GroundTile currentHighlightedTile;

		private List<GroundTile> tiles = new List<GroundTile>();
		private List<SpriteRenderer> tileRenderers = new List<SpriteRenderer>();

		public GroundTile GetTileAt(Vector2Int gridPos) {
			return GetTileAt(gridPos.x, gridPos.y);
		}

		public GroundTile GetTileAt(int x, int y) {
			int index = y * (this.groundSize.x + 1) + x;

			if (index >= 0 && index < this.tiles.Count) {
				return this.tiles[index];
			}
			return null;
		}

		public void RegenerateTerrain() {
			foreach (SpriteRenderer sprite in this.tileRenderers) {
				Destroy(sprite);
			}

			float currentHeight = this.groundPosition.y - (this.groundSize.y / 2) - 1.0f - this.tileStep;

			int tileRowCount = (int) Mathf.Ceil(this.groundSize.y / this.tileStep) + 1;

			this.tileRenderers.Clear();
			this.tiles.Clear();

			for (int tileY = 0; tileY < tileRowCount; tileY++, currentHeight += this.tileStep) {
				GameObject rowObject = GameObject.Instantiate(this.groundTileSpritePrefab, this.transform);
				rowObject.name = "background_row_" + tileY;
				rowObject.SetActive(true);

				SpriteRenderer rowSprite = rowObject.GetComponent<SpriteRenderer>();
				rowSprite.drawMode = SpriteDrawMode.Tiled;
				rowSprite.size = new Vector2(this.groundSize.x + (tileY % 2), 1);
				rowSprite.transform.position = new Vector3(
					this.groundPosition.x,
					currentHeight
				);
				rowSprite.sortingOrder = tileRowCount - tileY;

				this.tileRenderers.Add(rowSprite);

				for (int tileX = 0; tileX < this.groundSize.x + (tileY % 2); tileX++) {
					float xOffset = tileX - (this.groundSize.x - (1 - (tileY % 2))) / 2.0f;

					GroundTile tile = new GroundTile();
					tile.position = new Vector2(this.groundPosition.x + xOffset, currentHeight + tileTopCenter);
					tile.gridPosition = new Vector2Int(tileX - ((tileY + 1) / 2), tileY);
					tile.walkable = true;
					tile.layer = tileRowCount - tileY;

					this.tiles.Add(tile);
				}
			}

			this.groundTileSpritePrefab.SetActive(false);
		}

		void Awake() {
			Assert.IsNotNull(this.groundTileSpritePrefab);
			Assert.IsNotNull(this.groundTileSpritePrefab.GetComponent<SpriteRenderer>());
			Assert.AreNotEqual(this.tileStep, 0.0f);

			RegenerateTerrain();
			
			this.highlight.gameObject.SetActive(false);

			this.mousePosAction = InputSystem.actions.FindAction("Point");
		}

		public GroundTile[] FindPath(Vector2Int start, Vector2Int end) {
			return FindPath(GetTileAt(start.x, start.y), GetTileAt(end.x, end.y));
		}

		public GroundTile[] FindPath(GroundTile start, Vector2Int end) {
			return FindPath(start, GetTileAt(end.x, end.y));
		}

		public GroundTile[] FindPath(Vector2Int start, GroundTile end) {
			return FindPath(GetTileAt(start.x, start.y), end);
		}

		public GroundTile[] FindPath(GroundTile start, GroundTile end) {
			int GetTileIndex(GroundTile t) {
				return t.gridPosition.y * (this.groundSize.x + 1) + t.gridPosition.x;
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

			PathfindNode currentNode = new PathfindNode(start, null);

			Queue<PathfindNode> movementQueue = new Queue<PathfindNode>();
			movementQueue.Enqueue(currentNode);

			Vector2Int[] neighbours = {
				Vector2Int.left + Vector2Int.up,
				Vector2Int.right + Vector2Int.down,
				Vector2Int.down,
				Vector2Int.up
			};

			do {
				currentNode = movementQueue.Dequeue();
				
				for (int i = 0; i < 4; i++) {
					GroundTile next = GetTileAt(currentNode.tile.gridPosition + neighbours[i]);
					if (next && !visitedStates[GetTileIndex(next)]) {
						visitedStates[GetTileIndex(next)] = true;

						movementQueue.Enqueue(new PathfindNode(next, currentNode));
					}
				}
			} while (currentNode.tile != end && movementQueue.Count > 0);

			List<PathfindNode> pathNodes = new List<PathfindNode>();

			if (currentNode.tile == end) {
				while (currentNode != null) {
					pathNodes.Add(currentNode);

					currentNode = currentNode.previousNode;
				}
			}

			GroundTile[] path = new GroundTile[pathNodes.Count];

			for (int i = 0; i < pathNodes.Count; i++) {
				path[i] = pathNodes[pathNodes.Count - 1 - i].tile;
			}

			return path.ToArray();
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

		public void HighlightTile(GroundTile tile) {
			if (tile != this.currentHighlightedTile) {
				if (this.currentHighlightedTile && this.currentHighlightedTile.occupant) {
					this.currentHighlightedTile.occupant.OnExitHover();
				}

				if (tile && tile.occupant) {
					tile.occupant.OnEnterHover();
				}

				this.currentHighlightedTile = tile;
			}

			if (!tile) {
				this.highlight.gameObject.SetActive(false);

				return;
			}

			this.highlight.gameObject.SetActive(true);
			this.highlight.transform.position = tile.position + Vector2.down * tileTopCenter;
			this.highlight.sortingOrder = tile.layer;
		}
	}
}
