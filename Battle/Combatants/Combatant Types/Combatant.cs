using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly;
using System.Linq;
using Grawly.UI.Legacy;
using Grawly.Story;
using Grawly.Battle.TurnBehaviors;
using Grawly.Battle.Functions;
using DG.Tweening;
using Grawly.UI;
using System.Reflection;
using Grawly.Battle.Modifiers;
using Grawly.Battle.Modifiers.Afflictions;
using Grawly.Battle.Equipment.Badges;
using Grawly.Toggles;
using Grawly.Toggles.Proto;
using Grawly.UI.MenuLists;

namespace Grawly.Battle {


	public abstract class Combatant : IMenuable {

		/// <summary>
		/// Important information about the combatant.
		/// </summary>
		public Metadata metaData;

		#region FIELDS - LEVELING
		/// <summary>
		/// The current level of this combatant.
		/// </summary>
		public int Level {
			get {
				float numerator = Mathf.Sqrt(totalExp);
				float denominator = Mathf.Sqrt(Mathf.Sqrt(totalExp)) * 0.5f;
				return Mathf.FloorToInt(numerator / denominator);
			}
		}
		/// <summary>
		/// The total EXP of this combatant.
		/// </summary>
		private int totalExp;
		/// <summary>
		/// The total EXP of this combatant.
		/// </summary>
		public int TotalEXP {
			get {
				return this.totalExp;
			}
			set {
				Debug.Log("Setting total exp of " + this.metaData.name + " to " + value);
				this.totalExp = value;
			}
		}
		#endregion

		#region FIELDS - RESOURCES : CEILINGS
		protected float hpCeil;
		protected float mpCeil;
		#endregion

		#region FIELDS - RESOURCES : RAW VALUES
		private int rawHP;
		private int rawMP;
		private int rawSP;
		#endregion

		#region FIELDS - RESOURCES : CLAMPED VALUES
		public int HP {
			get {
				return rawHP;
			}
			set {
				rawHP = Mathf.Clamp(value, 0, MaxHP);
			}
		}
		public int MP {
			get {
				return rawMP;
			}
			set {
				rawMP = Mathf.Clamp(value, 0, MaxMP);
			}
		}
		public int SP {
			get {
				return rawSP;
			}
			set {
				rawSP = Mathf.Clamp(value, 0, MaxSP);
			}
		}
		public int SPLevel {
			get {
				if (SPRatio == 1.0f) { return 3; } else if (SPRatio > .666f) { return 2; } else if (SPRatio > .333f) { return 1; } else { return 0; }
			}
		}
		#endregion

		#region FIELDS - RESOURCES : MAX VALUES
		public virtual int MaxHP {
			get {
				return (this.Level + this.BaseEN) * 6;
			}
		}
		public virtual int MaxMP {
			get {
				return (this.Level + this.BaseMA) * 3;
			}
		}
		public int MaxSP {
			get {
				// SP isn't really changing.
				return 1000;
			}
		}
		#endregion

		#region FIELDS - RESOURCES : RATIOS
		public float HPRatio {
			get {
				return (float)HP / (float)MaxHP;
			}
		}
		public float MPRatio {
			get {
				return (float)MP / (float)MaxMP;
			}
		}
		public float SPRatio {
			get {
				return (float)SP / (float)MaxSP;
			}
		}
		#endregion

		#region FIELDS - ATTRIBUTES : CEILINGS
		private float stCeil;
		private float maCeil;
		private float enCeil;
		private float agCeil;
		private float luCeil;
		#endregion

		#region PROPERTIES - STATIC ATTRIBUTES
		public int BaseST {
			get {
				return this.GetAttributeValue(this.Level, 1f, stCeil);
			}
		}
		public int BaseMA {
			get {
				return this.GetAttributeValue(this.Level, 1f, maCeil);
			}
		}
		public int BaseEN {
			get {
				return this.GetAttributeValue(this.Level, 1f, enCeil);
			}
		}
		public int BaseAG {
			get {
				return this.GetAttributeValue(this.Level, 1f, agCeil);
			}
		}
		public int BaseLU {
			get {
				return this.GetAttributeValue(this.Level, 1f, luCeil);
			}
		}
		#endregion
		
		#region PROPERTIES - DYNAMIC ATTRIBUTES
		public int DynamicST {
			get {
				return (int)Mathf.Pow(
						f: this.BaseST * GameController.Instance.DifficultyToggles.GetMultiplier(combatant: this, attributeType: AttributeType.ST),
						p: GameController.Instance.DifficultyToggles.GetExponent(combatant: this, attributeType: AttributeType.ST));
			}
		}
		public int DynamicMA {
			get {
				return (int)Mathf.Pow(
					f: this.BaseMA * GameController.Instance.DifficultyToggles.GetMultiplier(combatant: this, attributeType: AttributeType.MA),
					p: GameController.Instance.DifficultyToggles.GetExponent(combatant: this, attributeType: AttributeType.MA));
			}
		}
		public int DynamicEN {
			get {
				return (int)Mathf.Pow(
					f: this.BaseEN* GameController.Instance.DifficultyToggles.GetMultiplier(combatant: this, attributeType: AttributeType.EN),
					p: GameController.Instance.DifficultyToggles.GetExponent(combatant: this, attributeType: AttributeType.EN));
			}
		}
		public int DynamicAG {
			get {
				return (int)Mathf.Pow(
					f: this.BaseAG * GameController.Instance.DifficultyToggles.GetMultiplier(combatant: this, attributeType: AttributeType.AG),
					p: GameController.Instance.DifficultyToggles.GetExponent(combatant: this, attributeType: AttributeType.AG));
			}
		}
		public int DynamicLU {
			get {
				return (int)Mathf.Pow(
					f: this.BaseLU * GameController.Instance.DifficultyToggles.GetMultiplier(combatant: this, attributeType: AttributeType.LU),
					p: GameController.Instance.DifficultyToggles.GetExponent(combatant: this, attributeType: AttributeType.LU));
			}
		}
		#endregion

