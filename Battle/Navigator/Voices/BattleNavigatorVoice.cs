using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.Battle.Navigator {

	/// <summary>
	/// Contains the functions required to get the voice for the navigator.
	/// </summary>
	public abstract class BattleNavigatorVoice {

		#region MAIN CALLS
		/// <summary>
		/// Prepares this voice to be used.
		/// Includes things like, in the Instance of the text to speech, instansiating the prefab it requires.
		/// </summary>
		public abstract void Initialize();
		/// <summary>
		/// Instructs the navigator to speak.
		/// </summary>
		/// <param name="navigatorParams">The paramters involved in making this navigator speak.</param>
		public abstract void Speak(BattleNavigatorParams navigatorParams);
		#endregion

	}


}