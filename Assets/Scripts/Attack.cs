using System;
using UnityEngine;

namespace FullMetalAcorn {
	public enum AttackEffectType {
		Damage,
		Heal
	}


	[Serializable]
	public class Attack {
		[Serializable]
		public struct AttackEffect {
			public AttackEffectType type;
			public float param;
		}
		public string name;
		public AttackEffect[] effects;
		public int range;
		public int splashRange;
	}
}