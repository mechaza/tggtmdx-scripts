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
using Grawly.Battle;
using Grawly.DungeonCrawler;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Grawly.Special.PurpleCube {
	
	/// <summary>
	/// A controller class of sorts that manages the Purple Cube fight.
	/// </summary>
	public class PurpleCubeFightController : SerializedMonoBehaviour {

		#region FIELDS - TOGGLES
		/// <summary>
		/// Should the battle begin when the scene starts?
		/// </summary>
		[TabGroup("General", "Toggles"), SerializeField]
		private bool beginBattleOnStart = false;
		/// <summary>
		/// The BattleTemplate to initiate this fight with.
		/// </summary>
		[TabGroup("General", "Toggles"), SerializeField]
		private BattleTemplate battleTemplate;
		#endregion

		#region FIELDS - BEHAVIORS
		/// <summary>
		/// The behavior that should be used when setting up the battle arena.
		/// </summary>
		[OdinSerialize, TabGroup("Battle", "Arena Behaviors"), Title("Arena Setup"), InlineProperty, HideLabel]
		private BattleArenaSetup BattleArenaSetup { get; set; }
		/// <summary>
		/// The behavior this battle should use for the Battle Navigator.
		/// </summary>
		[OdinSerialize, TabGroup("Battle", "Arena Behaviors"), Title("Navigator"), InlineProperty, HideLabel]
		private NavigatorBehavior NavigatorBehavior { get; set; }
		/// <summary>
		/// The intro that should be used for this battle.
		/// E.x., how the game transitions from dungeon to battle, or otherwise.
		/// </summary>
		[OdinSerialize, TabGroup("Battle", "Arena Behaviors"), Title("Intro"), InlineProperty, HideLabel]
		private BattleIntro BattleIntro { get; set; }
		/// <summary>
		/// The outro that should be used for this battle.
		/// Handles if I'm transitioning to like, the results screen or whatever.
		/// </summary>
		[OdinSerialize, TabGroup("Battle", "Arena Behaviors"), Title("Outro"), InlineProperty, HideLabel]
		private BattleOutro BattleOutro { get; set; }
		/// <summary>
		/// The DropParser to use when parsing. The results of the battle.
		/// </summary>
		[OdinSerialize, TabGroup("Battle", "Arena Behaviors"), Title("Drop Parser"), InlineProperty, HideLabel]
		private BattleDropParser BattleDropParser { get; set; }
		#endregion
		
		#region FIELDS - MODIFIERS
		/// <summary>
		/// A list of battle modifiers to use by default.
		/// </summary>
		[OdinSerialize, TabGroup("Battle", "Modifiers"), Title("Modifiers"), ListDrawerSettings(Expanded = true)]
		private List<BattleModifier> battleModifiers = new List<BattleModifier>();
		/// <summary>
		/// A list of battle modifiers to use by default.
		/// Clones the list in the template so it doesnt affect the scriptableobject.
		/// </summary>
		private List<BattleModifier> BattleModifiers {
			get {
				Debug.Log("Cloning BattleModifiers...");
				return this.battleModifiers.Select(m => m.Clone()).ToList();
			}
		}
		#endregion

		#region UNITY CALLS
		private void Start() {
			// If set to begin the battle on start, do so.
			if (this.beginBattleOnStart == true) {
				GameController.Instance.RunEndOfFrame(action: () => {
					this.StartBattle();
				});
			}
		}
		#endregion
		
		#region MAIN CALLS
		/// <summary>
		/// Begins the battle with the purple cube controller.
		/// </summary>
		[Button, HideInEditorMode]
		public void StartBattle() {
			
			CrawlerActionPrompt.Instance?.Dismiss();
			
			BattleController.Instance.StartBattle(
				battleTemplate: this.battleTemplate,
				battleParams: this.GenerateBattleParams());
			
		}
		#endregion

		#region HELPERS
		/// <summary>
		/// A way of generating battle params for this class.
		/// </summary>
		/// <returns></returns>
		private BattleParams GenerateBattleParams() {
			// Return a new set 
			return new BattleParams() {
				ArenaSetup = this.BattleArenaSetup,
				BattleIntro = this.BattleIntro,
				BattleOutro = this.BattleOutro,
				DropParser = this.BattleDropParser,
				NavigatorBehavior = this.NavigatorBehavior.Clone(),
				BattleModifiers = this.BattleModifiers
			};
		}
		#endregion
		
	}

}