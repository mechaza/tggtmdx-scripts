using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;
using Grawly.Battle;
using System.Linq;

namespace Grawly.UI {

	/// <summary>
	/// The object that shows a player's status on the pause menu.
	/// </summary>
	public class PauseMenuPlayerStatus : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler, ICancelHandler {

		#region FIELDS - STATE
		/// <summary>
		/// The index of this menu list item as a child of its parent in the hierarchy. Primarily for keeping track of which was last highlighted. I hate this.
		/// </summary>
		[SerializeField, TabGroup("Status", "Toggles")]
		private int index = -1;
		/// <summary>
		/// The index of this menu list item as a child of its parent in the hierarchy. Primarily for keeping track of which was last highlighted. I hate this.
		/// </summary>
		public int Index {
			get {
				return this.index;
			}
		}
		/// <summary>
		/// The player assocaited with this status.
		/// </summary>
		public Player player { get; private set; }
		/// <summary>
		/// Whether or not this status is allowed to be submitted.
		/// Will be false if trying to heal a player who is dead, for example.
		/// </summary>
		private bool isSubmitable = true;
		#endregion
	
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The GameObject that contains all the other statuses.
		/// </summary>
		[SerializeField, TabGroup("Status", "Scene References")]
		private GameObject statusObject;
		/// <summary>
		/// The image that serves as the background for the other text and bars and shit.
		/// </summary>
		[SerializeField, TabGroup("Status", "Scene References")]
		private Image statusBackgroundImage;
		/// <summary>
		/// The STM that shows the player's name.
		/// </summary>
		[SerializeField, TabGroup("Status", "Scene References")]
		private SuperTextMesh playerNameLabel;
		/// <summary>
		/// The label that shows the current value of HP.
		/// </summary>
		[SerializeField, TabGroup("Status", "Scene References")]
		private SuperTextMesh healthValueLabel;
		/// <summary>
		/// The label that shows the current value of magic.
		/// </summary>
		[SerializeField, TabGroup("Status", "Scene References")]
		private SuperTextMesh magicValueLabel;
		/// <summary>
		/// The STM that just shows the SLASH between the HP/MP.
		/// </summary>
		[SerializeField, TabGroup("Status", "Scene References")]
		private SuperTextMesh slashLabel;
		/// <summary>
		/// The iamge that serves as the fill for the health bar.
		/// </summary>
		[SerializeField, TabGroup("Status", "Scene References")]
		private Image healthBarFill;
		/// <summary>
		/// The image that serves as the fill for the magic bar.
		/// </summary>
		[SerializeField, TabGroup("Status", "Scene References")]
		private Image magicBarFill;
		/// <summary>
		/// The image that serves as the "fill" for the persona icon.
		/// E.x., the white space around the alpha. The mask.
		/// </summary>
		[SerializeField, TabGroup("Status", "Scene References")]
		private Image personaIconMaskImage;
		/// <summary>
		/// The actual image representing the Persona itself.
		/// </summary>
		[SerializeField, TabGroup("Status", "Scene References")]
		private Image personaIconImage;
		#endregion

		#region BUILDING
		/// <summary>
		/// Builds the thing. Should be the first point of entry.
		/// </summary>
		/// <param name="player"></param>
		public void Build(Player player, bool highlighted) {
			this.player = player;
			// When building, I may want to keep the player's status highlighted.
			// If so, use highlight. If not, just keep it dehighlighted.
			if (highlighted == true) {
				this.Highlight(player: player);
			} else {
				this.Dehighlight(player: player);
			}
			
		}
		#endregion

