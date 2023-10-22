using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Grawly.Battle.WorldEnemies;
using DG.Tweening;
using Grawly.Toggles;
using Grawly.Toggles.Proto;

namespace Grawly.Battle.BattleMenu {

	/// <summary>
	/// This is what should be displayed on screen when highlighting an enemy.
	/// </summary>
	public class EnemyCursorDX : MonoBehaviour, ISelectHandler, ICancelHandler, ISubmitHandler, IDeselectHandler, IMoveHandler {

		#region FIELDS - STATE
		/// <summary>
		/// The world enemy this cursor may be attached to at any given time.
		/// </summary>
		private WorldEnemyDX worldEnemyDX;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The selectable attached to this component.
		/// </summary>
		[Title("Misc")]
		[SerializeField, TabGroup("Cursor", "Misc References")]
		private Selectable selectable;
		/// <summary>
		/// The RectTransform that encapsulates all of the visuals for this cursor.
		/// Note that the selectable is not contained here; it is on the parent.
		/// </summary>
		[SerializeField, TabGroup("Cursor", "Misc References")]
		private RectTransform allCursorVisualsRectTransform;
		#endregion

		#region FIELDS - SCENE REFERENCES : DIAMOND GAMEOBJECTS
		/// <summary>
		/// The diamond representing the health/magic bar.
		/// </summary>
		[Title("Diamond Objects")]
		[SerializeField, TabGroup("Cursor", "Misc References")]
		private GameObject healthBarDiamondGameObject;
		/// <summary>
		/// The diamond representing the Resistance.
		/// </summary>
		[SerializeField, TabGroup("Cursor", "Misc References")]
		private GameObject resistanceDiamondGameObject;
		#endregion

		#region FIELDS - SCENE REFERENCES : THINGS INSIDE THE DIAMONDS
		/// <summary>
		/// The image used for the health bar.
		/// </summary>
		[Title("Diamond Insides")]
		[SerializeField, TabGroup("Cursor", "Misc References")]
		private Image healthBarImage;
		/// <summary>
		/// The STM that shows how much damage the enemy has taken.
		/// </summary>
		[SerializeField, TabGroup("Cursor", "Misc References")]
		private SuperTextMesh damageTextLabel;
		/// <summary>
		/// The text that gets shown inside the resistance diamond if the enemy has an affliction.
		/// </summary>
		[SerializeField, TabGroup("Cursor", "Misc References")]
		private Text resistanceTypeText;
		#endregion

		#region FIELDS - SCENE REFERENCES : ENEMY INFO TAG
		/// <summary>
		/// The GameObject for showing the enemy's information.
		/// </summary>
		[Title("Info Tag")]
		[SerializeField, TabGroup("Cursor", "Misc References")]
		private GameObject enemyInfoTagGameObject;
		/// <summary>
		/// The super text mesh showing the enemy's name.
		/// </summary>
		[SerializeField, TabGroup("Cursor", "Misc References")]
		private SuperTextMesh enemyInfoNameLabel;
		/// <summary>
		/// The super text mesh showing the enemy's level.
		/// </summary>
		[SerializeField, TabGroup("Cursor", "Misc References")]
		private SuperTextMesh enemyInfoLevelLabel;
		#endregion

		#region FIELDS - SCENE REFERENCES : DEBUG INFORMATION
		/// <summary>
		/// The GameObject for showing debug information on an enemy.
		/// </summary>
		[Title("Debug Information")]
		[SerializeField, TabGroup("Cursor", "Misc References")]
		private GameObject debugInformationGameObject;
		/// <summary>
		/// The label for showing the enemy's current resources.
		/// </summary>
		[SerializeField, TabGroup("Cursor", "Misc References")]
		private SuperTextMesh resourcesDebugLabel;
		/// <summary>
		/// The label for showing the enemy's current attributes.
		/// </summary>
		[SerializeField, TabGroup("Cursor", "Misc References")]
		private SuperTextMesh attributesDebugLabel;
		/// <summary>
		/// The label for showing what the current difficulty is, just as a reminder.
		/// </summary>
		[SerializeField, TabGroup("Cursor", "Misc References")]
		private SuperTextMesh difficultyLabel;
		#endregion
		
