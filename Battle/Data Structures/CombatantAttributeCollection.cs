using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.Battle {


	/// <summary>
	/// Contains five values for each of the different kinds of attributes a combatant may have.
	/// </summary>
	public struct CombatantAttributeCollection {


		#region FIELDS - RAW VALUES
		[SerializeField]
		private float ST;
		[SerializeField]
		private float MA;
		[SerializeField]
		private float EN;
		[SerializeField]
		private float AG;
		[SerializeField]
		private float LU;
		#endregion

		#region ACCESSORS
		/// <summary>
		/// Gets the value associated with the given attribute type.
		/// </summary>
		/// <param name="key">The kind of attribute to retrieve the value for.</param>
		public float this[AttributeType key] {
			get {
				switch (key) {
					case AttributeType.ST:
						return this.ST;
					case AttributeType.MA:
						return this.MA;
					case AttributeType.EN:
						return this.EN;
					case AttributeType.AG:
						return this.AG;
					case AttributeType.LU:
						return this.LU;
					default:
						Debug.LogError("Couldn't determine attribte type! Returning -1");
						return -1f;
				}
			}
		}
		#endregion






	}
}