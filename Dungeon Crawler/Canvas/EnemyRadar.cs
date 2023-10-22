using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace Grawly.DungeonCrawler {
	
	/// <summary>
	/// The class that shows how much danger the player is in.
	/// </summary>
	public class EnemyRadar : MonoBehaviour {
		
		private static EnemyRadar instance { get; set; }
		public static EnemyRadar Instance {
			get {
				if (instance == null) {
					return null;
				} else {
					return instance;
				}
			}
		}

		#region FIELDS - TOGGLES
		/// <summary>
		/// The amount of time to take when scaling the diamond in.
		/// </summary>
		[Title("Toggles")]
		[SerializeField]
		private float scaleTime = 0.5f;
		/// <summary>
		/// The amount of time to take when rotating the radar image.
		/// </summary>
		[SerializeField]
		private float rotateTime = 0.2f;
		/// <summary>
		/// The amount of time to take when fading the color on the diamond image.
		/// </summary>
		[SerializeField]
		private float colorFadeTime = 0.2f;
		/// <summary>
		/// The easing to use when scaling in.
		/// </summary>
		[SerializeField]
		private Ease scaleInEaseType = Ease.InOutQuint;
		/// <summary>
		/// The easing to use when scaling out.
		/// </summary>
		[SerializeField]
		private Ease scaleOutEaseType = Ease.InOutQuint;
		/// <summary>
		/// The easing to use when rotating the diamond.
		/// </summary>
		[SerializeField]
		private Ease rotateEaseType = Ease.InOutBounce;
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// All of the objects contined in this radar.
		/// </summary>
		[Title("Scene References")]
		[SerializeField]
		private GameObject allObjects;
		/// <summary>
		/// The image diamond that rotates and changes color.
		/// </summary>
		[SerializeField]
		private Image diamondImage;
		/// <summary>
		/// The image that changes in response to the radar.
		/// </summary>
		[SerializeField]
		private Image smileyImage;
		#endregion

		#region UNITY CALLS
		private void Awake() {
			instance = this;
		}
		private void Start() {
			// Reset the scale of the all objects.
			this.allObjects.transform.localScale = Vector3.zero;
		}
		#endregion
		
		#region PREPARATION
		/// <summary>
		/// Totally resets the state of the radar.
		/// </summary>
		private void ResetState() {
			
			// Kill the relevant tweens.
			this.diamondImage.DOKill(complete: true);
			this.diamondImage.GetComponent<RectTransform>().DOKill(complete: true);
			this.allObjects.transform.DOKill(complete: true);
			
			// Reset the rotation of the diamond.
			this.diamondImage.GetComponent<RectTransform>().localEulerAngles = Vector3.zero;
			
		}
		#endregion

		#region MAIN CALLS
		/// <summary>
		/// Shows the radar and initializes it with the given danger level type.
		/// </summary>
		/// <param name="dangerLevelType"></param>
		public void Show(DangerLevelType dangerLevelType) {
			
			// Reset the state.
			this.ResetState();

			// Reset the scale of the all objects.
			this.allObjects.transform.localScale = Vector3.zero;
			
			// Display the objects.
			this.allObjects.transform.DOScale(
				endValue: Vector3.one, 
				duration: this.scaleTime)
				.SetEase(this.scaleInEaseType);
			
			// Set the radar's color.
			this.diamondImage.color = this.GetDangerColor(dangerLevelType);
			
		}
		/// <summary>
		/// Dismisses the radar from view.
		/// </summary>
		public void Dismiss() {
			// Kill any potential tweens.
			this.allObjects.transform.DOKill(complete: true);
			// Scale the objects out.
			this.allObjects.transform.DOScale(
					endValue: Vector3.zero, 
					duration: this.scaleTime)
				.SetEase(this.scaleOutEaseType);
			
		}
		/// <summary>
		/// Displays the radar. Helpful for things like the pause menu.
		/// </summary>
		public void Display() {
			// Kill any potential tweens.
			this.allObjects.transform.DOKill(complete: true);
			// Scale the objects out.
			this.allObjects.transform.DOScale(
					endValue: Vector3.one, 
					duration: this.scaleTime)
				.SetEase(this.scaleInEaseType);
		}
		/// <summary>
		/// Sets the danger level of the radar.
		/// </summary>
		/// <param name="dangerLevelType"></param>
		public void SetDanger(DangerLevelType dangerLevelType) {
			
			// Reset the state. This kills any tweens.
			this.ResetState();
			
			// Rotate the diamond.
			this.diamondImage.GetComponent<RectTransform>().DOLocalRotate(
				endValue: new Vector3(x: 0f, y: 0f, z: -90f),
				duration: this.rotateTime, 
				mode: RotateMode.LocalAxisAdd)
				.SetEase(this.rotateEaseType);

			// Also make sure to tween the color appropriately.
			this.diamondImage.DOColor(
				endValue: this.GetDangerColor(dangerLevelType),
				duration: this.colorFadeTime)
				.SetEase(Ease.Linear);
			
		}
		#endregion

		#region HELPERS
		/// <summary>
		/// Gets the color associated with the provided danger level type.
		/// </summary>
		/// <param name="dangerLevelType"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		private Color GetDangerColor(DangerLevelType dangerLevelType) {
			switch (dangerLevelType) {
				case DangerLevelType.None:
					return Color.white;
				case DangerLevelType.Low:
					return GrawlyColors.colorDict[GrawlyColorTypes.Blue];
				case DangerLevelType.Medium:
					return GrawlyColors.colorDict[GrawlyColorTypes.Yellow];
				case DangerLevelType.High:
					return GrawlyColors.colorDict[GrawlyColorTypes.Red];
				case DangerLevelType.Encounter:
					return GrawlyColors.colorDict[GrawlyColorTypes.Red];
				default:
					throw new ArgumentOutOfRangeException(nameof(dangerLevelType), dangerLevelType, null);
			}
		}
		#endregion
		
	}

	
}