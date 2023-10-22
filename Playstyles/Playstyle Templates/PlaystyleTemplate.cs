using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Menus.Input;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using Grawly.Menus.NameEntry;
using System.Linq;
using DG.Tweening;
using Grawly.UI;
using Sirenix.Serialization;

namespace Grawly.Playstyle {
	
	/// <summary>
	/// Contains information on different defining characteristics of a Playstyle.
	/// </summary>
	/// <remarks>
	/// It can be assumed that these will be static assets that shouldn't ever be changed at runtime.
	/// Or ever, really. They're just here to make it easy for me to redefine what different playstyles do.
	/// </remarks>
	[CreateAssetMenu(menuName = "Grawly/Playstyle/Playstyle Template")]
	public class PlaystyleTemplate : SerializedScriptableObject {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The name associated with this playstyle template.
		/// </summary>
		[OdinSerialize]
		public string PlaystyleName { get; private set; } = "";
		/// <summary>
		/// The playstyle type associated with this template.
		/// </summary>
		[OdinSerialize]
		public PlaystyleType PlaystyleType { get; private set; } = PlaystyleType.None;
		/// <summary>
		/// The tutorial to present to the player when this playstyle is picked on the selection screen.
		/// </summary>
		[OdinSerialize]
		public TutorialTemplate PlaystyleTutorial { get; private set; }
		/// <summary>
		/// An icon to represent what this playstyle should look like when selecting it.
		/// </summary>
		[OdinSerialize]
		public Sprite PlaystyleIcon { get; private set; }
		/// <summary>
		/// A description outlining this playstyle.
		/// </summary>
		[OdinSerialize, MultiLineProperty()]
		public string PlaystyleDescription { get; private set; } = "";
		#endregion

	}
}