		#region FIELDS - SCENE REFERENCES : RESISTANCE TAGS
		/// <summary>
		/// The image for showing the Weak resistance type.
		/// </summary>
		[SerializeField, TabGroup("Cursor", "Tag References")]
		private Image weakTagImage;
		/// <summary>
		/// The image for showing the Block resistance type.
		/// </summary>
		[SerializeField, TabGroup("Cursor", "Tag References")]
		private Image blockTagImage;
		/// <summary>
		/// The image for showing the Critical accuracy type.
		/// </summary>
		[SerializeField, TabGroup("Cursor", "Tag References")]
		private Image critTagImage;
		/// <summary>
		/// The image for showing the Miss accuracy type.
		/// </summary>
		[SerializeField, TabGroup("Cursor", "Tag References")]
		private Image missTagImage;
		/// <summary>
		/// The image for showing the Reflect resistance type.
		/// </summary>
		[SerializeField, TabGroup("Cursor", "Tag References")]
		private Image reflectTagImage;
		/// <summary>
		/// The image for showing the Resist resistance type.
		/// </summary>
		[SerializeField, TabGroup("Cursor", "Tag References")]
		private Image resistTagImage;
		#endregion

		#region FIELDS - SCENE REFERENCES : BOOST TAGS
		/// <summary>
		/// The GameObject that has all the visuals for the accuracy boost icon.
		/// </summary>
		[SerializeField, TabGroup("Cursor", "Boost References")]
		private GameObject accuracyBoostGameObject;
		/// <summary>
		/// The Image that has the arrow for the accuracy boost.
		/// </summary>
		[SerializeField, TabGroup("Cursor", "Boost References")]
		private Image accuracyBoostArrowImage;
		/// <summary>
		/// The GameObject that has all the visuals for the defense boost icon.
		/// </summary>
		[SerializeField, TabGroup("Cursor", "Boost References")]
		private GameObject defenseBoostGameObject;
		/// <summary>
		/// The Image that has the arrow for the defeense boost.
		/// </summary>
		[SerializeField, TabGroup("Cursor", "Boost References")]
		private Image defenseBoostArrowImage;
		/// <summary>
		/// The GameObject that has all the visuals for the attack boost icon.
		/// </summary>
		[SerializeField, TabGroup("Cursor", "Boost References")]
		private GameObject attackBoostGameObject;
		/// <summary>
		/// The Image that has the arrow for the attack boost.
		/// </summary>
		[SerializeField, TabGroup("Cursor", "Boost References")]
		private Image attackBoostArrowImage;
		#endregion

		#region FIELDS - SCENE REFERENCES : DIAMOND DICTIONARY TO STORE THE THINGS ABOVE
		/// <summary>
		/// A dictionary storing the different diamond gameobjects.
		/// </summary>
		private Dictionary<EnemyCursorDiamondType, GameObject> diamondGameObjectDict = new Dictionary<EnemyCursorDiamondType, GameObject>();
		/// <summary>
		/// A list of the tags listed above. Mostly so I can quickly turn them off when needed.
		/// </summary>
		private List<Image> resistanceTagImages;
		#endregion

		#region UNITY CALLS
		private void Awake() {
			// I don't want to use SerializedMonoBehavior but I do want these references stored in a dictionary. So, initialize the dict.
			this.diamondGameObjectDict.Add(key: EnemyCursorDiamondType.MeterDiamond, value: this.healthBarDiamondGameObject);
			this.diamondGameObjectDict.Add(key: EnemyCursorDiamondType.ResistanceDiamond, value: this.resistanceDiamondGameObject);
			// Also store the tags into a list I can quickly iterate through.
			this.resistanceTagImages = new List<Image>() { this.weakTagImage, this.blockTagImage, this.critTagImage, this.missTagImage, this.reflectTagImage, this.resistTagImage };

		}
		private void Update() {
			// If the rect transform for the visuals is active, update the position.
			if (this.allCursorVisualsRectTransform.gameObject.activeInHierarchy == true) {
				// this.transform.position = this.worldEnemyDX.WorldBehavior.GetCursorPosition(worldEnemyDX: this.worldEnemyDX);
				this.transform.position = this.worldEnemyDX.CursorPosition;
			}
		}
		#endregion

