using UnityEngine;
using UnityEngine.InputSystem;

namespace FullMetalAcorn {
	class CameraController : MonoBehaviour {
		[SerializeField]
		private Camera gameCamera;
		[SerializeField]
		private Rect cameraMoveBounds = new Rect(-16, -9, 32, 18);
		[SerializeField]
		private float cameraScrollBoundary = 100;
		[SerializeField]
		private float cameraScrollSpeed = 1;

		private InputAction mousePosAction;

		void Awake() {
			this.mousePosAction = InputSystem.actions.FindAction("Point");
		}

		void Update() {
			if (this.gameCamera) {
				Vector2 cursorPos = this.mousePosAction.ReadValue<Vector2>();

				if (cursorPos.x <= 0 || cursorPos.x > Screen.width || cursorPos.y <= 0 || cursorPos.y > Screen.height) {
					return;
				}

				Rect worldSpaceRect = new Rect(
					this.transform.TransformPoint(cameraMoveBounds.position),
					this.transform.TransformVector(cameraMoveBounds.size)
				);

				Rect cameraRect = new Rect(
					this.gameCamera.transform.position - new Vector3(this.gameCamera.aspect, 1.0f) * this.gameCamera.orthographicSize,
					new Vector3(this.gameCamera.aspect, 1.0f) * this.gameCamera.orthographicSize * 2.0f
				);

				Vector2 cameraMovement = Vector2.zero;

				if (cursorPos.x < this.cameraScrollBoundary) {
					cameraMovement += Vector2.left * Mathf.Clamp(this.cameraScrollSpeed * Time.deltaTime, 0, cameraRect.xMin - worldSpaceRect.xMin);
				}
				if (cursorPos.x > Screen.width - this.cameraScrollBoundary) {
					cameraMovement += Vector2.right * Mathf.Clamp(this.cameraScrollSpeed * Time.deltaTime, 0, worldSpaceRect.xMax - cameraRect.xMax);
				}
				if (cursorPos.y < this.cameraScrollBoundary) {
					cameraMovement += Vector2.down * Mathf.Clamp(this.cameraScrollSpeed * Time.deltaTime, 0, cameraRect.yMin - worldSpaceRect.yMin);
				}
				if (cursorPos.y > Screen.height - this.cameraScrollBoundary) {
					cameraMovement += Vector2.up * Mathf.Clamp(this.cameraScrollSpeed * Time.deltaTime, 0, worldSpaceRect.yMax - cameraRect.yMax);
				}

				this.gameCamera.transform.position += (Vector3) cameraMovement;
			}
		}

		void OnDrawGizmos() {
			Rect cameraRect = new Rect(
				this.gameCamera.transform.position - new Vector3(this.gameCamera.aspect, 1.0f) * this.gameCamera.orthographicSize,
				new Vector3(this.gameCamera.aspect, 1.0f) * this.gameCamera.orthographicSize * 2.0f
			);

			Gizmos.DrawWireCube(cameraRect.center, cameraRect.size);

			Color prev = Gizmos.color;

			Gizmos.color = Color.green;

			float worldSpaceBoundaryX = (this.cameraScrollBoundary / Screen.width) * this.gameCamera.aspect * this.gameCamera.orthographicSize * 2.0f;
			float worldSpaceBoundaryY = (this.cameraScrollBoundary / Screen.height) * this.gameCamera.orthographicSize * 2.0f;

			Gizmos.DrawWireCube(
				new Vector3(
					cameraRect.xMin + worldSpaceBoundaryX / 2.0f,
					cameraRect.center.y
				),
				new Vector3(
					worldSpaceBoundaryX,
					cameraRect.height
				)
			);
			Gizmos.DrawWireCube(
				new Vector3(
					cameraRect.xMax - worldSpaceBoundaryX / 2.0f,
					cameraRect.center.y
				),
				new Vector3(
					worldSpaceBoundaryX,
					cameraRect.height
				)
			);
			Gizmos.DrawWireCube(
				new Vector3(
					cameraRect.center.x,
					cameraRect.yMin + worldSpaceBoundaryY / 2.0f
				),
				new Vector3(
					cameraRect.width,
					worldSpaceBoundaryY
				)
			);
			Gizmos.DrawWireCube(
				new Vector3(
					cameraRect.center.x,
					cameraRect.yMax - worldSpaceBoundaryY / 2.0f
				),
				new Vector3(
					cameraRect.width,
					worldSpaceBoundaryY
				)
			);

			Gizmos.color = prev;

			Gizmos.DrawWireCube(cameraMoveBounds.center, cameraMoveBounds.size);
		}
	}
}