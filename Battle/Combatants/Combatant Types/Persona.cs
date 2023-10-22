using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly;
using System.Linq;

namespace Grawly.Battle {

	public class Persona : Combatant {

		#region FIELDS - GRAPHICS
		public Sprite bustUp;
		public Sprite icon;
		public Sprite badgeBoardHeadshot;
		#endregion

		#region FIELDS - COMBATANT ANIMATOR
		/// <summary>
		/// The animator this Persona should use. Except. Personas do not have animators.
		/// </summary>
		public override ICombatantAnimator CombatantAnimator {
			get {
				throw new System.NotImplementedException("Personas do not have animators.");
			}
		}
		#endregion

		#region FIELDS - ATTRIBUTES AND MOVES
		/// <summary>
		/// Contains the moves this persona unlocks as they level up.
		/// </summary>
		public Queue<LevelUpMove> levelUpMoves = new Queue<LevelUpMove>();
		/// <summary>
		/// The standard attack behavior associated with this persona.
		/// </summary>
		public BattleBehavior standardAttackBehavior;
		#endregion

		#region PROPERTIES - LEVEL UP SKILLS
		/// <summary>
		/// Can this Persona learn their next level up move?
		/// </summary>
		public bool IsNextSkillReady {
			get {
				
				// Return early if there are no moves to learn.
				if (this.levelUpMoves.Count == 0) {
					return false;
				}
				
				// The persona level must be equal to or higher than the target level.
				int targetLevel = this.levelUpMoves.Peek().level;
				return this.Level >= targetLevel;
				
			}
		}
		/// <summary>
		/// All of the level moves that the persona could possibly learn
		/// </summary>
		public int AvailableLevelSkillCount {
			get {
				
				// Create a new list from the level up moves queue.
				var levelSkillsList = new List<LevelUpMove>(this.levelUpMoves);
				
				// Count the number of moves where the target level is less or equal to the Persona's
				int skillCount = levelSkillsList.Where(ls => ls.level <= this.Level).Count();

				// Return it.
				return skillCount;

			}
		}
		#endregion
		
		#region PROPERTIES - MISC
		/// <summary>
		/// When switching personas, I will need to know if it is in use or not so I can bar selection.
		/// </summary>
		public bool InUse {
			get {
				foreach (Player player in GameController.Instance.Variables.Players) {
					if (player.ActivePersona== this) {
						return true;
					}
				}
				return false;
			}
		}
		/// <summary>
		/// A version of InUse that takes into account a set of variables I want to read from.
		/// </summary>
		/// <param name="variables"></param>
		/// <returns></returns>
		public bool IsInUse(GameVariables variables) {
			// If any of the players make use of this persona, return true.
			return variables.Players.Select(p => p.ActivePersona).Contains(this);
		}
		/// <summary>
		/// The icon this Persona should use when inside a menu.
		/// </summary>
		public override Sprite Icon {
			get {
				return this.icon;
			}
		}
		#endregion

		#region CONSTRUCTORS
		public Persona(PersonaTemplate template, GameVariables gameVariables) : base(template, gameVariables) {

			this.bustUp = template.bustUp;
			this.icon = template.icon;
			this.badgeBoardHeadshot = template.badgeBoardHeadshotSprite;
			
			// Get the standard attack behavior. This is the behavior that is used on the "attack" command.
			this.standardAttackBehavior = template.standardAttackBehavior;
			// Copy over the level up dictionary.
			this.levelUpMoves = new Queue<LevelUpMove>(template.levelUpMoves);
		}
		public Persona(SerializablePersona sp, GameSaveLoaderController scd, GameVariables gameVariables) : base(sp, scd, gameVariables) {
			PersonaTemplate template = scd.GetPersonaTemplate(sp);
			this.bustUp = template.bustUp;
			this.icon = template.icon;
			this.standardAttackBehavior = template.standardAttackBehavior;
			this.levelUpMoves = new Queue<LevelUpMove>(template.levelUpMoves);
		}
		#endregion

		#region BEHAVIOR MANAGEMENT
		/// <summary>
		/// Dequeues the next level skill from the persona and adds it to its moveset.
		/// </summary>
		/// <returns>The behavior that was just learned.</returns>
		public BattleBehavior AddNextLevelSkill() {
			Debug.Assert(IsNextSkillReady);
			BattleBehavior nextSkill = this.levelUpMoves.Dequeue().behavior;
			this.AddBehavior(nextSkill);
			return nextSkill;
		}
		#endregion
		
		#region SERIALIZABLES
		[System.Serializable]
		public class SerializablePersona : SerializableCombatant {
			public SerializablePersona(Persona persona) : base(persona) {

			}
		}
		#endregion

	}

}