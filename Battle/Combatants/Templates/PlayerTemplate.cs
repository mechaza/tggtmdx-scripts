using System.Collections;
using System.Collections.Generic;
using Grawly.Battle.Equipment;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Grawly.Battle {
	[CreateAssetMenu(menuName = "Grawly/Combatant/Player")]
	public class PlayerTemplate : CombatantTemplate {

		/// <summary>
		/// The ID of this Player. Helpful so I can associate it with custom names.
		/// </summary>
		[TabGroup("General", "General")]
		public CharacterIDType characterIDType;
		/// <summary>
		/// Helps figure out if the player can participate in a tactics move.
		/// </summary>
		[TabGroup("General", "General")]
		public TacticsParticipantType participantType;
		/// <summary>
		/// The default persona for this combatant.
		/// </summary>
		[TabGroup("General", "General")]
		public PersonaTemplate personaTemplate;
		/// <summary>
		/// The default weapon ID for this combatant.
		/// </summary>
		[TabGroup("General", "General"), InfoBox(infoMessageType: InfoMessageType.Warning, message: "Note that the template is just here so that its ID can be referenced.")]
		public WeaponTemplate weaponTemplate;
		
		[TabGroup("General", "Graphics")]
		public Sprite bustUp;
		[TabGroup("General", "Graphics")]
		public Sprite playerStatusIcon;
		[TabGroup("General", "Graphics")]
		public Sprite cutIn;
		/// <summary>
		/// The icon for when the players are inside combatant selection menus. This was originally for tactics I think.
		/// </summary>
		[TabGroup("General", "Graphics")]
		public Sprite normalIcon;
		/// <summary>
		/// The icon for when the players are inside combatant selection menus. This was originally for tactics I think.
		/// </summary>
		[TabGroup("General", "Graphics")]
		public Sprite fadedIcon;
		/// <summary>
		/// The sprite to use when this player is presented in the badge board.
		/// </summary>
		[TabGroup("General", "Graphics")]
		public Sprite badgeBoardHeadshotSprite;
	}
}
