using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace FullMetalAcorn {
	public class GroundController : MonoBehaviour {
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

		private GroundTile currentHoveredTile;

		private List<GroundTile> tiles = new List<GroundTile>();
		private List<SpriteRenderer> tileRenderers = new List<SpriteRenderer>();

		public GroundTile GetTileAt(int x, int y) {
			int index = y * groundSize.x + y + x;

			if (index < this.tiles.Count) {
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
			if (!tile) {
				this.highlight.gameObject.SetActive(false);

				return;
			}

			this.highlight.gameObject.SetActive(true);
			this.highlight.transform.position = tile.position + Vector2.down * tileTopCenter;
			this.highlight.sortingOrder = tile.layer;
		}

		void Update() {

		}
	}
}