		#region FIELDS - METADATA
		/// <summary>
		/// Is this combatant dead?
		/// </summary>
		public bool IsDead {
			get {
				if (this.HP <= 0) {
					return true;
				} else {
					return false;
				}
			}
		}
		/// <summary>
		/// Keeps track of whether or not this combatant has fallen.
		/// </summary>
		public bool IsDown { get; protected set; } = false;
		/*/// <summary>
		/// Determines whether or not this combatant is able to at the start of their turn.
		/// </summary>
		public virtual bool CanMoveOnReady {
			get {
				// This is more or less determined by a number of factors.
				// Though, for now it's just the affliction.
				if (Affliction.CanMoveOnReady == true) {
					return true;
				} else {
					return false;
				}

			}
		}*/
		#endregion

		#region FIELDS - MOOD AND INTENT
		/// <summary>
		/// The mood for this combatant.
		/// </summary>
		protected CombatantMood combatantMood = new CombatantMood();
		/// <summary>
		/// The mood for this combatant.
		/// </summary>
		public CombatantMood CombatantMood {
			get {
				return this.combatantMood;
			}
		}
		#endregion

		#region FIELDS - MOVES AND RESISTANCES
		/// <summary>
		/// The behaviors of the combatant. Probably does not include items, as I want those to be shared.
		/// </summary>
		protected Dictionary<BehaviorType, List<BattleBehavior>> behaviors;
		/// <summary>
		/// The behaviors of the combatant. Probably does not include items, as I want those to be shared.
		/// </summary>
		public virtual Dictionary<BehaviorType, List<BattleBehavior>> AllBehaviors {
			get {
				return behaviors;
			}
			set {
				behaviors = value;
			}
		}
		/// <summary>
		/// The battle behaviors that this combatant is allowed to use in battle.
		/// Does not contain behaviors that do not show up in the menu to begin with.
		/// </summary>
		public List<BattleBehavior> UsableSpecialBehaviors {
			get {
				if (this.Affliction is IBehaviorRestrictor) {
					return (this.Affliction as IBehaviorRestrictor)
						.RestrictUsableBehaviors(
						specialBehaviors: this.AllBehaviors[BehaviorType.Special]
							.Where(b => b.showInBattleMenu)
							.Where(b => this.HasResourcesForBehavior(behavior: b) == true)
							.Where(b => this.TurnBehavior.GetTargets(combatant: this, behavior: b).Count > 0)
							.ToList());
				} else {
					return this.AllBehaviors[BehaviorType.Special]
						.Where(b => b.showInBattleMenu)
						.Where(b => this.HasResourcesForBehavior(behavior: b) == true)
						.Where(b => this.TurnBehavior.GetTargets(combatant: this, behavior: b).Count > 0)
						.ToList();
				}
			}
		}
		/// <summary>
		/// Keeps track of resistances to certain elements and whatnot.
		/// There's the potential for these to be overridden, hence the necessity
		/// of a computed property rather than a field (fields cannot be overridden)
		/// </summary>
		protected Dictionary<ElementType, ResistanceType> resistances;
		/// <summary>
		/// Keeps track of resistances to certain elements and whatnot.
		/// There's the potential for these to be overridden, hence the necessity
		/// of a computed property rather than a field (fields cannot be overridden)
		/// </summary>
		public virtual Dictionary<ElementType, ResistanceType> Resistances {
			get {
				return resistances;
			}
			set {
				resistances = value;
			}
		}
		#endregion

		#region FIELDS - MODIFIERS
		/// <summary>
		/// A simple getter method for the affliction.
		/// </summary>
		/// <returns></returns>
		public ICombatantAffliction Affliction { get; private set; }
		/// <summary>
		/// The moves that exist within the combatant's moveset that exist to just modify the battle.
		/// </summary>
		public virtual List<BattleBehaviorModifier> BattleBehaviorModifiers {
			get {
				try {
					return this.AllBehaviors[BehaviorType.Special]										// Go into the Special behaviors only.
						.SelectMany(b => b.PassiveFunctions												// Select the BattleBehaviorModifiers...
						.Select(m => m.AssignBattleBehaviorAndCombatant(behavior: b, combatant: this)))	// ...but also transform them by passing them references to the behavior/combatant who own them.
						.ToList();
				} catch (System.Exception e) {
					Debug.LogError("Could not get BattleBehaviorModifiers for " + this.metaData.name + "! Reason:\n" + e.Message + "\nReturning blank list.");
					return new List<BattleBehaviorModifier>();
				}

			}
		}
		/// <summary>
		/// Contains values and functions that help regulate modifiers to attack/defense/accuracy.
		/// Only really here for compatability.
		/// </summary>
		public StatusModifiers statusModifiers = new StatusModifiers();
		/// <summary>
		/// A list of modifiers that respond to ceratin events/stimuli.
		/// Gets cleared at the end of each battle (i.e., these are temporary)
		/// </summary>
		private List<CombatantModifier> combatantModifiers = new List<CombatantModifier>();
		#endregion

		#region FIELDS - BEHAVIOR
		/// <summary>
		/// The turn behavior that allows this combatant to decide upon a move.
		/// </summary>
		private CombatantTurnBehavior turnBehavior;
		/// <summary>
		/// The turn behavior that allows this combatant to decide upon a move.
		/// </summary>
		public CombatantTurnBehavior TurnBehavior {
			get {
				return this.turnBehavior;
			}
			set {
				// This will fail if its anything other than a normal turn behavior.
				this.turnBehavior = CombatantTurnBehavior.Clone(turnBehavior: value as CombatantTurnBehavior, combatant: this);
			}
		}
		#endregion

