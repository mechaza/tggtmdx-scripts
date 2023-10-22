using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;

namespace Grawly.Battle.BattleMenu {

	/// <summary>
	/// Animates the rotating cube behind the top level selections.
	/// </summary>
	public class RotatingMenuSquare : MonoBehaviour {

		public static RotatingMenuSquare instance;

		#region FIELDS - TOGGLES
		/// <summary>
		/// How fast to rotate the square
		/// </summary>
		[SerializeField, TabGroup("Cube", "Toggles")]
		private float rotationSpeed;
		/// <summary>
		/// The time to use for scaling the square in/out.
		/// </summary>
		[SerializeField, TabGroup("Cube", "Toggles")]
		private float scaleTweenTime;
		/// <summary>
		/// The time to use for tweening the color of the cube.
		/// </summary>
		[SerializeField, TabGroup("Cube", "Toggles")]
		private float colorTweenTime;
		/// <summary>
		/// The ease type to use when scaling the cube in.
		/// </summary>
		[SerializeField, TabGroup("Cube", "Toggles")]
		private Ease scaleEaseType;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// All of the objects inside the menu square.
		/// </summary>
		[SerializeField, TabGroup("Cube", "Scene References")]
		private GameObject allObjects;
		/// <summary>
		/// The square itself.
		/// </summary>
		[SerializeField, TabGroup("Cube", "Scene References")]
		private Image squareImage;
		/// <summary>
		/// The text inside the square.
		/// </summary>
		[SerializeField, TabGroup("Cube", "Scene References")]
		private Text squareText;
		#endregion

		#region FIELDS - SCENE REFERENCES RELATED TO THE THINGS ABOVE
		/// <summary>
		/// The rect transform for the square image.
		/// </summary>
		[SerializeField, TabGroup("Cube", "Scene References")]
		private RectTransform squareImageRectTransform;
		/// <summary>
		/// The rect transform for the square image.
		/// </summary>
		[SerializeField, TabGroup("Cube", "Scene References")]
		private RectTransform squareTextRectTransform;
		#endregion

		#region UNITY CALLS
		private void Awake() {
			if (instance == null) {
				instance = this;
				// this.squareImageRectTransform = this.squareImage.GetComponent<RectTransform>();
				// this.squareTextRectTransform = this.squareText.GetComponent<RectTransform>();
			}
		}
		private void Update() {
			if (squareTextRectTransform.gameObject.activeInHierarchy == true) {
				// Rotate the rect transform of the square image.
				this.squareImageRectTransform.Rotate(xAngle: 0f, yAngle: 0f, zAngle: this.rotationSpeed * Time.deltaTime);
				// Set the rotation of the square text to be the inverse of the square parent. I hate this but hopefully it works.
				this.squareTextRectTransform.localEulerAngles = -this.squareImageRectTransform.localRotation.eulerAngles;
			}
		}
		#endregion

		#region SETTERS
		/// <summary>
		/// Sets the string on the text inside the square.
		/// </summary>
		/// <param name="text">The text that should be inside the square.</param>
		public void SetSquareText(string text) {
			// There's some weird issue with how Unity renders text and this not working sometimes.
			// This is the only workaround I could figure out.
			this.squareText.DOKill(complete: true);
			this.squareText.text = text;
			this.squareText.color = new Color(0.001f,0.001f,0.001f, 0.9999f);
			this.squareText.DOColor(endValue: Color.black, duration: 0.01f).SetEase(Ease.Linear);
		}
		/// <summary>
		/// Sets the string on the text inside the square based on what top level selection was picked.
		/// </summary>
		/// <param name="topLevelSelection">The top level selection that was picked.</param>
		public void SetSquareText(BattleMenuDXTopLevelSelectionType topLevelSelection) {
			// Reset the font size on the text. If it needs to be resized, it will happen.
			this.squareText.fontSize = 175;
			switch (topLevelSelection) {
				case BattleMenuDXTopLevelSelectionType.Attack:
					this.squareText.fontSize = 120;
					this.SetSquareText("Attack");
					break;
				case BattleMenuDXTopLevelSelectionType.Analysis:
					this.SetSquareText("");
					// this.TweenSquareColor(color: GrawlyColors.colorDict[GrawlyColorTypes.Purple], time: this.colorTweenTime);
					break;
				case BattleMenuDXTopLevelSelectionType.Item:
					this.SetSquareText("Item");
					break;
				case BattleMenuDXTopLevelSelectionType.Skill:
					this.SetSquareText("Skill");
					break;
				default:
					this.SetSquareText("");
					break;
			}
		}
		#endregion

		#region ANIMATIONS
		/// <summary>
		/// Tweens the square objects in/out based on what the status should be.
		/// </summary>
		/// <param name="status">Whether to show the square or not.</param>
		/// <param name="resetColor">Should the color be reset to the default when tweening in?</param>
		public void TweenSquareStatus(bool status, bool resetColor = false) {

			// If the GameObject containing the visuals is off, turn it back on and make the scale zero. May remove this later. 
			if (this.allObjects.activeInHierarchy == false) {
				Debug.Log("BATTLE: Rotation cube is not active! Setting it active and making its scale zero.");
				this.allObjects.SetActive(true);
				this.allObjects.transform.localScale = Vector3.zero;
			}

			// If resetting the color, turn it burple.
			if (resetColor == true) {
				this.TweenSquareColor(color: GrawlyColors.colorDict[GrawlyColorTypes.Purple], time: 0f);
			}

			// Scaling should be at 1 if status is True. False if othersise.
			this.allObjects.transform.DOScale(endValue: status == true ? 1f : 0f, duration: this.scaleTweenTime).SetEase(ease: this.scaleEaseType);
		}
		/// <summary>
		/// Tweens the color of the square to the specified color over time.
		/// </summary>
		/// <param name="color">The color to tween to.</param>
		/// <param name="time">The amount of time to tween.</param>
		public void TweenSquareColor(Color color, float time) {
			this.squareImage.DOColor(endValue: color, duration: time);
		}
		/// <summary>
		/// Tweens the color of the square to the color associated with the specified top level selection.
		/// </summary>
		/// <param name="topLevelSelection">The top level selection that is associated with the color I want to tween to.</param>
		public void TweenSquareColor(BattleMenuDXTopLevelSelectionType topLevelSelection) {
			switch (topLevelSelection) {
				case BattleMenuDXTopLevelSelectionType.Attack:
					this.TweenSquareColor(color: GrawlyColors.colorDict[GrawlyColorTypes.Purple], time: this.colorTweenTime);
					break;
				case BattleMenuDXTopLevelSelectionType.Analysis:
					this.TweenSquareColor(color: GrawlyColors.colorDict[GrawlyColorTypes.Red], time: this.colorTweenTime);
					break;
				case BattleMenuDXTopLevelSelectionType.Item:
					this.TweenSquareColor(color: GrawlyColors.colorDict[GrawlyColorTypes.Yellow], time: this.colorTweenTime);
					break;
				case BattleMenuDXTopLevelSelectionType.Skill:
					this.TweenSquareColor(color: GrawlyColors.colorDict[GrawlyColorTypes.Blue], time: this.colorTweenTime);
					break;
				default:
					this.TweenSquareColor(color: GrawlyColors.colorDict[GrawlyColorTypes.Purple], time: this.colorTweenTime);
					break;
			}
		}
		#endregion

	}


}