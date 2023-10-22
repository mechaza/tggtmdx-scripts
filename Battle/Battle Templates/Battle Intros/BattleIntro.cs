using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
#endif

namespace Grawly.Battle.Intros {

	/// <summary>
	/// I needed a way to transition to battles differently so I'm making this as part of the Battle Template to replace the old method I had hard coded in the Dungeon Controller.
	/// </summary>
	public abstract class BattleIntro {

		/// <summary>
		/// The way in which the battle should be prepped and run.
		/// </summary>
		/// <param name="template">The template that will be used for this battle. Technically, the BattleIntro is also part of this as well.</param>
		/// <param name="battleParams">The routine set that contains the ACTUAL routines to run.</param>
		public abstract void PlayIntro(BattleTemplate template, BattleParams battleParams);

	}

}