		#region FIELDS - ANIMATOR
		/// <summary>
		/// The animator that should be used for this combatant when processing DamageCalculations.
		/// Players use their PlayerStatusDX; Enemies use their WorldEnemyDX.
		/// </summary>
		public abstract ICombatantAnimator CombatantAnimator { get; }
		#endregion

		#region FIELDS - BATTLE VARIABLES
		/// <summary>
		/// The opponents of this particular combatant.
		/// </summary>
		public virtual List<Combatant> Opponents {
			get {
				throw new System.Exception("The base class should never be called. Combatants are neither player nor enemy.");
			}
		}
		/// <summary>
		/// The allies of this particular combatant.
		/// </summary>
		public virtual List<Combatant> Allies {
			get {
				throw new System.Exception("Have you considered like. Making this an abstract class because uh.");
			}
		}
		#endregion

		#region CONSTRUCTORS
		/// <summary>
		/// Sets up a combatant from a template.
		/// </summary>
		/// <param name="template"></param>
		public Combatant(CombatantTemplate template, GameVariables gameVariables) {
			// Grab the metadata
			this.metaData = template.metaData;

			// Make the null affliction.
			this.Affliction = new NullAffliction().AssignCombatant(combatant: this) as ICombatantAffliction;

			// Get the total exp. This is required for doing the upcoming calculations for HP/MP and other attributes.
			this.totalExp = template.totalExp;
			if (template.preferedLevel > 0) {
				this.totalExp = ExpForLevel(template.preferedLevel);
			}
			// Also remember to save the ceilings. These are also required for computations.
			this.hpCeil = template.HPCeiling;
			this.mpCeil = template.MPCeiling;
			this.stCeil = template.STCeiling;
			this.maCeil = template.MACeiling;
			this.enCeil = template.ENCeiling;
			this.agCeil = template.AGCeiling;
			this.luCeil = template.LUCeiling;

			// Set the HP/MP/SP. The Max values of each are already computed as a function of level/floor/ceilings.
			if (this is Player == false) {
				this.HP = MaxHP;
				this.MP = MaxMP;
				this.SP = MaxSP;    // Except this one.
			} else {
				Debug.Log("Detected Player combatant. Deferring HP/MP setting until base class is finished.");
			}

			// Make the dictionaries.
			this.resistances = new Dictionary<ElementType, ResistanceType>();
			this.behaviors = new Dictionary<BehaviorType, List<BattleBehavior>>();

			// Clone and set the turn behavior.
			this.TurnBehavior = template.TurnBehavior;

			// Reset the status modifiers.
			this.statusModifiers.Reset();

			// Don't forget to set up a list for each type in the behaviors dictionary.
			foreach (BehaviorType type in System.Enum.GetValues(typeof(BehaviorType))) {
				behaviors[type] = new List<BattleBehavior>();
			}

			// Go through each element type and add the default resistance.
			foreach (ElementType element in System.Enum.GetValues(typeof(ElementType))) {
				resistances[element] = ResistanceType.Nm;
			}

			// Adjust the resistances based on what was in the template's list.
			foreach (ResistanceTuple resistanceTuple in template.resistances) {
				resistances[resistanceTuple.element] = resistanceTuple.resistance;
			}

			// Now grab the behaviors from the template.
			foreach (BattleBehavior behavior in template.battleBehaviors) {
				if (behavior.behaviorType == BehaviorType.Item) {
					Debug.LogWarning("Trying to add an item to a player. Will add to all items list instead.");
					GameController.Instance.Variables.Items[behavior] += 1;
				} else {
					behaviors[behavior.behaviorType].Add(behavior);
				}
			}

			// Also clone the default combatant modifiers and add them to this combatant.
			template.defaultCombatantModifiers.Select(m => m.Clone()).ToList().ForEach(m => this.AddModifier(modifier: m));

		}
		/// <summary>
		/// Sets up a combatant from a SerializableCombatant. (I.e., from a saved game.
		/// </summary>
		/// <param name="sc">The serialized combatant.</param>
		/// <param name="scd">The object that contains the assets that need to be pulled from.</param>
		/// <param name="gameVariables">The game variables that are a part of this save file. Needed for certain edge cases like when a player has a persona that also needs to be added.</param>
		public Combatant(SerializableCombatant sc, GameSaveLoaderController scd, GameVariables gameVariables) {

			this.metaData = sc.metaData;
			this.totalExp = sc.totalExp;
			this.hpCeil = sc.hpCeil;
			this.mpCeil = sc.mpCeil;
			this.stCeil = sc.stCeil;
			this.maCeil = sc.maCeil;
			this.enCeil = sc.enCeil;
			this.agCeil = sc.agCeil;
			this.luCeil = sc.luCeil;
			this.rawHP = sc.rawHP;
			this.rawMP = sc.rawMP;
			this.rawSP = sc.rawSP;
			this.resistances = new Dictionary<ElementType, ResistanceType>();
			this.behaviors = new Dictionary<BehaviorType, List<BattleBehavior>>();

			foreach (ResistanceTuple rt in sc.resistances) {
				resistances.Add(rt.element, rt.resistance);
			}

			// Make a new list for each behavior type.
			foreach (BehaviorType type in System.Enum.GetValues(typeof(BehaviorType))) {
				behaviors[type] = new List<BattleBehavior>();
			}
			// Go through each string in the behaviors and find the correct behavior that corresponds to the string.
			foreach (string str in sc.behaviors) {
				BattleBehavior behavior = scd.GetBattleBehavior(str);
				// AddBehavior(behavior);
				behaviors[behavior.behaviorType].Add(behavior);
			}

			// Reset the status modifiers.
			this.statusModifiers.Reset();

			// Make the null affliction.
			this.Affliction = new NullAffliction().AssignCombatant(combatant: this) as ICombatantAffliction;
		}
		#endregion

