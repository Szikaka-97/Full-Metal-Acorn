using UnityEngine;

namespace FullMetalAcorn {
	public class AttackManager : MonoBehaviour {
		[SerializeField]
		private LineRenderer m_attackLine;

		private Actor current;

		private void OnUnitClicked(TileClickEvent e) {
			if (e.affectedTile.occupant && e.affectedTile.occupant is Actor) {
				if ((e.affectedTile.occupant as Actor).IsPlayerTeam) {
					current = e.affectedTile.occupant as Actor;
				}
				else if (current) {
					Actor enemy = e.affectedTile.occupant as Actor;

					enemy.Health -= 999;
					m_attackLine.enabled = false;
				}
			}
		}

		private void OnUnitHovered(TileHoverEvent e) {
			if (!this.current || !e.affectedTile.occupant || e.affectedTile.occupant is not Actor || (e.affectedTile.occupant as Actor).IsPlayerTeam) {
				return;
			}

			if (e.newState == TileHoverState.Enter) {
				Vector3[] linePoss = {
					this.current.transform.position,
					e.affectedTile.occupant.transform.position
				};

				m_attackLine.SetPositions(linePoss);
				m_attackLine.enabled = true;
			}
			else {
				m_attackLine.enabled = false;
			}
		}

		void OnEnable() {
			TileEventManager.Subscribe(OnUnitClicked);
			TileEventManager.Subscribe(OnUnitHovered);
		}

		void OnDisable() {
			TileEventManager.Unsubscribe(OnUnitClicked);
			TileEventManager.Unsubscribe(OnUnitHovered);
		}
	}
}