		#region SETUP
		/// <summary>
		/// Preps this cursor to be used for the specified world enemy.
		/// It is assumed this version of the method is being called during target selection and not as it is being animated.
		/// </summary>
		/// <param name="worldEnemyDX">The world enemy this cursor will be attached to.</param>
		/// <param name="currentBattleBehavior">The BattleBehavior that was currently picked from the menu.</param>
		/// <param name="isSelectable">Is this cursor selectable or not?</param>
		public void BuildEnemyCursorDX(WorldEnemyDX worldEnemyDX, BattleBehavior currentBattleBehavior, bool isSelectable) {

			// Call the method that preps the cursor for all the things I need it do do.
			this.PrepareCursorForBuilding(worldEnemyDX: worldEnemyDX);

			// Adjust the selectable to whether or not this cursor can be. Selected.
			this.selectable.enabled = isSelectable;

			// Consequently, by default, set the visuals to be active as the inverse of whether this is selectable or not.
			this.allCursorVisualsRectTransform.gameObject.SetActive(value: !isSelectable);

			// Check if the enemy has a resistance to the move and if that resistance is known. If its anything other than normal, display it.
			if (GameController.Instance.Variables.GetKnownEnemyResistance(enemy: worldEnemyDX.Enemy, behavior: currentBattleBehavior).HasValue
				&& worldEnemyDX.Enemy.CheckResistance(behavior: currentBattleBehavior) != ResistanceType.Nm) {
				this.resistanceDiamondGameObject.SetActive(true);
				this.resistanceTypeText.text = worldEnemyDX.Enemy.CheckResistance(behavior: currentBattleBehavior).ToString();
			} else {
				this.resistanceDiamondGameObject.SetActive(false);
			}

			// Also build the enemy info tag.
			this.enemyInfoTagGameObject.SetActive(true);
			this.enemyInfoNameLabel.Text = worldEnemyDX.Enemy.metaData.name;
			this.enemyInfoLevelLabel.Text = "<size=30>Lv</size>" + worldEnemyDX.Enemy.Level.ToString();
			// Call the helper method for the boost icons bc its kinda long.
			this.EnableBoostIcons(worldEnemyDX: worldEnemyDX);

			// If the show debug info flag is on, prep the debug labels.
			if (ToggleController.GetToggle<ShowEnemyDebugInfo>().GetToggleBool() == true) {
				this.debugInformationGameObject.SetActive(true);
				this.SetupDebugLabels(worldEnemyDX: worldEnemyDX);
			} else {
				// Otherwise, just turn it off.
				this.debugInformationGameObject.SetActive(false);
			}

		}
		/// <summary>
		/// Builds a cursor from a damage calculation. This is mostly for effect.
		/// </summary>
		/// <param name="worldEnemyDX"></param>
		/// <param name="currentBattleBehavior"></param>
		/// <param name="isSelectable"></param>
		public void BuildEnemyCursorDX(WorldEnemyDX worldEnemyDX, DamageCalculation damageCalculation) {

			// Call the method that preps the cursor for all the things I need it do do.
			this.PrepareCursorForBuilding(worldEnemyDX: worldEnemyDX);

			// Turn the visuals on.
			this.allCursorVisualsRectTransform.gameObject.SetActive(value: true);

			// Selectable should not be enabled for damage calculations.
			this.selectable.enabled = false;

			// Set the text on the cursor. Make it jitter if the target is under attack. Make it green if healing.
			if (damageCalculation.TargetShouldShowDamageAmount == true) {
				this.damageTextLabel.gameObject.SetActive(true);
				this.damageTextLabel.Text = (damageCalculation.TargetTookHPDamage ? "<j=sample>" : "<c=greeny>") + damageCalculation.DamageMagnitude.ToString();

				// Also tween the calculation to show the fill amount properlly.
				DOTween.To(
					getter: () => this.healthBarImage.fillAmount,
					setter: x => this.healthBarImage.fillAmount = x,
					endValue: damageCalculation.NewFinalTargetHPRatio,
					duration: 1f);

			} else {
				this.healthBarDiamondGameObject.SetActive(false);
			}

			// If the intention is to attack this enemy, shake the transform that contains all the visuals.
			if (damageCalculation.TargetTookHPDamage == true) {
				this.allCursorVisualsRectTransform.DOShakeAnchorPos(duration: 0.5f, strength: 50, vibrato: 50);
			}

			// If the target is being hit by a move where the resistance tag is relevant, show it.
			if (damageCalculation.TargetShouldShowResistanceTag == true) {
				this.EnableResistanceTag(damageCalculation: damageCalculation);
			}

			// Make sure the info and debug tags are OFF for damage calculations.
			this.enemyInfoTagGameObject.SetActive(false);
			this.debugInformationGameObject.SetActive(false);

		}
		/// <summary>
		/// Builds a cursor from a simple damage amount.
		/// Handy for things like afflictions.
		/// </summary>
		/// <param name="worldEnemyDX">The WorldEnemyDX who needs its cursor built.</param>
		/// <param name="damageAmount">The amount of damage to show the bar tweening.</param>
		/// <param name="resourceType">The resource this damage is targeting. Affects things like bar color.</param>
		public void BuildEnemyCursorDX(WorldEnemyDX worldEnemyDX, int damageAmount, BehaviorCostType resourceType) {

			// First and foremost, determine what the final ratio for the HP bar should be.
			float finalRatio = 1f;
			switch (resourceType) {
				case BehaviorCostType.HP:
					finalRatio = Mathf.Clamp01((float)(worldEnemyDX.Enemy.HP - damageAmount) / (float)worldEnemyDX.Enemy.MaxHP);
					break;
				case BehaviorCostType.MP:
					finalRatio = Mathf.Clamp01((float)(worldEnemyDX.Enemy.MP - damageAmount) / (float)worldEnemyDX.Enemy.MaxMP);
					break;
				default:
					Debug.LogError("Couldn't determine what resource type to use!");
					break;
			}


			// Call the method that preps the cursor for all the things I need it do do.
			this.PrepareCursorForBuilding(worldEnemyDX: worldEnemyDX);

			// Turn the visuals on.
			this.allCursorVisualsRectTransform.gameObject.SetActive(value: true);

			// Selectable should not be enabled for damage calculations.
			this.selectable.enabled = false;

			this.damageTextLabel.gameObject.SetActive(true);
			this.damageTextLabel.Text = (damageAmount > 0 ? "<j=sample>" : "<c=greeny>") + damageAmount.ToString();

			// Also tween the calculation to show the fill amount properlly.
			DOTween.To(
				getter: () => this.healthBarImage.fillAmount,
				setter: x => this.healthBarImage.fillAmount = x,
				endValue: finalRatio,
				duration: 1f);

			// Shake the cursor visuals.
			this.allCursorVisualsRectTransform.DOShakeAnchorPos(duration: 0.5f, strength: 50, vibrato: 50);

			// Make sure the info and debug tags are OFF for damage calculations.
			this.enemyInfoTagGameObject.SetActive(false);
			this.debugInformationGameObject.SetActive(false);

		}
		/// <summary>
		/// A quick routine to prep the cursor for the building I need to do because I ended up repeating a lot of lines and I just wanna save it.
		/// </summary>
		private void PrepareCursorForBuilding(WorldEnemyDX worldEnemyDX) {

			// Remember the world enemy.
			this.worldEnemyDX = worldEnemyDX;

			// Turn on the GameObject. Because I'm apparently not doing that.
			this.gameObject.SetActive(true);

			// Iterate through each of the tags and turn them off.
			this.resistanceTagImages.ForEach(i => i.gameObject.SetActive(false));

			// Turn ON the health bar diamond.
			this.healthBarDiamondGameObject.SetActive(true);

			// Turn off the resistance diamond in case it was turned on earlier.
			this.resistanceDiamondGameObject.SetActive(false);

			// Set the fill amount of the HP meter to the enemy's HP ratio.
			this.healthBarImage.fillAmount = worldEnemyDX.Enemy.HPRatio;

			// Set the default position of the cursor so that I don't see that weird thing where it appears in the center of the screen a frame before its selected.
			// this.transform.position = worldEnemyDX.WorldBehavior.GetCursorPosition(worldEnemyDX: worldEnemyDX);
			this.transform.position = worldEnemyDX.CursorPosition;
			
			// Reset the damage text.
			this.damageTextLabel.Text = "";
		}
		#endregion

