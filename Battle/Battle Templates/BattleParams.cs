using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Grawly.UI.Legacy;
using System.Linq;
using System.Reflection;
using System.Text;
using System;
using Grawly.UI;
using Grawly.Battle.Intros;
using Grawly.Battle.Outros;
using Grawly.Battle.BattleArena;
using Grawly.Battle.BattleArena.Setup;
using Grawly.Battle.BattleArena.Setup.Standard;
using Grawly.Battle.Modifiers;
using Sirenix.Serialization;
using Grawly.Battle.DropParsers;
using Grawly.Battle.Intros.Standard;
using Grawly.Battle.Navigator;
using Grawly.MiniGames.ShuffleTime;


namespace Grawly.Battle {
	
	/// <summary>
	/// When starting a battle, it will need a set of "behavior" functions.
	/// This helps encapsulate everything into one logical package.
	/// This is important if a battle is set up *NOT* using behaviors from the template.
	/// </summary>
	public class BattleParams {

		#region FIELDS - FLAGS
		/// <summary>
		/// The advantage type associated with this battle.
		/// </summary>
		public BattleAdvantageType BattleAdvantageType { get; set; } = BattleAdvantageType.Normal;
		/// <summary>
		/// Should the player be freed upon returning from the battle?
		/// </summary>
		public bool FreePlayerOnReturn { get; set; } = true;
		#endregion

		#region PROPERTIES - FLAGS
		/// <summary>
		/// Does this battle have an advantage type other than normal?
		/// </summary>
		public bool HasAdvantageType {
			get {
				return this.BattleAdvantageType != BattleAdvantageType.Normal;
			}
		}
		#endregion

		#region FIELDS - CALLBACKS
		/// <summary>
		/// A callback to invoke upon completion of the battle.
		/// </summary>
		public Action OnBattleComplete { get; set; } = null;
		/// <summary>
		/// A callback to invoke upon returning the main game after closing the results screen.
		/// </summary>
		public Action OnBattleReturn { get; set; } = null;
		#endregion
		
		#region FIELDS - BEHAVIORS
		/// <summary>
		/// The behavior that should be used when setting up the battle arena.
		/// </summary>
		public BattleArenaSetup ArenaSetup { get; set; }
		/// <summary>
		/// The behavior this battle should use for the Battle Navigator.
		/// </summary>
		public NavigatorBehavior NavigatorBehavior { get; set; }
		/// <summary>
		/// The intro that should be used for this battle.
		/// E.x., how the game transitions from dungeon to battle, or otherwise.
		/// </summary>
		public BattleIntro BattleIntro { get; set; }
		/// <summary>
		/// The outro that should be used for this battle.
		/// Handles if I'm transitioning to like, the results screen or whatever.
		/// </summary>
		public BattleOutro BattleOutro { get; set; }
		/// <summary>
		/// The DropParser to use when parsing. The results of the battle.
		/// </summary>
		public BattleDropParser DropParser { get; set; }
		#endregion

		#region FIELDS - MODIFIERS
		/// <summary>
		/// The BattleModifiers that should be run in this battle.
		/// </summary>
		public List<BattleModifier> BattleModifiers { get; set; }
		#endregion

	}


}