		#region BATTLE BEHAVIOR MANAGEMENT
		/// <summary>
		/// Adds a behavior to the combatant's behavior dictionary.
		/// </summary>
		/// <param name="behavior">The new behavior</param>
		public void AddBehavior(BattleBehavior behavior) {
			if (AllBehaviors[behavior.behaviorType].Count > 8) {
				Debug.LogWarning("Behavior count is over 8! Be careful when adding " + behavior.behaviorName + " to " + metaData.name);
			} 
			AllBehaviors[behavior.behaviorType].Add(behavior);
			
		}
		/// <summary>
		/// Adds a battle behavior to the combatant's behavior dictionary. Overrides behavior in given slot.
		/// </summary>
		/// <param name="behavior">The new behavior</param>
		/// <param name="slot">The slot to overwrite</param>
		public void AddBehavior(BattleBehavior behavior, int slot) {
			// If the slot number is on the edge of the list, just use the regular add function.
			if (slot == AllBehaviors[behavior.behaviorType].Count) {
				AddBehavior(behavior);
			} else if (slot < AllBehaviors[behavior.behaviorType].Count) {
				AllBehaviors[behavior.behaviorType][slot] = behavior;
			} else {
				Debug.LogError("Slot not valid!");
			}

		}
		/// <summary>
		/// Adds a battle behavior to the combatant's behavior dictionary. New behavior overwrites one to replace.
		/// </summary>
		/// <param name="behavior">The new behavior</param>
		/// <param name="toReplace">The behavior to overwrite</param>
		public void AddBehavior(BattleBehavior behavior, BattleBehavior toReplace) {
			// Go through each behavior in the list and find the one that matches the target
			for (int i = 0; i < AllBehaviors[behavior.behaviorType].Count; i++) {
				if (AllBehaviors[behavior.behaviorType][i] == toReplace) {
					// If the entry in the list matches the one meant to be replaced, add it.
					AddBehavior(behavior, i);
					return;
				}
			}
			Debug.LogError("Could not find target behavior to replace!");
		}
		/// <summary>
		/// Can this behavior be added to the combatant without error?
		/// </summary>
		/// <param name="behavior"></param>
		/// <returns></returns>
		public bool CanAddBehavior(BattleBehavior behavior) {

			// Get the skill slot cap from the toggle controller.
			int skillSlotCap = ToggleController.GetToggle<CombatantSkillSlotCap>().GetToggleInt();
			
			// If the skill count equal or greater to the cap, it can't be learned.
			if (AllBehaviors[behavior.behaviorType].Count >= skillSlotCap) {
				return false;
			} else {
				return true;
			}
			
		}
		#endregion

		#region MODIFIERS
		/// <summary>
		/// Finds and returns the modifiers that implement the specified type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public virtual List<T> GetModifiers<T>() {
			
			// Create a new list.
			List<T> modifierList = new List<T>();
			
			// Go through the modifiers in the behaviors and find ones that match up.
			modifierList.AddRange(this.BattleBehaviorModifiers.Where(m => m is T).Cast<T>().ToList());
			
			// Do the same for combatant modifiers.
			modifierList.AddRange(this.combatantModifiers.Where(m => m is T).Cast<T>().ToList());
			
			// Check if the affliction is of that same type as well.
			if (this.Affliction is T) {
				modifierList.Add((T)this.Affliction);
			}

			// Return the list.
			return modifierList;
			
		}
		/// <summary>
		/// Adds a modifier to this combatant. A modifier can be thought of as an object that holds memory and responds to certain stimuli.
		/// </summary>
		/// <param name="modifier"></param>
		public void AddModifier(CombatantModifier modifier) {
			Debug.Log("ADDING MODIFIER OF TYPE " + modifier.GetType().ToString() + " TO " + this.metaData.name);
			// Remember when assigning the modifier to also pass it a reference to this combatant.
			this.combatantModifiers.Add(modifier.AssignCombatant(combatant: this));
		}
		/// <summary>
		/// Removes a modifier from the combatant. Returns whether or not it was successfully removed.
		/// </summary>
		/// <param name="modifier"></param>
		/// <returns></returns>
		public bool RemoveModifier(CombatantModifier modifier) {
			bool result = this.combatantModifiers.Remove(modifier);
			string success = result == true ? "SUCCESS: " : "FAILURE: ";
			Debug.Log(success + "REMOVING MODIFIER OF TYPE " + modifier.GetType().ToString() + " FROM " + this.metaData.name);
			return result;
		}
		/// <summary>
		/// Removes a modifier from the combatant based on the actual type.
		/// All instances of the given type will be removed.
		/// </summary>
		/// <param name="modifierTypeToRemove">The type of modifier to remove.</param>
		/// <returns>The number of modifiers that were removed.</returns>
		public int RemoveModifier(System.Type modifierTypeToRemove) {
			int removeCount = 0;
			this.GetModifiers<CombatantModifier>()
				.Where(m => m.GetType() == modifierTypeToRemove)
				.ToList()
				.ForEach(m => {
					this.RemoveModifier(modifier: m);
					removeCount += 1;
				});
			return removeCount;
		}
		/// <summary>
		/// Clears out all modifiers from the combatant. Usually called at the end of the battle.
		/// </summary>
		public void ClearModifiers() {
			Debug.Log("CLEARING MODIFIERS FROM " + this.metaData.name);
			this.combatantModifiers.Clear();
		}
		#endregion

