using UnityEngine;
using UI = UnityEngine.UI;
using TMPro;

namespace FullMetalAcorn {
	public class AttackSelectButton : MonoBehaviour {
		[SerializeField]
		private TextMeshProUGUI text;
		[System.NonSerialized]
		public Attack attack;

		public UI.Button.ButtonClickedEvent onClick {
			get {
				return GetComponentInChildren<UI.Button>().onClick;
			}
		}

		public void UpdateText(bool current) {
			if (text) {
				text.text = string.Format(
					"{0}\nRange: {1}\n" + (current ? "\nActive" : ""),
					attack.name, attack.range
				);
			}
		}
	}
}