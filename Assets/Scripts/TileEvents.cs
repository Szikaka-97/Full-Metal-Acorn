using System.Collections.Generic;

namespace FullMetalAcorn {
	public static class TileEventManager {
		delegate void TileHoverEventReceiver(TileHoverEvent e);
		delegate void TileClickEventReceiver(TileClickEvent e);

		private static LinkedList<TileHoverEventReceiver> hoverEventReceivers;
		private static LinkedList<TileClickEventReceiver> clickEventReceivers;

		static void Subscribe(TileHoverEventReceiver r) {
			if (r != null) {
				hoverEventReceivers.AddLast(r);
			}
		}
		static void Subscribe(TileClickEventReceiver r) {
			if (r != null) {
				clickEventReceivers.AddLast(r);
			}
		}
	}

	public class TileHoverEvent {

	}

	public class TileClickEvent {

	}
}