		#region BATTLE EVALUATION - BATTLE BEHAVIOR
		/// <summary>
		/// Can this combatant use this behavior?
		/// </summary>
		/// <param name="behavior"></param>
		/// <returns></returns>
		public bool HasResourcesForBehavior(BattleBehavior behavior) {
			switch (behavior.costType) {
				case BehaviorCostType.HP:
					return (this.HP - this.GetActualBattleBehaviorCost(behavior: behavior)) >= 0;
				case BehaviorCostType.MP:
					return (this.MP - this.GetActualBattleBehaviorCost(behavior: behavior)) >= 0;
				case BehaviorCostType.ST:
					throw new NotImplementedException("No moves use the old Stamina.");
					// return (this.ST - this.GetActualBattleBehaviorCost(behavior: behavior)) >= 0;
				default:
					return true;
			}
		}
		/// <summary>
		/// Gets the "actual" cost of a battle behavior for this combatant.
		/// This is relevant if there is a modifier that changes how much a behavior should cost.
		/// </summary>
		/// <param name="behavior"></param>
		/// <returns></returns>
		public int GetActualBattleBehaviorCost(BattleBehavior behavior) {
			// Grab the base cost.
			int runningCost = behavior.baseCost;
			// Go through each modifier that intercepts the cost and adjust it.
			foreach (IInterceptBattleBehaviorCost costInterceptor in this.GetModifiers<IInterceptBattleBehaviorCost>()) {
				runningCost = costInterceptor.InterceptBattleBehaviorCost(runningCost: runningCost, self: this, behavior: behavior);
			}
			// Return the adjusted cost (though 99.9% of the time this will be the cost as it normally is.)
			return runningCost;
		}
		/// <summary>
		/// For moves that take a percentage of a resource (Phys) rather than a static number (MP), get what it is
		/// </summary>
		/// <param name="behavior"></param>
		/// <returns></returns>
		public int GetPercentageCost(BattleBehavior behavior) {
			switch (behavior.costType) {
				case BehaviorCostType.HP:
					return (int)((float)(this.MaxHP) * ((float)this.GetActualBattleBehaviorCost(behavior: behavior) / 100f));
					// return (int)((float)(this.MaxHP) * ((float)behavior.baseCost / 100f));
				case BehaviorCostType.MP:
					return (int)((float)(this.MaxMP) * ((float)this.GetActualBattleBehaviorCost(behavior: behavior) / 100f));
					// return (int)((float)(this.MaxMP) * ((float)behavior.baseCost / 100f));
				case BehaviorCostType.ST:
					return (int)((float)(this.MaxSP) * ((float)this.GetActualBattleBehaviorCost(behavior: behavior) / 100f));
					// return (int)((float)(this.MaxSP) * ((float)behavior.baseCost / 100f));
				default:
					Debug.LogError("Couldn't get percentage!");
					return 0;
			}

		}
		#endregion

		#region BATTLE EVALUATION - MISC
		/// <summary>
		/// Down this combatant.
		/// </summary>
		public void SetDownedStatus(bool isDown) {
			this.IsDown = isDown;
			// Also tell the animator to update the status. I'm not entirely happy with calling it here because it couples the logic/animation too tightly but if I need to change it I will.
			this.CombatantAnimator.AnimateDownedStatus(combatant: this, isDown: isDown);
		}
		/// <summary>
		/// Deducts the cost of a behavior from this combatant's resources.
		/// </summary>
		/// <param name="behavior"></param>
		public void DeductBehaviorCost(BattleBehavior behavior) {
			// Debug.LogError(behavior.behaviorName);
			switch (behavior.costType) {
				case BehaviorCostType.HP:
					this.HP -= this.GetPercentageCost(behavior);
					break;
				case BehaviorCostType.MP:
					this.MP -= this.GetActualBattleBehaviorCost(behavior: behavior);
					// this.MP -= behavior.baseCost;
					break;
				case BehaviorCostType.ST:
					// Stamina deduction was set up a bit... bad.
					this.SP -= (behavior.baseCost * 332);
					break;
				case BehaviorCostType.None:
					// Cost type is set to none, so. Don't do anything.
					break;
				default:
					Debug.LogError("Couldn't determine cost type!");
					break;
			}
		}
		/// <summary>
		/// Evaluates the impact of a damage tuple. Basically just deducts HP/MP by the damageTuple.amt value.
		/// </summary>
		/// <param name="damageCalculation"></param>
		protected void EvaluateDamageCalculationImpact(DamageCalculation damageCalculation) {

			Debug.Log(this.metaData.name + " has taken " + damageCalculation.rawDamageAmount + " of " + damageCalculation.behavior.targetResource.ToString() + " damage.");
			// Call the simple evaluation. I have this function that takes a DC just in case.
			this.SimpleEvaluateDamage(
				damageAmount: damageCalculation.rawDamageAmount,
				resourceType: damageCalculation.behavior.targetResource);

			/*switch (damageCalculation.behavior.targetResource) {
				case BehaviorCostType.HP:
					this.HP -= damageCalculation.rawDamageAmount;
					break;
				case BehaviorCostType.MP:
					this.MP -= damageCalculation.rawDamageAmount;
					break;
				case BehaviorCostType.ST:
					this.SP -= damageCalculation.rawDamageAmount;
					break;
				case BehaviorCostType.None:
					break;
				default:
					Debug.LogError("Couldn't deduct impact from target! Target: " + damageCalculation.target.metaData.name + ", Behavior: " + damageCalculation.behavior.behaviorName);
					break;
			}*/
		}
		/// <summary>
		/// Takes steps to process a damage tuple in its entirety.
		/// </summary>
		/// <param name="damageCalculation"></param>
		public void EvaluateDamageCalculation(DamageCalculation damageCalculation) {

			// Check if the move reflected off the opponent.
			if (damageCalculation.finalResistance == ResistanceType.Ref) {
				throw new System.Exception("I never want to handle reflections from here.");
			}

			// Only proceed if there was no miss.
			if (damageCalculation.accuracyType != AccuracyType.Miss) {
				// Evaluate the impact of the damage tuple.
				this.EvaluateDamageCalculationImpact(damageCalculation);

				// Go through the calculated power boosts and add them.
				damageCalculation.powerBoosts.ForEach(kvp => {
					this.statusModifiers.SetPowerBoost(kvp: kvp);
				});

				// Cure an affliction
				if (damageCalculation.cureAffliction == true) {
					damageCalculation.target.CureAffliction(animateCure: false);
				} else if (damageCalculation.setAffliction == true) {
					damageCalculation.target.SetAffliction(afflictionType: damageCalculation.afflictionType, animateAffliction: false);
				}
			} else {
				// I do wanna... log this just in case.
				Debug.Log("BATTLE: " + damageCalculation.behavior.behaviorName + " from " + damageCalculation.source.metaData.name + " missed " + damageCalculation.FinalTarget.metaData.name);
			}


		}
		/// <summary>
		/// Evaluates a raw amount of damage to the specified resource.
		/// </summary>
		/// <param name="damageAmount">The damage to take away.</param>
		/// <param name="resourceType">The resource type to target.</param>
		public void SimpleEvaluateDamage(int damageAmount, BehaviorCostType resourceType) {

			// This is different 

			switch (resourceType) {
				case BehaviorCostType.HP:
					this.HP -= damageAmount;
					break;
				case BehaviorCostType.MP:
					this.MP -= damageAmount;
					break;
				case BehaviorCostType.ST:
					this.SP -= damageAmount;
					break;
				case BehaviorCostType.None:
					break;
				default:
					Debug.LogError("Couldn't deduct impact from target!");
					break;
			}
		}
		#endregion

