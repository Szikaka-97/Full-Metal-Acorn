using System.Collections.Generic;

namespace FullMetalAcorn {
	public enum TileHoverState {
		Enter,
		Exit
	}

	public enum GriddableMovementAction {
		Enter,
		Exit,
		Probing
	}

	public static class TileEventManager {
		public delegate void TileHoverEventReceiver(TileHoverEvent e);
		public delegate void TileClickEventReceiver(TileClickEvent e);
		public delegate void GriddableMovementEventReceiver(GriddableMovementEvent e);

		private static event TileHoverEventReceiver hoverEvents;
		private static event TileClickEventReceiver clickEvents;
		private static event GriddableMovementEventReceiver movementEvents;

		public static void Subscribe(TileHoverEventReceiver r) {
			if (r != null) {
				hoverEvents += r;
			}
		}
		public static void Subscribe(TileClickEventReceiver r) {
			if (r != null) {
				clickEvents += r;
			}
		}
		public static void Subscribe(GriddableMovementEventReceiver r) {
			if (r != null) {
				movementEvents += r;
			}
		}

		public static void Unsubscribe(TileHoverEventReceiver r) {
			if (r != null) {
				hoverEvents -= r;
			}
		}
		public static void Unsubscribe(TileClickEventReceiver r) {
			if (r != null) {
				clickEvents -= r;
			}
		}
		public static void Unsubscribe(GriddableMovementEventReceiver r) {
			if (r != null) {
				movementEvents -= r;
			}
		}

		public static void Emit(TileHoverEvent e) {
			hoverEvents(e);
		}

		public static void Emit(TileClickEvent e) {
			clickEvents(e);
		}

		public static void Emit(GriddableMovementEvent e) {
			movementEvents(e);
		}
	}

	public class TileHoverEvent {
		public TileHoverState newState;
		public GroundTile affectedTile;
	}

	public class TileClickEvent {
		public GroundTile affectedTile;
	}

	public class GriddableMovementEvent {
		public Griddable actor;
		public GroundTile from;
		public GroundTile to;
	}
}