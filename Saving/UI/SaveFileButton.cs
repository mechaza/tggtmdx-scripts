using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using Grawly.Battle;
using Grawly.Calendar;

namespace Grawly.UI {
	/// <summary>
	/// A representation of the save files on the save/load screen.
	/// </summary>
	public class SaveFileButton : MonoBehaviour {
		#region FIELDS - STATE
		/// <summary>
		/// The game save that this button is representing.
		/// </summary>
		private GameSave gameSave;
		/// <summary>
		/// The slot number that this save file button is associated with.
		/// </summary>
		private int slotNumber = -1;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// This is the object that should be turned on/off to COMPLETELY hide the save slot. Good for slots that... don't exist.
		/// </summary>
		[SerializeField, TabGroup("Saving", "Scene References")]
		private GameObject allGraphicsObject;
		/// <summary>
		/// The object that can be turned on/off to show/hide the rest of the things that are its children. Good for if I want that NEW GAME option.
		/// </summary>
		[SerializeField, TabGroup("Saving", "Scene References")]
		private GameObject filledSaveFileGraphicsObject;
		/// <summary>
		/// The object that contains the graphics for showing NO SAVE.
		/// </summary>
		[SerializeField, TabGroup("Saving", "Scene References")]
		private GameObject noSaveGraphicsObject;
		/// <summary>
		/// The white box that serves as the backing for this button.
		/// </summary>
		[SerializeField, TabGroup("Saving", "Scene References")]
		private Image buttonBacking;
		/// <summary>
		/// The image that should show up when this button is highlighted.
		/// </summary>
		[SerializeField, TabGroup("Saving", "Scene References")]
		private Image buttonHighlight;
		/// <summary>
		/// The text mesh that has the number of what save slot this is.
		/// </summary>
		[SerializeField, TabGroup("Saving", "Scene References")]
		private SuperTextMesh saveSlotNumberLabel;
		/// <summary>
		/// The text mesh that has the date this save happened on.
		/// </summary>
		[SerializeField, TabGroup("Saving", "Scene References")]
		private SuperTextMesh dateLabel;
		/// <summary>
		/// The text mesh that has the day of week this save happened on.
		/// </summary>
		[SerializeField, TabGroup("Saving", "Scene References")]
		private SuperTextMesh dayOfWeekLabel;
		/// <summary>
		/// The text mesh that has the place/time where the save happened.
		/// </summary>
		[SerializeField, TabGroup("Saving", "Scene References")]
		private SuperTextMesh placeTimeLabel;
		/// <summary>
		/// The image behind the difficulty label, which can be adjusted by color to show different difficulties.
		/// </summary>
		[SerializeField, TabGroup("Saving", "Scene References")]
		private Image difficultyImage;
		/// <summary>
		/// The text describing the difficulty itself.
		/// </summary>
		[SerializeField, TabGroup("Saving", "Scene References")]
		private SuperTextMesh difficultyLabel;
		/// <summary>
		/// The label that shows what the playtime is.
		/// </summary>
		[SerializeField, TabGroup("Saving", "Scene References")]
		private SuperTextMesh playTimeLabel;
		/// <summary>
		/// The label that shows what the average player level is.
		/// </summary>
		[SerializeField, TabGroup("Saving", "Scene References")]
		private SuperTextMesh averageLevelLabel;
		/// <summary>
		/// The label that shows when no save is stored in this file.
		/// </summary>
		[SerializeField, TabGroup("Saving", "Scene References")]
		private SuperTextMesh noSaveLabel;
		#endregion

		#region PREPARATION
		/// <summary>
		/// Preps this button for use with a particular save file.
		/// </summary>
		/// <param name="gameSave">The game saves to use for preparation.</param>
		/// <param name="slotNumber">The slot number in those game saves to retrieve.</param>
		public void Prepare(List<GameSave> gameSaves, int slotNumber, CalendarDay calendarDay) {
			// Okay this is pretty fun so here we go.
			// It doesn't actually need the entire list, but the list has important information about how this button should be prepared.
			if (slotNumber < 0 || slotNumber >= gameSaves.Count) {
				// If the slot number is less than zero or exceeds the list count, it shouldn't be present.
				// this.allGraphicsObject.SetActive(false);
				// DOING THIS BC ISSUES WITH CALNEDAR DAY I GUESS...
				this.TurnOff();
				// Just return at this point.
				return;
			}

			// If we're at this point, the button needs building. Do that.
			this.allGraphicsObject.SetActive(true);
			// Give it the information it needs.
			this.gameSave = gameSaves[slotNumber];
			this.slotNumber = slotNumber;
			// For the CalendarDay, get it via asking the calendardata for it.
			// this.calendarDay = calendarData.GetDay(dayOfFocus: this.gameSave.currentDayNumber);

			// Set the Dehighlight graphics by default.
			this.Dehighlight(gameSave: this.gameSave, slotNumber: this.slotNumber, calendarDay: calendarDay);
		}
		/// <summary>
		/// AHHHHHHHH
		/// </summary>
		public void TurnOff() {
			this.allGraphicsObject.SetActive(false);
		}
		#endregion