		#region STATUS CHECK
		/// <summary>
		/// Returns true if the damage specified will make the combatant die.
		/// </summary>
		/// <param name="damageAmount">The amount of damage to potentially give to the combatant.</param>
		/// <returns>Whether this amount will kill them or not.</returns>
		public bool WillDieFromDamage(int damageAmount) {
			return (this.HP - damageAmount) <= 0;
		}
		#endregion

		#region PAUSE MENU EVALUATION
		/// <summary>
		/// Is this combatant (usually a player) affected by the specified behavior?
		/// </summary>
		/// <param name="behavior">The behavior to check for.</param>
		/// <returns></returns>
		public bool IsAffectedByBehavior(BattleBehavior behavior) {
			// If this combatant is dead...
			if (this.IsDead == true) {
				// ... Return true if the behavior's target types are ones that target dead allies.
				return (behavior.targetType == TargetType.AllDeadAllies || behavior.targetType == TargetType.OneDeadAlly);
			} else {
				// If the combatant is not dead, return the inverse of that.
				return !(behavior.targetType == TargetType.AllDeadAllies || behavior.targetType == TargetType.OneDeadAlly);
			}
		}
		#endregion

		#region AFFLICTIONS
		/// <summary>
		/// Sets the affliction by generating a new affliction of the specified type.
		/// </summary>
		/// <param name="afflictionType">The type of affliction to set.</param>
		/// <param name="animateAffliction">Should the affliction also be animated at the same time?</param>
		/// <returns>Whether or not the affliction was successful in setting.</returns>
		public AfflictionSetResultsType SetAffliction(AfflictionType afflictionType, bool animateAffliction) {
			// Make the set fail if the combatant already has an affliction and the incoming affliction is not the None type.
			if (this.Affliction.Type != AfflictionType.None && afflictionType != AfflictionType.None) {
				Debug.Log(this.metaData.name + " already has affliction of type " + this.Affliction.Type.ToString() + "! The new affliction of type " + afflictionType.ToString() + " will not be set.");
				return AfflictionSetResultsType.Failure;

			} else {

				// If I specified to animate the affliction at the same time, do that.
				// THIS MUST BE DONE BEFORE THE SET TAKES PLACE TO ENSURE THAT THE ANIMATION WORKS AS INTENDED.
				if (animateAffliction == true) {
					this.CombatantAnimator.AnimateAfflictionSet(combatant: this, afflictionType: afflictionType);
				}

				// If the player does not already have an affliction, go ahead and set it.
				this.SetAffliction(affliction: CombatantModifier.CombatantAfflictionFromType(type: afflictionType, combatant: this));

				// Return that the operation was a success.
				return AfflictionSetResultsType.Success;
			}

		}
		/// <summary>
		/// Sets an affliction.
		/// </summary>
		/// <param name="affliction"></param>
		private void SetAffliction(ICombatantAffliction affliction) {
			Debug.Log("Setting affliction of type " + affliction + " on " + this.metaData.name);
			this.Affliction = affliction;

		}
		/// <summary>
		/// Cure the combatant's affliction.
		/// </summary>
		public void CureAffliction(bool animateCure) {
			this.SetAffliction(afflictionType: AfflictionType.None, animateAffliction: animateCure);
		}
		#endregion

		#region RESISTANCES
		/// <summary>
		/// Checks if a behavior exploits the combatants weakness and returns the kind.
		/// </summary>
		public ResistanceType CheckResistance(BattleBehavior behavior) {
			return CheckResistance(behavior.elementType);
		}
		/// <summary>
		/// Checks if a behavior exploits the combatants weakness and returns the kind.
		/// </summary>
		public ResistanceType CheckResistance(ElementType elementType) {

			// Double check that this enemy does not have any breaks on them. If they do, return Normal.
			if (this.CheckResistanceBreak(elementType: elementType) == true) {
				return ResistanceType.Nm;
			}

			// Otherwise, check their resistances normally.
			// Do this by seeing if the resistances dictionary contains this element type.
			if (Resistances.ContainsKey(elementType) == true) {
				// Debug.Log(elementType.ToString() + " has resistance of type " + Resistances[elementType]);
				return Resistances[elementType];
			} else {
				// If the element type doesn't exist in the resistances dictionary, assume that it's normal.
				return ResistanceType.Nm;
			}
		}
		/// <summary>
		/// This one is a little bit weird so lemme explain.
		/// NullifyIncomingAttack and ResistanceChange can conflict with each other; 
		/// ResistanceChange needs to take priority if it's applying a resistance break.
		/// NullifyIncomingAttack will call this CheckResistanceBreak method while it's doing its thing
		/// and see if it is allowed to nullify a DamageCalculation.
		/// </summary>
		/// <param name="elementType"></param>
		/// <returns></returns>
		public bool CheckResistanceBreak(ElementType elementType) {
			return this.combatantModifiers                              // Go through the modifiers
				.Where(m => m is ResistanceChange)                      // Find ones that are resistance changes
				.Cast<ResistanceChange>()                               // Cast the list so i can work with it
				.Any(m => m.BreaksElement(elementType));                // If any of them breaks the specified element, return true.
		}
		#endregion

