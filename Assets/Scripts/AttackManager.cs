using System.Linq;
using UnityEngine;

namespace FullMetalAcorn {
	public class AttackManager : MonoBehaviour {
		[SerializeField]
		private LineRenderer m_attackLine;

		private Actor current;
		private Attack selectedAttack = null;

		private void SelectActor(Actor actor) {
			current = actor;
			selectedAttack = null;
		}

		public void CurrentAttack(Attack attack) {
			if (current && current.Attacks.Contains(attack)) {
				selectedAttack = attack;
			}
		}

		private void UnitAttackSelected(Attack attack) {
			selectedAttack = attack;
		}

		private void OnUnitClicked(TileClickEvent e) {
			if (e.affectedTile.occupant && e.affectedTile.occupant is Actor) {
				if ((e.affectedTile.occupant as Actor).IsPlayerTeam) {
					if (e.affectedTile.occupant == current) {
						SelectActor(null);
					}
					else {
						SelectActor(e.affectedTile.occupant as Actor);
					}
				}
				else if (current && selectedAttack != null && Vector2.Distance((Vector2) this.current.transform.position, (Vector2) e.affectedTile.occupant.transform.position) <= selectedAttack.range) {
					Actor enemy = e.affectedTile.occupant as Actor;

					enemy.Health -= 999;
					m_attackLine.enabled = false;
				}
			}
			else {
				this.current = null;
				this.selectedAttack = null;
			}
		}

		private void OnUnitHovered(TileHoverEvent e) {
			if (!this.current || this.selectedAttack == null || !e.affectedTile.occupant || e.affectedTile.occupant is not Actor || (e.affectedTile.occupant as Actor).IsPlayerTeam) {
				return;
			}

			if (Vector2.Distance((Vector2) this.current.transform.position, (Vector2) e.affectedTile.occupant.transform.position) > this.selectedAttack.range / 2) {
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
