using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly;
using System.Linq;
using DG.Tweening;
using Grawly.Battle.WorldEnemies;
namespace Grawly.Battle {


	public class Enemy : Combatant {

		#region FIELDS - TEMPLATE
		/// <summary>
		/// The template that was used for this enemy.
		/// </summary>
		public EnemyTemplate Template { get; private set; }
		#endregion

		#region FIELDS - RESOURCES : MAX VALUES
		public override int MaxHP {
			get {
				return base.MaxHP;
				// return (int)(base.MaxHP * 0.5f);
				// return (this.Level + this.EN) * 6;
				// return this.GetAttributeValue(this.Level, 120f, this.hpCeil);
			}
		}
		public override int MaxMP {
			get {
				return base.MaxMP;
				// return (int)(base.MaxMP * 0.5f);
				// return (this.Level + this.MA) * 3;
				// return this.GetAttributeValue(this.Level, 40f, this.mpCeil);
			}
		}
		#endregion

		#region FIELDS - ETC
		/// <summary>
		/// Holds potential drops for this enemy.
		/// </summary>
		public Drops Drops { get; set; }
		/// <summary>
		/// A sprite to be used for the "world space" of the enemy.
		/// </summary>
		public Sprite EnemySprite { get; set; }
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The world enemy as it exists in the stage.
		/// Gets set upon creation.
		/// </summary>
		public WorldEnemyDX WorldEnemyDX { get; private set; }
		/// <summary>
		/// All of the available opponents an enemy can have.
		/// </summary>
		public override List<Combatant> Opponents {
			get {
				return BattleController.Instance.AlivePlayers.Cast<Combatant>().ToList();
			}
		}
		/// <summary>
		/// The allies for this enemy.
		/// </summary>
		public override List<Combatant> Allies {
			get {
				return BattleController.Instance.Enemies.Cast<Combatant>().ToList();
				// return BattleController.Instance.RemainingEnemies.Cast<Combatant>().ToList();
			}
		}
		#endregion

		#region FIELDS - SCENE REFERENCES : COMBATANT ANIMATOR
		/// <summary>
		/// The animator this Enemy should use. It's just their WorldEnemyDXBehavior.
		/// </summary>
		public override ICombatantAnimator CombatantAnimator {
			get {
				return this.WorldEnemyDX;
				// return this.WorldEnemyDX.CombatantAnimator;
			}
		}
		#endregion

		#region CONSTRUCTORS
		public Enemy(EnemyTemplate template, GameVariables gameVariables) : base(template, gameVariables) {
			Debug.LogWarning("This should not be called during normal play! It is only for prototyping purposes!");
			this.Template = template;
			this.EnemySprite = template.BodySprite;
			this.Drops = template.drops;
		}
		public Enemy(EnemyTemplate template, GameVariables gameVariables, WorldEnemyDX worldEnemyDX) : base(template, gameVariables) {
			this.Template = template;
			this.WorldEnemyDX = worldEnemyDX;
			this.EnemySprite = template.BodySprite;
			this.Drops = template.drops;
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - IMENUABLE
		/// <summary>
		/// The icon this enemy should use inside menus.
		/// </summary>
		public override Sprite Icon {
			get {
				return this.Template.iconSprite;
			}
		}
		#endregion

	}


}