		#region HELPERS - TAG ANIMATION
		/// <summary>
		/// A helper function for enabling a resistance tag as a result of a damage calculation.
		/// </summary>
		/// <param name="damageCalculation">The calculation that should be read from.</param>
		private void EnableResistanceTag(DamageCalculation damageCalculation) {
			// If the damage calculation had a specific accuracy/resistance it needs to show, turn on the appropriate object. Accuracy takes priority over resistance.
			if (damageCalculation.accuracyType == AccuracyType.Critical || damageCalculation.accuracyType == AccuracyType.Miss) {
				switch (damageCalculation.accuracyType) {
					case AccuracyType.Critical:
						this.critTagImage.gameObject.SetActive(true);
						break;
					case AccuracyType.Miss:
						this.missTagImage.gameObject.SetActive(true);
						break;
					default:
						break;
				}
			} else if (damageCalculation.finalResistance != ResistanceType.Nm) {
				switch (damageCalculation.finalResistance) {
					case ResistanceType.Wk:
						this.weakTagImage.gameObject.SetActive(true);
						break;
					case ResistanceType.Str:
						this.resistTagImage.gameObject.SetActive(true);
						break;
					case ResistanceType.Ref:
						this.reflectTagImage.gameObject.SetActive(true);
						break;
					case ResistanceType.Abs:
						throw new System.NotImplementedException("I never actually made an absorb tag?");
						break;
					case ResistanceType.Nul:
						this.blockTagImage.gameObject.SetActive(true);
						break;
					default:
						break;
				}
			}

			/*// If the damage calculation had a specific accuracy/resistance it needs to show, turn on the appropriate object. Accuracy takes priority over resistance.
			if (damageCalculation.accuracyType == AccuracyType.Critical || damageCalculation.accuracyType == AccuracyType.Miss) {
				switch (damageCalculation.accuracyType) {
					case AccuracyType.Critical:
						this.critTagImage.gameObject.SetActive(true);
						break;
					case AccuracyType.Miss:
						this.missTagImage.gameObject.SetActive(true);
						break;
					default:
						break;
				}
			} else if (damageCalculation.finalResistance != ResistanceType.Nm) {
				switch (damageCalculation.finalResistance) {
					case ResistanceType.Wk:
						this.weakTagImage.gameObject.SetActive(true);
						break;
					case ResistanceType.Str:
						this.resistTagImage.gameObject.SetActive(true);
						break;
					case ResistanceType.Ref:
						this.reflectTagImage.gameObject.SetActive(true);
						break;
					case ResistanceType.Abs:
						throw new System.NotImplementedException("I never actually made an absorb tag?");
						break;
					case ResistanceType.Nul:
						this.blockTagImage.gameObject.SetActive(true);
						break;
					default:
						break;
				}
			}*/

		}
		/// <summary>
		/// Just a helper routine for setting the boost icons up.
		/// </summary>
		/// <param name="worldEnemyDX"></param>
		private void EnableBoostIcons(WorldEnemyDX worldEnemyDX) {

			if (worldEnemyDX.Enemy.GetPowerBoost(PowerBoostType.Accuracy) != 1f) {
				this.accuracyBoostGameObject.SetActive(true);
				if (worldEnemyDX.Enemy.GetPowerBoost(PowerBoostType.Accuracy) > 1f) {
					this.accuracyBoostArrowImage.color = GrawlyColors.colorDict[GrawlyColorTypes.Red];
				} else {
					this.accuracyBoostArrowImage.color = GrawlyColors.colorDict[GrawlyColorTypes.Blue];
				}
			} else {
				this.accuracyBoostGameObject.SetActive(false);
			}

			if (worldEnemyDX.Enemy.GetPowerBoost(PowerBoostType.Defense) != 1f) {
				this.defenseBoostGameObject.SetActive(true);
				if (worldEnemyDX.Enemy.GetPowerBoost(PowerBoostType.Defense) > 1f) {
					this.defenseBoostArrowImage.color = GrawlyColors.colorDict[GrawlyColorTypes.Red];
				} else {
					this.defenseBoostArrowImage.color = GrawlyColors.colorDict[GrawlyColorTypes.Blue];
				}
			} else {
				this.defenseBoostGameObject.SetActive(false);
			}

			if (worldEnemyDX.Enemy.GetPowerBoost(PowerBoostType.Attack) != 1f) {
				this.attackBoostGameObject.SetActive(true);
				if (worldEnemyDX.Enemy.GetPowerBoost(PowerBoostType.Attack) > 1f) {
					this.attackBoostArrowImage.color = GrawlyColors.colorDict[GrawlyColorTypes.Red];
				} else {
					this.attackBoostArrowImage.color = GrawlyColors.colorDict[GrawlyColorTypes.Blue];
				}
			} else {
				this.attackBoostGameObject.SetActive(false);
			}

		}
		/// <summary>
		/// Sets up the labels on the text meshes that show an enemy's information.
		/// </summary>
		/// <param name="worldEnemyDx"></param>
		private void SetupDebugLabels(WorldEnemyDX worldEnemyDX) {
			
			string resourcesStr = "";
			resourcesStr += "<c=white>HP: " + worldEnemyDX.Enemy.HP + "/" + worldEnemyDX.Enemy.MaxHP + "\n";
			resourcesStr += "<c=white>MP: " + worldEnemyDX.Enemy.MP + "/" + worldEnemyDX.Enemy.MaxMP;

			string attributesStr = "<c=white>";
			attributesStr += "ST: " + worldEnemyDX.Enemy.DynamicST + "    ";
			attributesStr += "MA: " + worldEnemyDX.Enemy.DynamicMA + "    ";
			attributesStr += "EN: " + worldEnemyDX.Enemy.DynamicEN + "\n";
			attributesStr += "AG: " + worldEnemyDX.Enemy.DynamicAG + "    ";
			attributesStr += "LU: " + worldEnemyDX.Enemy.DynamicLU;

			this.resourcesDebugLabel.Text = resourcesStr;
			this.attributesDebugLabel.Text = attributesStr;

			this.difficultyLabel.Text = GameController.Instance.DifficultyToggles.difficultyType.ToString();
		}
		#endregion