		#region ATTRIBUTES
		/// <summary>
		/// Gets the baseline value of the specified attribute, before any modifications are applied.
		/// </summary>
		/// <param name="attributeType">The type of attribute to get.</param>
		/// <returns>The value of the specified attribute before modifiers are applied.</returns>
		public int GetBaseAttribute(AttributeType attributeType) {
			switch (attributeType) {
				case AttributeType.ST:
					return BaseST;
				case AttributeType.MA:
					return BaseMA;
				case AttributeType.EN:
					return BaseEN;
				case AttributeType.AG:
					return BaseAG;
				case AttributeType.LU:
					return BaseLU;
				default:
					throw new System.Exception("Could not determine attribute to return!");
			}
		}
		/// <summary>
		/// Gets the dynamic value of the specified attribute, after modifications are applied.
		/// </summary>
		/// <param name="attributeType">The type of attribute to get.</param>
		/// <returns>The value of the specified attribute after modifiers are applied.</returns>
		public int GetDynamicAttribute(AttributeType attributeType) {
			switch (attributeType) {
				case AttributeType.ST:
					return DynamicST;
				case AttributeType.MA:
					return DynamicMA;
				case AttributeType.EN:
					return DynamicEN;
				case AttributeType.AG:
					return DynamicAG;
				case AttributeType.LU:
					return DynamicLU;
				default:
					throw new System.Exception("Could not determine attribute to return!");
			}
		}
		#endregion
		
		#region BOOSTS
		/// <summary>
		/// Grabs the boost of the specified type.
		/// </summary>
		/// <param name="boostType"></param>
		/// <returns></returns>
		public float GetPowerBoost(PowerBoostType boostType) {
			// For now, just return the boost inside the status modifiers.
			return this.statusModifiers.GetPowerBoost(boostType: boostType);
		}
		#endregion

		#region LEVELING UP
		/// <summary>
		/// Parses the information from a battle results data and does things like add EXP
		/// </summary>
		/// <param name="battleResults"></param>
		public virtual void ParseBattleResults(LegacyBattleResultsData battleResults) {
			// Copy over the old level/HP/MP values, because I need to check against this information later.
			int oldLevel = Level;
			int oldMaxHP = MaxHP;
			int oldMaxMP = MaxMP;
			// Use the GameController's expMultiplier to multiply the amount of exp.
			// this.totalExp += (int)(battleResults.exp * LegacyStoryController.Instance.expMultiplier);
			// Debug.LogWarning("Removed the ability to use the exp multiplier. Fix this if you need to.");
			this.totalExp += battleResults.exp;
			if (this.Level > oldLevel) {
				Debug.Log(metaData.name + " LEVELED UP. OLD LEVEL: " + oldLevel + ", NEW LEVEL: " + this.Level);
			}

			// Restore the difference btween these two values.
			this.HP += (this.MaxHP - oldMaxHP);
			this.MP += (this.MaxMP - oldMaxMP);
		}
		/// <summary>
		/// Parses the drops from the battle. Mostly used for the DungeonDropParser.
		/// </summary>
		/// <param name="battleTemplate">The BattleTemplate that was just finished.</param>
		/// <param name="gameVariables">The game variables involved.</param>
		public virtual void ParseBattleResults(BattleTemplate battleTemplate, GameVariables gameVariables) {
			// Copy over the old level/HP/MP values, because I need to check against this information later.
			int oldLevel = Level;
			int oldMaxHP = MaxHP;
			int oldMaxMP = MaxMP;

			this.totalExp += battleTemplate.GetExperience(gameVariables: gameVariables);

			if (this.Level > oldLevel) {
				Debug.Log(metaData.name + " LEVELED UP. OLD LEVEL: " + oldLevel + ", NEW LEVEL: " + this.Level);
			}

			// Restore the difference btween these two values.
			this.HP += (this.MaxHP - oldMaxHP);
			this.MP += (this.MaxMP - oldMaxMP);
		}
		/// <summary>
		/// Parses an amount of EXP and allows the player to level up.
		/// </summary>
		/// <param name="experience"></param>
		public CombatantLevelUpResults ParseExperience(int experience) {
			// Remember the old level.
			int oldLevel = this.Level;
			// Add the experience to the total.
			this.TotalEXP += experience;
			// Return a new level up results that shows how the experience was parsed.
			return new CombatantLevelUpResults(combatant: this, oldLevel: oldLevel, newLevel: this.Level);
		}
		/// <summary>
		/// Get the amount of experience points required get to the next level.
		/// </summary>
		/// <returns>The amount of EXP required to reach the next level.</returns>
		public int ExpForNextLevel() {
			return this.ExpForLevel(this.Level + 1) - this.totalExp;
		}
		/// <summary>
		/// Get the amount of experience points required for a given level.
		/// </summary>
		/// <param name="level"></param>
		/// <returns></returns>
		public int ExpForLevel(int level) {
			// return Mathf.CeilToInt((Mathf.Pow(level, 4)) * 16);
			return Mathf.CeilToInt((Mathf.Pow(level, 4)) / 16);
		}
		/// <summary>
		/// Get a value for HP/ST/whatever.
		/// </summary>
		/// <param name="level">The level of the combatant.</param>
		/// <param name="floor">The lowest possible value to lerp between.</param>
		/// <param name="ceiling">The highest possible value to lerp between.</param>
		/// <returns></returns>
		protected int GetAttributeValue(int level, float floor, float ceiling) {
			if (level > 99) { Debug.LogError("LEVEL SHOULD NOT BE HIGHER THAN 99"); }
			// float t = Mathf.InverseLerp(1f, 99f, level);
			// Debug.Log("Level: " + level + ", t value: " + t);
			// Try using the fraction of the level over 100.
			float stat = Mathf.Lerp(floor, ceiling, level / 100f);
			// Debug.Log("Level/100: " + (level/100f).ToString() + ", Floor: " + floor + ", Ceiling: " + ceiling + ", Stat: " + stat);
			return Mathf.CeilToInt(stat);
		}
		#endregion

