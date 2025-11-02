using UnityEngine;

namespace FullMetalAcorn {
	public class Level : MonoBehaviour {
		public static Level Instance {
			get;
			private set;
		}

		[SerializeField]
		private GroundController m_ground;

		public GroundController Ground {
			get => m_ground;
		}
		
		void Awake() {
			Instance = this;
		}
	}
}
