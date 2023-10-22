using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using Grawly.UI;

namespace Grawly.Battle.BattleMenu {
	
	/// <summary>
	/// One of the squares that rotates in the background of the all out attack screen.
	/// </summary>
	public class AllOutAttackBackgroundSquare : MonoBehaviour {
		
		#region FIELDS - TOGGLES
		/// <summary>
		/// The speed of rotation for the rotation pivot.
		/// </summary>
		[SerializeField]
		private Vector3 rotationSpeed = new Vector3();
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// Contains all other objects as children.
		/// </summary>
		[SerializeField, Title("Scene References")]
		private GameObject allObjects;
		#endregion

		#region UNITY CALLS
		private void Update() {
			this.allObjects.transform.Rotate(this.rotationSpeed * Time.deltaTime);
		}
		#endregion

	}
}