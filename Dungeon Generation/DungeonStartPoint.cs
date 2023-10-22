using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Grawly.Dungeon.Generation {

	/// <summary>
	/// Kinda hacking this together because I'm an idiot. Will refactor later.
	/// </summary>
	public class DungeonStartPoint : MonoBehaviour {
		
		public static DungeonStartPoint Instance {
			get {
				return GameObject.FindObjectOfType<DungeonStartPoint>();
			}
		}

		#region UNITY CALLS
		private void Start() {
			
		}
		#endregion

	}


}