		#region MISC COMBATANT UTILITIES
		/// <summary>
		/// Checks if the given combatants are of the same type. E.x., is a player the same as an enemy? (No)
		/// </summary>
		/// <param name="left">The first combatant. There's no reason for this to be called left.</param>
		/// <param name="right">The second combatant. There's no reason for this to be called right.</param>
		/// <returns></returns>
		public static bool CombatantTypeMatch(Combatant left, Combatant right) {
			if (left.GetType() == right.GetType()) {
				return true;
			} else {
				return false;
			}
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - IMENAUBLE
		public string PrimaryString {
			get {
				return this.metaData.name;
			}
		}
		public string QuantityString {
			get {
				return "";
			}
		}
		public string DescriptionString {
			get {
				return "";
			}
		}
		public virtual Sprite Icon {
			get {
				return null;
			}
		}
		#endregion

		#region SERIALIZABLES
		/// <summary>
		/// The combatant in a form that can be saved and loaded into a save file.
		/// </summary>
		[System.Serializable]
		public class SerializableCombatant {
			#region FIELDS
			public Metadata metaData;
			public int totalExp;
			public List<string> behaviors = new List<string>();
			public List<ResistanceTuple> resistances = new List<ResistanceTuple>();
			public float hpCeil;
			public float mpCeil;
			public float stCeil;
			public float maCeil;
			public float enCeil;
			public float agCeil;
			public float luCeil;
			// Remember that these aren't definitions so much as the player's current vitals.
			// I.e., raw values are cured by cheesecakes and shit.
			public int rawHP;
			public int rawMP;
			public int rawSP;
			#endregion

			#region PROPERTIES
			/// <summary>
			/// I sorta just want this so I can make things easier on the save file screen.
			/// </summary>
			public int Level {
				get {
					float numerator = Mathf.Sqrt(totalExp);
					float denominator = Mathf.Sqrt(Mathf.Sqrt(totalExp)) * 0.5f;
					// return Mathf.CeilToInt(numerator / denominator);
					return Mathf.FloorToInt(numerator / denominator);
				}
			}
			#endregion

			/// <summary>
			/// Creates a serializable combatant from another combatant.
			/// </summary>
			/// <param name="combatant"></param>
			public SerializableCombatant(Combatant combatant) {
				this.metaData = combatant.metaData;
				this.totalExp = combatant.totalExp;
				this.hpCeil = combatant.hpCeil;
				this.mpCeil = combatant.mpCeil;
				this.stCeil = combatant.stCeil;
				this.maCeil = combatant.maCeil;
				this.enCeil = combatant.enCeil;
				this.agCeil = combatant.agCeil;
				this.luCeil = combatant.luCeil;
				this.rawHP = combatant.rawHP;
				this.rawMP = combatant.rawMP;
				this.rawSP = combatant.rawSP;
				foreach (ElementType element in new List<ElementType>(combatant.Resistances.Keys)) {
					ResistanceTuple resistanceTuple;
					resistanceTuple.element = element;
					resistanceTuple.resistance = combatant.Resistances[element];
					resistances.Add(resistanceTuple);
				}
				foreach (BehaviorType type in new List<BehaviorType>(combatant.AllBehaviors.Keys)) {
					foreach (BattleBehavior behavior in combatant.AllBehaviors[type]) {
						behaviors.Add(behavior.behaviorName);
					}
				}
			}
		}
		#endregion

	}

	/// <summary>
	/// A helper class that contains the details required to fill the level up screen.
	/// </summary>
	public class CombatantLevelUpResults {

		#region FIELDS - STATE
		public Combatant combatant { get; private set; }
		public int oldLevel { get; private set; }
		public int newLevel { get; private set; }
		#endregion

		#region FIELDS - COMPUTED
		public bool LeveledUp {
			get {
				return this.newLevel > this.oldLevel;
			}
		}
		#endregion

		#region CONSTRUCTORS
		public CombatantLevelUpResults(Combatant combatant, int oldLevel, int newLevel) {
			this.combatant = combatant;
			this.oldLevel = oldLevel;
			this.newLevel = newLevel;
		}
		#endregion

	}

	/*/// <summary>
	/// This is a class I'm probably going to delete later once I'm debugging.
	/// Ideally I'd make a scriptableobject for setting resistances of any given combatant.
	/// Future me, if you have combatants able to construct from scriptable objects, feel free to remove this entire class.
	/// </summary>
	public static class ResistancesLibrary {
		// More or less, this is just a set of definitions for resistances any given combatant can have.
		// I'm probably only going to be using this for debugging purposes. I'll delete it later.
		public static Dictionary<string, Dictionary<ElementType, ResistanceType>> dict = new Dictionary<string, Dictionary<ElementType, ResistanceType>>();
		static ResistancesLibrary() {
			dict["All Normal"] = new Dictionary<ElementType, ResistanceType>();
		}
	}*/

}