		#region HIGHLIGHT/DEHIGHLIGHT
		/// <summary>
		/// Adjusts the graphics so that this button looks highlighted.
		/// </summary>
		public void Highlight(GameSave gameSave, int slotNumber, CalendarDay calendarDay) {
			
			Debug.Log("Slot " + slotNumber + ": " + gameSave.saveCount + " Saves, " + gameSave.loadCount + " Loads.");
			
			this.SetSTMMaterials(material: DataController.GetDefaultSTMMaterial("UI Dropshadow 2"));

			// Crossfade the button backing/highlight so their colors are correct.
			this.buttonBacking.CrossFadeColor(targetColor: GrawlyColors.colorDict[GrawlyColorTypes.Purple], duration: 0f, ignoreTimeScale: true, useAlpha: true);
			// this.buttonHighlight.CrossFadeAlpha(alpha: 1f, duration: 0f, ignoreTimeScale: true);
			this.buttonHighlight.gameObject.SetActive(true);

			// If there is NO SAVE, set the appropriate graphics.
			if (gameSave.newGame == true) {
				this.noSaveGraphicsObject.SetActive(true);
				this.filledSaveFileGraphicsObject.SetActive(false);
				this.noSaveLabel.Text = "<c=white>" + "NO SAVE";
				this.saveSlotNumberLabel.Text = "<c=weekday>" + (slotNumber + 1).ToString();
				return;
			} else {
				// if there IS, swap that shit.
				this.noSaveGraphicsObject.SetActive(false);
				this.filledSaveFileGraphicsObject.SetActive(true);
			}

			// Set the labels to their correct colors.
			this.saveSlotNumberLabel.Text = "<c=softy>" + (slotNumber + 1).ToString();
			this.dateLabel.Text = "<c=white>" + calendarDay.MonthNumber + " / " + calendarDay.DayNumber;
			this.dayOfWeekLabel.Text = "<c=white>" + calendarDay.WeekdayType.ToString();
			this.placeTimeLabel.Text = "<c=white>" + gameSave.locationName + " : " + gameSave.currentTimeOfDay.ToString();
			this.difficultyImage.color = Color.black;
			// this.difficultyLabel.Text = "<c=white>" + gameSave.difficultyType.ToString();
			this.difficultyLabel.Text = "<c=" + this.GetDifficultyColorHex(gameSave.difficultyType) + ">" + gameSave.difficultyType.ToString();
			// this.playTimeLabel.Text = "<c=white>" + "0h 0m";
			this.playTimeLabel.Text = "<c=white>" + gameSave.PlayTimeString;
			this.averageLevelLabel.Text = "<c=white>" + (gameSave.players.Sum(p => p.Level) / 4).ToString();
		}
		/// <summary>
		/// Adjusts the graphics so that this button looks dehighlighted.
		/// </summary>
		public void Dehighlight(GameSave gameSave, int slotNumber, CalendarDay calendarDay) {
			
			this.SetSTMMaterials(material: DataController.GetDefaultSTMMaterial("UIDefault"));

			// Crossfade the button backing/highlight so their colors are correct.
			this.buttonBacking.CrossFadeColor(targetColor: Color.white, duration: 0f, ignoreTimeScale: true, useAlpha: true);
			this.buttonHighlight.gameObject.SetActive(false);

			// If there is NO SAVE, set the appropriate graphics.
			if (gameSave.newGame == true) {
				this.noSaveGraphicsObject.SetActive(true);
				this.filledSaveFileGraphicsObject.SetActive(false);
				this.noSaveLabel.Text = "<c=black>" + "NO SAVE";
				this.saveSlotNumberLabel.Text = "<c=weekday>" + (slotNumber + 1).ToString();
				return;
			} else {
				// if there IS, swap that shit.
				this.noSaveGraphicsObject.SetActive(false);
				this.filledSaveFileGraphicsObject.SetActive(true);
			}
			
			// Set the labels to their correct colors.
			this.saveSlotNumberLabel.Text = "<c=weekday>" + (slotNumber + 1).ToString();
			// Debug.Log("Breaking here because I can't figure out why the calnendar is bugging.");
			// Debug.Break();
			this.dateLabel.Text = "<c=black>" + calendarDay.MonthNumber + " / " + calendarDay.DayNumber;
			this.dayOfWeekLabel.Text = "<c=black>" + calendarDay.WeekdayType.ToString();
			this.placeTimeLabel.Text = "<c=black>" + gameSave.locationName + " : " + gameSave.currentTimeOfDay.ToString();
			// this.difficultyImage.CrossFadeColor(targetColor: this.GetDifficultyColor(gameSave.difficultyType), duration: 0f, ignoreTimeScale: true, useAlpha: true);
			this.difficultyImage.color = Color.white;
			this.difficultyLabel.Text = "<c=black>" + gameSave.difficultyType.ToString();
			// this.playTimeLabel.Text = "<c=black>" + "0h 0m";
			this.playTimeLabel.Text = "<c=black>" + gameSave.PlayTimeString;
			this.averageLevelLabel.Text = "<c=black>" + (gameSave.players.Sum(p => p.Level) / 4).ToString();
		}
		/// <summary>
		/// Sets the material on the different labels.
		/// </summary>
		/// <param name="material"></param>
		private void SetSTMMaterials(Material material) {
			this.noSaveLabel.textMaterial = material;
			this.saveSlotNumberLabel.textMaterial = material;
			this.dateLabel.textMaterial = material;
			this.dayOfWeekLabel.textMaterial = material;
			this.placeTimeLabel.textMaterial = material;
			this.difficultyLabel.textMaterial = material;
			this.playTimeLabel.textMaterial = material;
			this.averageLevelLabel.textMaterial = material;
		}
		#endregion

