using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.Gauntlet {

	/// <summary>
	/// Basically a callback that can get run after an event is triggered.
	/// Note that these will mostly be returned as closures inside of other events, so the reaction sequence
	/// isn't the only thing that the reactions will be dealing with. (I didn't word that right but I hope it makes some sense?)
	/// </summary>
	/// <param name="gauntletReactionSequence">The sequence that this reaction is running in.</param>
	public delegate void GauntletReaction(GauntletReactionSequence gauntletReactionSequence);

}