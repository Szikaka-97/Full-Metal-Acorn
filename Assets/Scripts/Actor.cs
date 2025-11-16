using UnityEngine;

namespace FullMetalAcorn {
	public class Actor : Moveable{
		[SerializeField]
		private int m_health;

		[SerializeField]
		private bool m_playerTeam;
		
		public int Health {
			get => m_health;
			set {
				m_health = value;

				if (m_health <= 0) {
					m_health = 0;

					Die();
				}
			}
		}

        public override bool IsMouseMoveable => this.m_playerTeam;
		
		public void Die() {
			this.gameObject.SetActive(false);
		}
	}
}