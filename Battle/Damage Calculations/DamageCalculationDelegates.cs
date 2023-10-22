using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.Battle {

	// This is just a simple namespace to define the headers of functions

	/// <summary>
	/// Modifies the state of the damage calculation in some way.
	/// Can be chained together with multiple passes to create dynamic calculations.
	/// </summary>
	public delegate DamageCalculationSet DamageCalculationPass(DamageCalculationSet damageCalculationSet);


}