		#region EVENT SYSTEMS INTERFACE IMPLEMENTATIONS
		public void OnSelect(BaseEventData eventData) {
			// If selected, turn on the visuals object.
			this.allCursorVisualsRectTransform.gameObject.SetActive(true);
		}
		public void OnCancel(BaseEventData eventData) {
			BattleMenuControllerDX.instance.CancelCombatantSelection();
			AudioController.instance?.PlaySFX(SFXType.Close);
			// BattleMenuControllerDX.Instance.TriggerEvent("Cancel Enemy Selection");
		}
		public void OnSubmit(BaseEventData eventData) {
			// Null out the currently selected game object. Even when I set the selectable to not be turned on, sometimes it still. does accept events.
			EventSystem.current.SetSelectedGameObject(null);
			// Upon submitting, tell the battle menu controller that this was the enemy destined to be picked.
			BattleMenuControllerDX.instance.SetCurrentTargetCombatants(combatants: new List<Combatant>() { this.worldEnemyDX.Enemy });
		}
		public void OnDeselect(BaseEventData eventData) {
			// If deselected, turn off the visuals object.
			this.allCursorVisualsRectTransform.gameObject.SetActive(false);
		}
		public void OnMove(AxisEventData eventData) {

			// If moving, do a quick double check on what's selected.
			if (eventData.moveDir != MoveDirection.None && this.selectable.FindSelectable(dir: eventData.moveVector)?.GetComponent<EnemyCursorDX>() == null) {
				
				// Wait a split second for the event system to register the new selected object, if one exists.
				GameController.Instance.WaitThenRun(timeToWait: 0.01f, delegate {
					
					// If the current selected object STILL isnt this, send an error. This is more for debugging purposes. Hopefully I'll remove this.
					if (EventSystem.current.currentSelectedGameObject != this.selectable.gameObject) {
						Debug.LogError("IF THIS MESSAGE IS BEING SHOWN IT MEANS THAT A MOVEMENT WAS TO A GAMEOBJECT THAT SHOULDNT RECEIVE IT." +
					"RESELECTING THE OLD CURSOR. ALERT ME (DISCORD USER GRAWLY) ABOUT THIS. BUT CONTINUE TO PLAY.");
						EventSystem.current.SetSelectedGameObject(this.selectable.gameObject);
					}

					
				});
				
			}

		}
		#endregion

		/// <summary>
		/// This new cursor is stylized as a diamond pattern with each diamond having a specific usage.
		/// This type allows me to define what diamond is assigned to what.
		/// </summary>
		private enum EnemyCursorDiamondType {
			MeterDiamond = 0,			// The diamond that shows the health meter.
			ResistanceDiamond = 1,		// The diamond that shows the resistance.
		}

	}


}