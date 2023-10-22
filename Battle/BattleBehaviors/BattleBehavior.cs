using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Grawly.UI.Legacy;
using Sirenix.OdinInspector;
using Grawly.Battle.Functions;
using System.Linq;
using Grawly.UI;
using Grawly.Battle.BehaviorAnimation;
using Grawly.Battle.Modifiers;
using Sirenix.Serialization;
using Grawly.Battle.BattleMenu;
using Grawly.UI.MenuLists;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Grawly.Battle {

	[CreateAssetMenu(menuName = "Grawly/Battle Behavior")]
	public class BattleBehavior : SerializedScriptableObject, IMenuable {

		#region FIELDS - METADATA
		/// <summary>
		/// The name of the behavior that will be displayed to the player.
		/// </summary>
		[VerticalGroup("General")]
		[BoxGroup("General/General")]
		public string behaviorName;
		/// <summary>
		/// Helps sort behaviors in a menu. Has no effect on calculations beyond organization.
		/// </summary>
		[BoxGroup("General/General")]
		public int priority;
		/// <summary>
		/// Should this behavior be shown in the battle menu?
		/// Some behaviors such as Null Phys for example dont get shown.
		/// </summary>
		[BoxGroup("General/General")]
		public bool showInBattleMenu = true;
		/// <summary>
		/// A brief description of the move for the user.
		/// </summary>
		[BoxGroup("General/General"), Multiline]
		public string description;
		/// <summary>
		/// Organizes the behavior by type (Attack, Special, etc)
		/// </summary>
		[BoxGroup("General/General")]
		public BehaviorType behaviorType;
		/// <summary>
		/// Should the intent type for this behavior be set manually?
		/// </summary>
		[BoxGroup("General/General"), SerializeField]
		private bool overrideIntentType = false;
		/// <summary>
		/// The intent type for this battle behavior. Only gets used if it's being overriden.
		/// </summary>
		[BoxGroup("General/General"), ShowIf("overrideIntentType")]
		private IntentType intentType = IntentType.None;
		/// <summary>
		/// The intent type for this battle behavior.
		/// </summary>
		public IntentType IntentType {
			get {
				// If I'm manually setting the intent, return that.
				if (overrideIntentType == true) {
					return this.intentType;
				}

				// I need to figure out how to compute this shit later.

				// Otherwise, try to figure it out from context.
				if (this.elementType == ElementType.Healing) {
					return IntentType.Assistive;
				}

				return IntentType.None;
			}
		}
		/// <summary>
		/// The cost of this behavior if purchasing from a shop.
		/// </summary>
		[BoxGroup("General/General"), ShowIf("IsItemBehavior")]
		public int moneyCost = 999999;
		/// <summary>
		/// Elemental affiliation of the behavior (Fire, Elec, etc)
		/// </summary>
		[BoxGroup("General/General")]
		public ElementType elementType;
		#endregion

		#region FIELDS - ANIMATION TOGGLES
		/// <summary>
		/// Should this behavior show the damage amount in its animation?
		/// </summary>
		[Space(10)]
		[BoxGroup("General/Animation")]
		public BattleBehaviorAnimationToggleType showDamageAmountToggle = BattleBehaviorAnimationToggleType.Dynamic;
		/// <summary>
		/// Should this behavior show the resistance tag in its animation?
		/// </summary>
		[BoxGroup("General/Animation")]
		public BattleBehaviorAnimationToggleType showResistanceTagToggle = BattleBehaviorAnimationToggleType.Dynamic;
		/// <summary>
		/// Should the Miss tag be shown, at the very least?
		/// </summary>
		[BoxGroup("General/Animation"), ShowIf("AnimationToggleResistanceTagIsDynamicOrNever"), PropertyTooltip("Should the Miss tag be shown, at the very least?")]
		public BattleBehaviorAnimationToggleType showMissTagToggle = BattleBehaviorAnimationToggleType.Dynamic;
		/// <summary>
		/// Should this behavior show a messge on the battle notifier when it connects with a combatant?
		/// </summary>
		[BoxGroup("General/Animation")]
		public bool showNotifierMessageOnContact = false;
		/// <summary>
		/// The string to show on the battle notifier if the move is set to show something when the move connects.
		/// </summary>
		[BoxGroup("General/Animation"), ShowIf("showNotifierMessageOnContact")]
		public string notifierMessageToShowOnContact = "";
		/// <summary>
		/// The kind of notifier message to show if a message is shown on contact.
		/// </summary>
		[BoxGroup("General/Animation"), ShowIf("showNotifierMessageOnContact")]
		public BattleNotifier.BattleNotifierMessageType notifierTypeToShowOnContact = BattleNotifier.BattleNotifierMessageType.Normal;
		/// <summary>
		/// Should this behavior show a message when an affliction is placed on a target?
		/// </summary>
		[BoxGroup("General/Animation")]
		public bool showNotifierMessageOnAffliction = false;
		/// <summary>
		/// The string to show on the battle notifier if the move is set to show something when the move connects.
		/// </summary>
		[BoxGroup("General/Animation"), ShowIf("showNotifierMessageOnAffliction")]
		[InfoBox(message: "There are no afflictions set in this behavior! Disregard this if intentional.", infoMessageType: InfoMessageType.Warning, visibleIfMemberName: "HasNoAffliction")]
		public string notifierMessageToShowOnAffliction = "";
		/// <summary>
		/// The kind of notifier message to show if a message is shown on contact.
		/// </summary>
		[BoxGroup("General/Animation"), ShowIf("showNotifierMessageOnAffliction")]
		public BattleNotifier.BattleNotifierMessageType notifierTypeToShowOnAffliction = BattleNotifier.BattleNotifierMessageType.Harmful;
		#endregion

		#region FIELDS - POWER
		/// <summary>
		/// Determines how this move should be calcuated (Dynamically, by a percentage amount, etc)
		/// </summary>
		[VerticalGroup("Power", PaddingTop = 10f)]
		[BoxGroup("Power/Power"), Title("Calculation")]
		public BattleCalculationType calculationType;
		/// <summary>
		/// The base accuracy of the behavior.
		/// </summary>
		[BoxGroup("Power/Power"), ShowIf("HasCalculation"), Range(0, 1)]
		public float baseAccuracy;
		/// <summary>
		/// The base power of the behavior. 
		/// </summary>
		[BoxGroup("Power/Power"), ShowIf("HasCalculation")]
		public int basePower;
		/// <summary>
		/// The base critical rate of the behavior.
		/// </summary>
		[BoxGroup("Power/Power"), ShowIf("HasCalculation"), Range(0, 1)]
		public float baseCritRate;
		/// <summary>
		/// How many times will this move hit? 
		/// </summary>
		[BoxGroup("Power/Power"), ShowIf("HasCalculation")]
		public HitFrequencyType hitFrequencyType;
		/// <summary>
		/// The minimum/maximum amount of times this behavior should hit.
		/// </summary>
		[BoxGroup("Power/Power"), ShowIf("HasCalculation"), ShowIf("HasMultipleHits"), SerializeField]
		private Vector2Int hitFrequencyMinMax;
		/// <summary>
		/// A randomly generated int determining the amount of times this move should hit. Gets called during behavior evaluation.
		/// </summary>
		public int HitFrequency {                   // This is what actually gets called during evaluation.
			get {
				switch (hitFrequencyType) {
					case HitFrequencyType.Once:
						return 1;
					case HitFrequencyType.Multiple:
						int multipleResult = hitFrequencyMinMax.x;
						// Sometimes there are issues where the range gets fucked up in the inspector.
						if (multipleResult < 1) {
							Debug.LogError(this.behaviorName + " had an issue with generating a hit frequency. Returning default frequency of 2.");
							return 2;
						} else {
							return multipleResult;
						}
					case HitFrequencyType.Range:
						int rangeResult = Random.Range(hitFrequencyMinMax.x, hitFrequencyMinMax.y);
						// Sometimes there are issues where the range gets fucked up in the inspector.
						if (rangeResult < 1) {
							Debug.LogError(this.behaviorName + " had an issue with generating a hit frequency. Returning default frequency of 2.");
							return 2;
						} else {
							return rangeResult;
						}
					default:
						Debug.LogError("Hit Frequency Type invalid!");
						return 1;
				}
			}
		}
		/// <summary>
		/// Should this behavior ignore EN? (defense)
		/// </summary>
		[BoxGroup("Power/Power"), ShowIf("HasCalculation"), PropertyTooltip("Should this behavior ignore defense? Helpful for healing moves.")]
		public bool ignoreEN = false;
		#endregion

		#region FIELDS - RESOURCE POOLS
		/// <summary>
		/// The resource to be used when executing a behavior.
		/// </summary>
		[BoxGroup("Power/Power"), Title("Resources")]
		public BehaviorCostType costType;
		/// <summary>
		/// The base cost to use this behavior.
		/// </summary>
		[BoxGroup("Power/Power"), ShowIf("HasCostType")]
		public int baseCost;
		/// <summary>
		/// The target group for this behavior.
		/// </summary>
		[BoxGroup("Power/Power")]
		public TargetType targetType;
		/// <summary>
		/// The resource that should be affected when executing this behavior against the target group.
		/// </summary>
		[BoxGroup("Power/Power")]
		public BehaviorCostType targetResource;
		/// <summary>
		/// The participants in a tactics behavior.
		/// </summary>
		[BoxGroup("Power/Power"), ShowIf("IsTacticsBehavior")]
		public List<TacticsParticipantType> participants;
		#endregion

		#region FIELDS - STATUS MODIFIERS
		[VerticalGroup("Status Modifiers", PaddingTop = 10f)]
		[BoxGroup("Status Modifiers/Status Modifiers")]
		/// <summary>
		/// The boost types this behavior should buff/debuff. Note that buff/debuff is determined by the target group (i.e., allies will always be buffed, enemies will always be debuffed.)
		/// </summary>
		[TabGroup("Status Modifiers/Status Modifiers/Boosts", "Boosts")]
		public List<PowerBoostType> boostTypes;
		/// <summary>
		/// The afflictions that this behavior should cure.
		/// </summary>
		[TabGroup("Status Modifiers/Status Modifiers/Boosts", "Cures"), InfoBox("Behavior has cures and afflictions set at same time. Only cures will be used.", InfoMessageType.Warning, "HasAfflictionAndCure")]
		public List<AfflictionType> afflictionTypeCures = new List<AfflictionType>();
		/// <summary>
		/// The afflictions that this behavior should inflict.
		/// </summary>
		[TabGroup("Status Modifiers/Status Modifiers/Boosts", "Afflictions"), InfoBox("Behavior has cures and afflictions set at same time. Only cures will be used.", InfoMessageType.Warning, "HasAfflictionAndCure")]
		public List<AfflictionType> afflictionTypes = new List<AfflictionType>();
		/// <summary>
		/// The accuracy of giving/curing an affliction. This is independent from the behavior's normal accuracy.
		/// </summary>
		[TabGroup("Status Modifiers/Status Modifiers/Boosts", "Afflictions"), ShowIf("HasAffliction"), Range(0, 1)]
		public float afflictionAccuracy;
		#endregion

		#region FIELDS - FUNCTIONS
		[VerticalGroup("Functions", PaddingTop = 10f)]
		[BoxGroup("Functions/Functions")]
		/// <summary>
		/// The function to execute once it has been selected.
		/// </summary>
		[TabGroup("Functions/Functions/Functions", "Battle"), OdinSerialize]
		private BattleBehaviorFunction battleFunction;
		/// <summary>
		/// The function to execute once it has been selected.
		/// </summary>
		public BattleBehaviorFunction BattleFunction {
			get {
				// Actually make sure that this behavior has its function. Sometimes Unity does weird things and removes it (if this happens, investigate.)
				if (this.battleFunction == null) {
					Debug.LogError("The battle function was missing! Assigning the standard function to this template.");
					this.battleFunction = new StandardBattleBehaviorFunction();
				}
				return this.battleFunction;
			}
		}
		/// <summary>
		/// A list of passive functions that modify this behavior in some way/shape/form passively.
		/// </summary>
		[TabGroup("Functions/Functions/Functions", "Passive"), ListDrawerSettings(Expanded = true), OdinSerialize]
		private List<BattleBehaviorModifier> passiveFunctions = new List<BattleBehaviorModifier>();
		/// <summary>
		/// A list of passive functions that modify this behavior in some way/shape/form.
		/// </summary>
		public List<BattleBehaviorModifier> PassiveFunctions {
			get {
				return this.passiveFunctions;
			}
		}
		/// <summary>
		/// The functions to execute when this behavior is selected from the pause menu.
		/// </summary>
		[TabGroup("Functions/Functions/Functions", "Pause"), ListDrawerSettings(Expanded = true), OdinSerialize]
		private List<PauseBehaviorFunction> pauseFunctions = new List<PauseBehaviorFunction>();
		/// <summary>
		/// The functions to execute when this behavior is selected from the pause menu.
		/// </summary>
		public List<PauseBehaviorFunction> PauseFunctions {
			get {
				return this.pauseFunctions;
			}
		}
		#endregion

		#region FIELDS - EFFECTS
		/// <summary>
		/// The animation that should be used for this move.
		/// Sometimes its irrelevent, but it's good because I can replace the EffectsController.
		/// </summary>
		[TabGroup("Metadata", "Animation"), OdinSerialize]
		private BattleBehaviorAnimation behaviorAnimation;
		/// <summary>
		/// The animation that should be used for this move.
		/// Sometimes its irrelevent, but it's good because I can replace the EffectsController.
		/// </summary>
		public BattleBehaviorAnimation BehaviorAnimation {
			get {
				// Sometimes I may have forgotten to generate the animation for the move. In such an event, try to infer what to use.
				if (this.behaviorAnimation == null) {
					Debug.LogWarning("BATTLE: BehaviorAnimation on " + this.behaviorName + " is null! Will try to infer what animation to use.");


					if (this.IsAssistive == true && this.TargetsMultipleCombatants == true) {
						this.behaviorAnimation = new RestorationMultipleTarget();

					} else if (this.IsAssistive == true && this.TargetsMultipleCombatants == false) {
						this.behaviorAnimation = new RestorationSingleTarget();

					} else if (this.IsAssistive == false && this.TargetsMultipleCombatants == true) {
						this.behaviorAnimation = new OffensiveMultipleTarget();

					} else if (this.IsAssistive == false && this.TargetsMultipleCombatants == false) {
						this.behaviorAnimation = new OffensiveSingleTarget();

					} else {
						Debug.LogError("Couldn't infer default target type. Assigning behavior to be single target offensive.");
						this.behaviorAnimation = new OffensiveSingleTarget();
					}
				}

				return this.behaviorAnimation;
			}
		}
		#endregion

		#region FIELDS - NOTES
		[TabGroup("Parse Notes", "Parse Notes")]
		public string parseNotes = "";
		#endregion

		#region FIELDS - COMPUTED
		/// <summary>
		/// Does this behavior target multiple combatants?
		/// </summary>
		public bool TargetsMultipleCombatants {
			get {
				switch (this.targetType) {
					case TargetType.AllAliveAllies:
					case TargetType.AllAliveCombatants:
					case TargetType.AllAliveEnemies:
					case TargetType.AllDeadAllies:
						return true;
					default:
						return false;
				}
			}
		}
		/// <summary>
		/// Is this move intended to assist allies?
		/// </summary>
		public bool IsAssistive {
			get {
				switch (this.elementType) {
					case ElementType.Healing:
					case ElementType.Assist:
						return true;
					default:
						return false;
				}
			}
		}
		#endregion

		#region INTERFACE IMPLEMENTATION - IMENUABLE
		public string PrimaryString {
			get {
				return this.behaviorName;
			}
		}
		public string QuantityString {
			get {
				switch (this.behaviorType) {
					case BehaviorType.Item:
						return "x" + GameController.Instance.Variables.Items[key: this].ToString();
					default:
						return this.baseCost.ToString() + ((this.costType == BehaviorCostType.HP) ? "HP" : "MP");
				}
			}
		}
		public string DescriptionString {
			get {
				return this.description;
			}
		}
		public Sprite Icon {
			get {
				return DataController.GetDefaultElementalIcon(elementType: this.elementType);
			}
		}
		#endregion

		#region ODIN FUNCTIONS
		private bool HasMultipleHits() {
			if (hitFrequencyType == HitFrequencyType.Once) {
				return false;
			} else {
				return true;
			}
		}

		private bool HasAffliction() {
			if (afflictionTypes.Count == 0) {
				return false;
			} else {
				return true;
			}
		}
		private bool HasNoAffliction() {
			return !this.HasAffliction();
		}
		private bool HasAfflictionAndCure() {
			if (afflictionTypes.Count > 0 && afflictionTypeCures.Count > 0) {
				return true;
			} else {
				return false;
			}
		}

		private bool HasCalculation() {
			if (calculationType == BattleCalculationType.None) {
				return false;
			} else {
				return true;
			}
		}
		private bool HasPercentageCalculation() {
			if (calculationType == BattleCalculationType.Percentage) {
				return true;
			} else {
				return false;
			}
		}

		private bool HasCostType() {
			if (costType == BehaviorCostType.None) {
				return false;
			} else {
				return true;
			}
		}

		/*private bool HasFunctionOther() {
			if (functionType == BattleFunctionType.Other) {
				return true;
			} else {
				return false;
			}
		}*/
		private bool IsTacticsBehavior {
			get {
				return (this.behaviorType == BehaviorType.Tactics);
			}
		}

		private bool IsItemBehavior() {
			if (behaviorType == BehaviorType.Item) {
				return true;
			} else {
				return false;
			}
		}
		
		// Animation Helpers
		/// <summary>
		/// Returns true if the resistance toggle is set to dynamic or never.
		/// </summary>
		/// <returns></returns>
		private bool AnimationToggleResistanceTagIsDynamicOrNever() {
			return this.showResistanceTagToggle == BattleBehaviorAnimationToggleType.Dynamic || this.showResistanceTagToggle == BattleBehaviorAnimationToggleType.Never;
		}

		// Colors
		private Color CalculationInspectorColor() {
			return Color.red;
		}

		#endregion

/*	#region FIELDS - METADATA
		/// <summary>
		/// The name of the behavior that will be displayed to the player.
		/// </summary>
		[TabGroup("Metadata", "General")]
		public string behaviorName;
		/// <summary>
		/// Helps sort behaviors in a menu. Has no effect on calculations beyond organization.
		/// </summary>
		[TabGroup("Metadata", "General")]
		public int priority;
		/// <summary>
		/// Should this behavior be shown in the battle menu?
		/// Some behaviors such as Null Phys for example dont get shown.
		/// </summary>
		[TabGroup("Metadata", "General")]
		public bool showInBattleMenu = true;
		/// <summary>
		/// A brief description of the move for the user.
		/// </summary>
		[TabGroup("Metadata", "General"), Multiline]
		public string description;
		/// <summary>
		/// Organizes the behavior by type (Attack, Special, etc)
		/// </summary>
		[TabGroup("Metadata", "General")]
		public BehaviorType behaviorType;
		/// <summary>
		/// Should the intent type for this behavior be set manually?
		/// </summary>
		[TabGroup("Metadata", "General"), SerializeField]
		private bool overrideIntentType = false;
		/// <summary>
		/// The intent type for this battle behavior. Only gets used if it's being overriden.
		/// </summary>
		[TabGroup("Metadata", "General"), ShowIf("overrideIntentType")]
		private IntentType intentType = IntentType.None;
		/// <summary>
		/// The intent type for this battle behavior.
		/// </summary>
		public IntentType IntentType {
			get {
				// If I'm manually setting the intent, return that.
				if (overrideIntentType == true) {
					return this.intentType;
				}

				// I need to figure out how to compute this shit later.

				// Otherwise, try to figure it out from context.
				if (this.elementType == ElementType.Healing) {
					return IntentType.Assistive;
				}

				return IntentType.None;
			}
		}
		/// <summary>
		/// The cost of this behavior if purchasing from a shop.
		/// </summary>
		[TabGroup("Metadata", "General"), ShowIf("IsItemBehavior")]
		public int moneyCost = 999999;
		#endregion

		#region FIELDS - ANIMATION TOGGLES
		/// <summary>
		/// Should this behavior show the damage amount in its animation?
		/// </summary>
		[TabGroup("Metadata", "Animation")]
		public BattleBehaviorAnimationToggleType showDamageAmountToggle = BattleBehaviorAnimationToggleType.Dynamic;
		/// <summary>
		/// Should this behavior show the resistance tag in its animation?
		/// </summary>
		[TabGroup("Metadata", "Animation")]
		public BattleBehaviorAnimationToggleType showResistanceTagToggle = BattleBehaviorAnimationToggleType.Dynamic;
		/// <summary>
		/// Should the Miss tag be shown, at the very least?
		/// </summary>
		[TabGroup("Metadata", "Animation"), ShowIf("AnimationToggleResistanceTagIsDynamicOrNever"), PropertyTooltip("Should the Miss tag be shown, at the very least?")]
		public BattleBehaviorAnimationToggleType showMissTagToggle = BattleBehaviorAnimationToggleType.Dynamic;
		/// <summary>
		/// Should this behavior show a messge on the battle notifier when it connects with a combatant?
		/// </summary>
		[TabGroup("Metadata", "Animation")]
		public bool showNotifierMessageOnContact = false;
		/// <summary>
		/// The string to show on the battle notifier if the move is set to show something when the move connects.
		/// </summary>
		[TabGroup("Metadata", "Animation"), ShowIf("showNotifierMessageOnContact")]
		public string notifierMessageToShowOnContact = "";
		/// <summary>
		/// The kind of notifier message to show if a message is shown on contact.
		/// </summary>
		[TabGroup("Metadata", "Animation"), ShowIf("showNotifierMessageOnContact")]
		public BattleNotifier.BattleNotifierMessageType notifierTypeToShowOnContact = BattleNotifier.BattleNotifierMessageType.Normal;
		/// <summary>
		/// Should this behavior show a message when an affliction is placed on a target?
		/// </summary>
		[TabGroup("Metadata", "Animation")]
		public bool showNotifierMessageOnAffliction = false;
		/// <summary>
		/// The string to show on the battle notifier if the move is set to show something when the move connects.
		/// </summary>
		[TabGroup("Metadata", "Animation"), ShowIf("showNotifierMessageOnAffliction")]
		[InfoBox(message: "There are no afflictions set in this behavior! Disregard this if intentional.", infoMessageType: InfoMessageType.Warning, visibleIfMemberName: "HasNoAffliction")]
		public string notifierMessageToShowOnAffliction = "";
		/// <summary>
		/// The kind of notifier message to show if a message is shown on contact.
		/// </summary>
		[TabGroup("Metadata", "Animation"), ShowIf("showNotifierMessageOnAffliction")]
		public BattleNotifier.BattleNotifierMessageType notifierTypeToShowOnAffliction = BattleNotifier.BattleNotifierMessageType.Harmful;
		#endregion*/

	}
}