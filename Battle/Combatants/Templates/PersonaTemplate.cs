using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Grawly.Battle {
	[CreateAssetMenu(menuName = "Grawly/Combatant/Persona")]
	public class PersonaTemplate : CombatantTemplate {

		#region FIELDS
		/// <summary>
		/// The 
		/// </summary>
		[TabGroup("General", "Graphics")]
		[InlineEditor(InlineEditorModes.LargePreview)]
		public Sprite bustUp;
		[TabGroup("General", "Graphics")]
		public Sprite icon;
		/// <summary>
		/// The sprite to use when this player is presented in the badge board.
		/// </summary>
		[TabGroup("General", "Graphics")]
		public Sprite badgeBoardHeadshotSprite;
		[TabGroup("Moves", "Battle Behaviors")]
		public BattleBehavior standardAttackBehavior;
		/// <summary>
		/// Contains the moves that this persona unlocks as they level up.
		/// </summary>
		[TabGroup("Moves", "Level Up")]
		public List<LevelUpMove> levelUpMoves = new List<LevelUpMove>();
		[TabGroup("Moves", "Level Up"), MultiLineProperty(lines: 10), HideLabel, SerializeField]
		private string levelUpMovesNote = "";
		#endregion
		


		[SerializeField]
		private void BuildLevelUpMovesFromList() {
			List<string> strs = new List<string>(levelUpMovesNote.Split('\r', '\n'));
			this.battleBehaviors.Clear();
			// 2D)AssetDatabase.LoadAssetAtPath("Assets/Textures/texture.jpg", typeof(Texture2D));
			foreach (string str in strs) {
#if UNITY_EDITOR
				// Try to parse the move level.
				int moveLevel = 0;
				System.Int32.TryParse(str.Substring(startIndex: 0, length: 2), out moveLevel);
				// Also grab the move name.
				string moveName = str.Substring(startIndex: 3);

				// Find the battle behavior.
				BattleBehavior bb = (BattleBehavior)AssetDatabase.LoadAssetAtPath("Assets/_TGGTMDX/Definitions/Behaviors/All Moves/" + moveName + ".asset", typeof(BattleBehavior));

				// Create a level up move and assign the data.
				LevelUpMove levelUpMove = new LevelUpMove();
				levelUpMove.behavior = bb;
				levelUpMove.level = moveLevel;

				// Add it.
				this.levelUpMoves.Add(levelUpMove);
#endif
			}

			// When done, order them by level.
			this.levelUpMoves.OrderBy(lum => lum.level);
		}

	}

}
