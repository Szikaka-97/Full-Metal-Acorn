using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace FullMetalAcorn {
	public class GroundController : MonoBehaviour {
		public struct GroundTile {
			public Vector2 position;
			public bool walkable;
			public int layer;
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

		private List<GroundTile> tiles = new List<GroundTile>();
		private List<SpriteRenderer> tileRenderers = new List<SpriteRenderer>();

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

				for (int tileX = 0; tileX < this.groundSize.x; tileX++) {
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
			targetTile = default(GroundTile);

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

		void Update() {
			Vector3 mousePos = this.mousePosAction.ReadValue<Vector2>();

			Vector2 pointedPosition = Camera.main.ScreenToWorldPoint(mousePos);

			if (TryGetTileOnPosition(pointedPosition, out GroundTile targetTile)) {
				this.highlight.gameObject.SetActive(true);
				this.highlight.transform.position = targetTile.position + Vector2.down * tileTopCenter;
				this.highlight.sortingOrder = targetTile.layer + 1;
			}
			else {
				this.highlight.gameObject.SetActive(false);
			}
		}
	}
}