		#region HELPERS
		/// <summary>
		/// Returns the color associated with the given difficulty.
		/// </summary>
		/// <param name="difficultyType"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		private string GetDifficultyColorHex(DifficultyType difficultyType) {
			switch (difficultyType) {
				case DifficultyType.VeryEasy:
					return GrawlyColors.ToHexFromRGB(GrawlyColors.colorDict[GrawlyColorTypes.Green]);
				case DifficultyType.Easy:
					return GrawlyColors.ToHexFromRGB(GrawlyColors.colorDict[GrawlyColorTypes.Green]);
				case DifficultyType.Normal:
					return GrawlyColors.ToHexFromRGB(GrawlyColors.colorDict[GrawlyColorTypes.Blue]);
				case DifficultyType.Hard:
					return GrawlyColors.ToHexFromRGB(GrawlyColors.colorDict[GrawlyColorTypes.Red]);
				case DifficultyType.VeryHard:
					return GrawlyColors.ToHexFromRGB(GrawlyColors.colorDict[GrawlyColorTypes.DarkRed]);
				default:
					Debug.LogError("Invalid difficulty type!");
					return GrawlyColors.ToHexFromRGB(GrawlyColors.colorDict[GrawlyColorTypes.Purple]);
			}
		}
		/// <summary>
		/// Gets the color with the associated difficulty type.
		/// </summary>
		/// <param name="difficultyType"></param>
		/// <returns></returns>
		private Color GetDifficultyColor(DifficultyType difficultyType) {
			switch (difficultyType) {
				case DifficultyType.VeryEasy:
					return GrawlyColors.colorDict[GrawlyColorTypes.Green];
				case DifficultyType.Easy:
					return GrawlyColors.colorDict[GrawlyColorTypes.Green];
				case DifficultyType.Normal:
					return GrawlyColors.colorDict[GrawlyColorTypes.Blue];
				case DifficultyType.Hard:
					return GrawlyColors.colorDict[GrawlyColorTypes.Red];
				case DifficultyType.VeryHard:
					return GrawlyColors.colorDict[GrawlyColorTypes.DarkRed];
				default:
					Debug.LogError("Invalid difficulty type!");
					return GrawlyColors.colorDict[GrawlyColorTypes.Green];
			}
		}
		#endregion
	}
}