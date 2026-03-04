using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FullMetalAcorn {
	public class AttackTypeSelector : MonoBehaviour {
		[SerializeField]
		private AttackManager attackManager;
		[SerializeField]
		private AttackSelectButton attackButtonPrefab;

		private	Actor selectedUnit;
		private List<AttackSelectButton> attackButtons = new List<AttackSelectButton>();
		private AttackSelectButton activeButton;

		private void SelectAttack(Attack attack) {
			attackManager.CurrentAttack(attack);
		}		

		private void AttackButtonClick(AttackSelectButton butt) {
			if (this.activeButton) {
				this.activeButton.UpdateText(false);
			}

			this.activeButton = butt;

			butt.UpdateText(true);

			SelectAttack(butt.attack);
		}

		private void ClearButtons() {
			this.activeButton = null;

			if (this.attackButtons.Count > 0) {
				foreach (var button in this.attackButtons) {
					Destroy(button.gameObject);
				}

				this.attackButtons.Clear();
			}
		}

		public void DisplayActorAttackSelect(Actor unit) {
			ClearButtons();

			if (this.selectedUnit == unit) {
				this.selectedUnit = null;

				return;
			}

			foreach (Attack att in unit.Attacks) {
				var button = Instantiate(this.attackButtonPrefab, this.attackButtonPrefab.transform.parent);

				button.attack = att;

				button.UpdateText(false);

				this.attackButtons.Add(button);

				button.onClick.AddListener(() => AttackButtonClick(button));

				button.gameObject.SetActive(true);
			}

			this.selectedUnit = unit;
		}

		public void OnUnitClicked(TileClickEvent e) {
			if (e.affectedTile.occupant && e.affectedTile.occupant is Actor && (e.affectedTile.occupant as Actor).IsPlayerTeam) {
				DisplayActorAttackSelect(e.affectedTile.occupant as Actor);
			}
			else {
				ClearButtons();

				this.selectedUnit = null;
			}
		}

		void OnEnable() {
			TileEventManager.Subscribe(OnUnitClicked);
		}

		void OnDisable() {
			TileEventManager.Unsubscribe(OnUnitClicked);
		}
	}
}