		#region GRAPHICS
		public void Highlight() {
			this.Highlight(player: this.player);
		}
		public void Dehighlight() {
			this.Dehighlight(player: this.player);
		}
		/// <summary>
		/// its just easier to think about Highlighting when i call it like this.
		/// </summary>
		/// <param name="player"></param>
		private void Highlight(Player player) {
			this.SetSTMMaterials(materialName: "UI Dropshadow 2");
			this.Refresh(colorPrefix: "<c=white>", player: player);
			this.statusBackgroundImage.CrossFadeColor(targetColor: GrawlyColors.colorDict[GrawlyColorTypes.Purple], duration: 0f, ignoreTimeScale: true, useAlpha: true);
			this.personaIconMaskImage.CrossFadeColor(targetColor: GrawlyColors.colorDict[GrawlyColorTypes.Purple], duration: 0f, ignoreTimeScale: true, useAlpha: true);
		}
		/// <summary>
		/// its just easier to think about Highlighting when i call it like this.
		/// </summary>
		/// <param name="player"></param>
		private void Dehighlight(Player player) {
			this.SetSTMMaterials(materialName: "UIDefault");
			this.Refresh(colorPrefix: "<c=black>", player: player);
			this.statusBackgroundImage.CrossFadeColor(targetColor: Color.white, duration: 0f, ignoreTimeScale: true, useAlpha: true);
			this.personaIconMaskImage.CrossFadeColor(targetColor: Color.white, duration: 0f, ignoreTimeScale: true, useAlpha: true);
		}
		/// <summary>
		/// A general "refresh" method that gets called from Highlight/Dehighlight.
		/// </summary>
		/// <param name="player"></param>
		private void Refresh(string colorPrefix, Player player) {

			this.playerNameLabel.Text = colorPrefix + player.metaData.name;

			this.slashLabel.Text = colorPrefix + "/";

			this.healthBarFill.fillAmount = player.HPRatio;
			this.magicBarFill.fillAmount = player.MPRatio;

			// this.personaIconImage.sprite = player.ActivePersona.Icon;
			this.personaIconImage.overrideSprite = player.ActivePersona.Icon;

			// Figure out how many zeroes are needed.
			string hpzeroes = "";
			string mpzeroes = "";
			if (player.HP < 10) { hpzeroes = "00"; } else if (player.HP < 100) { hpzeroes = "0"; }
			if (player.MP < 10) { mpzeroes = "00"; } else if (player.MP < 100) { mpzeroes = "0"; }

			// Adjust the text for the HP and MP accordingly. This sucks.
			this.healthValueLabel.Text = "<c=" + GrawlyColors.ToHexFromRGB(GrawlyColors.colorDict[GrawlyColorTypes.DarkRed]) + ">" + hpzeroes + "</c><c=" +
					GrawlyColors.ToHexFromRGB(GrawlyColors.colorDict[GrawlyColorTypes.Red]) + ">" + player.HP.ToString() + "</c>";
			this.magicValueLabel.Text = "<c=" + GrawlyColors.ToHexFromRGB(GrawlyColors.colorDict[GrawlyColorTypes.DarkGreen]) + ">" + mpzeroes + "</c><c=" +
					GrawlyColors.ToHexFromRGB(GrawlyColors.colorDict[GrawlyColorTypes.Green]) + ">" + player.MP.ToString() + "</c>";
		}
		/// <summary>
		/// A quick and dirty for setting the material on the STM's here.
		/// </summary>
		/// <param name="materialName"></param>
		private void SetSTMMaterials(string materialName) {
			this.playerNameLabel.textMaterial = DataController.GetDefaultSTMMaterial(materialName: materialName);
			// this.healthValueLabel.textMaterial = DataController.GetDefaultSTMMaterial(materialName: materialName);
			// this.magicValueLabel.textMaterial = DataController.GetDefaultSTMMaterial(materialName: materialName);
			// this.slashLabel.textMaterial = DataController.GetDefaultSTMMaterial(materialName: materialName);
		}
		#endregion

		#region STATE SETTERS
		/// <summary>
		/// Sets whether or not this status is submittable based on the behavior passed in.
		/// </summary>
		/// <param name="behavior">Whether or not this behavior is Submittable.</param>
		/// <returns></returns>
		public bool SetSubmittableForBehavior(BattleBehavior behavior) {
			// Trying this out for right now. If the player is dead and trying to heal them, set to false.
			if (this.player.IsDead == true && behavior.elementType == ElementType.Healing) {
				this.isSubmitable = false;
			} else {
				this.isSubmitable = true;
			}
			return this.isSubmitable;
		}
		/// <summary>
		/// Sets whether or not this status is selectable for changing personas.
		/// </summary>
		/// <returns></returns>
		public bool SetSubmittableForPersona() {
			// Only allow submission if the player is not dead and there is more than one avaialble persona to swap out with.
			if (this.player.IsDead == true) {
				this.isSubmitable = false;
			} else {
				this.isSubmitable = true;
			}
			return this.isSubmitable;
		}
		#endregion

		#region UI EVENT TRIGGERS
		public void OnSelect(BaseEventData eventData) {
			AudioController.instance?.PlaySFX(SFXType.Hover);
			PauseMenuController.instance.SetCurrentPlayerSelectionIndex(index: this.Index);
			// On select, also highlight.
			this.Highlight(player: this.player);
			// Not all states will implement a Selected Player transition, but ones like Skill do because they need to preview the player's skills before they are selected.
			PauseMenuController.instance.FSM.SendEvent("Highlighted Player");
		}
		public void OnDeselect(BaseEventData eventData) {
			this.Dehighlight(player: this.player);
		}
		public void OnSubmit(BaseEventData eventData) {
			// If its not possible to submit this player, play the invalid sfx.
			if (this.isSubmitable == false) {
				AudioController.instance?.PlaySFX(SFXType.Invalid);
				return;
			} else if (GameController.Instance.Variables.Personas.Count(p => p.InUse == false) == 0 && PauseMenuController.instance.FSM.ActiveStateName == "Bersona Menu") {
				// SHOEHORNIGN THIS IN TOO.... if there arent any abailable personas.. dont
				AudioController.instance?.PlaySFX(SFXType.Invalid);
				return;
			} else {
				AudioController.instance?.PlaySFX(SFXType.Select);
				PauseMenuController.instance.FSM.SendEvent("Picked Player");
			}
			
			
		}
		public void OnCancel(BaseEventData eventData) {
			AudioController.instance?.PlaySFX(SFXType.Close);
			PauseMenuController.instance.FSM.SendEvent("Back");
		}
		#endregion

	}


}