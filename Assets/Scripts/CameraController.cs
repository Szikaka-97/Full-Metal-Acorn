using UnityEngine;
using UnityEngine.InputSystem;

namespace FullMetalAcorn {
	class CameraController : MonoBehaviour {
		[SerializeField]
		private Camera gameCamera;
		[SerializeField]
		private Rect cameraMoveBounds = new Rect(-16, -9, 32, 18);
		[SerializeField]
		private float cameraMoveSpeed = 1;

		private InputAction cameraMoveAction;

		void Awake() {
			this.cameraMoveAction = InputSystem.actions.FindAction("Camera Move");
		}

		void Update() {
			if (this.gameCamera) {
				Rect worldSpaceRect = new Rect(
					this.transform.TransformPoint(cameraMoveBounds.position),
					this.transform.TransformVector(cameraMoveBounds.size)
				);

				Rect cameraRect = new Rect(
					Vector3.zero,
					new Vector3(this.gameCamera.aspect, 1.0f) * this.gameCamera.orthographicSize * 2.0f
				);

				Vector2 cameraMovement = this.cameraMoveAction.ReadValue<Vector2>() * cameraMoveSpeed;

				Vector3 camPos = this.gameCamera.transform.position + (Vector3) cameraMovement;

				camPos.x = Mathf.Clamp(camPos.x, worldSpaceRect.min.x + cameraRect.width / 2, worldSpaceRect.max.x - cameraRect.width / 2);
				camPos.y = Mathf.Clamp(camPos.y, worldSpaceRect.min.y + cameraRect.height / 2, worldSpaceRect.max.y - cameraRect.height / 2);

				this.gameCamera.transform.position = camPos;
			}
		}

		void OnDrawGizmos() {
			Rect cameraRect = new Rect(
				this.gameCamera.transform.position - new Vector3(this.gameCamera.aspect, 1.0f) * this.gameCamera.orthographicSize,
				new Vector3(this.gameCamera.aspect, 1.0f) * this.gameCamera.orthographicSize * 2.0f
			);

			Color prev = Gizmos.color;
			Gizmos.color = Color.green;

			Gizmos.DrawWireCube(cameraRect.center, cameraRect.size);

			Gizmos.color = prev;

			Gizmos.DrawWireCube(cameraMoveBounds.center, cameraMoveBounds.size);
		}
	}
}