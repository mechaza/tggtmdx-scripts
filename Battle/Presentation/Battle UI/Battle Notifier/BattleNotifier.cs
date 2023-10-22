using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Grawly.Battle.BattleMenu {

	public class BattleNotifier : MonoBehaviour {

		public static BattleNotifier instance;

		/// <summary>
		/// The object I can show/hide when I need to.
		/// </summary>
		[SerializeField]
		private GameObject battleNotifierObject;
		/// <summary>
		/// The text for the notifier.
		/// </summary>
		[SerializeField]
		private SuperTextMesh label;
		/// <summary>
		/// The image for the front of the notifier.
		/// </summary>
		[SerializeField]
		private Image frontBarImage;
		/// <summary>
		/// The image for the back of the notifier.
		/// </summary>
		[SerializeField]
		private Image dropshadowBarImage;
		/// <summary>
		/// The easing to use for the tweening of the bars.
		/// </summary>
		[SerializeField]
		private Ease tweenEaseType;
		/// <summary>
		/// The time to use for the tweening of the bars.
		/// </summary>
		[SerializeField]
		private float tweenTime = 0.1f;

		private void Awake() {
			if (instance == null) {
				instance = this;
				// Turn off the notifier.
				Toggle(false);
			}
		}

		/// <summary>
		/// Set whether the notifier should be seen or not.
		/// </summary>
		/// <param name="status"></param>
		private void Toggle(bool status) {
			if (status == true) {
				this.battleNotifierObject.SetActive(true);
				// label.color = Color.black;
				// label.Rebuild();
				// outerBox.color = Color.white;
				// innerBox.color = Color.black;
			} else {
				this.battleNotifierObject.SetActive(false);
				/*label.color = Color.clear;
				label.Rebuild();
				outerBox.color = Color.clear;
				innerBox.color = Color.clear;*/
			}
			// this.battleNotifierObject.SetActive(status);
		}

		/// <summary>
		/// Display a message on the battle notifier.
		/// </summary>
		/// <param name="text"></param>
		public static void DisplayNotifier(string text, float time, BattleNotifierMessageType messageType = BattleNotifierMessageType.Normal) {
			
			Debug.Log("NOTIFIER: " + text);
			// Stop coroutines on this behavior to ensure that the next notifier doesn't overwrite a new incoming one.
			instance.StopAllCoroutines();
			instance.StartCoroutine(instance.DisplayNotifierRoutine(text, time, messageType));
		}

		public static void DisplayNotifier(string text, BattleNotifierMessageType messageType = BattleNotifierMessageType.Normal) {
			
			Debug.Log("NOTIFIER: " + text);
			instance.Toggle(true);
			instance.StopAllCoroutines();
			instance.label.Text = text;
		}

		/// <summary>
		/// Stops the notifier from showing anything.
		/// </summary>
		public static void Halt() {
			instance.Toggle(false);
			instance.StopAllCoroutines();
		}

		private IEnumerator DisplayNotifierRoutine(string text, float time, BattleNotifierMessageType messageType) {

			// Depending on the message type, I may need to do things like change the color.
			string prependString = "";

			switch (messageType){
				case BattleNotifierMessageType.Normal:
					this.frontBarImage.color = Color.white;
					this.dropshadowBarImage.color = Color.black;
					this.label.textMaterial = DataController.GetDefaultSTMMaterial(materialName: "UIDefault");
					break;
				case BattleNotifierMessageType.BattleBehavior:
					this.frontBarImage.color = Color.white;
					this.dropshadowBarImage.color = Color.black;
					this.label.textMaterial = DataController.GetDefaultSTMMaterial(materialName: "UIDefault");
					break;
				case BattleNotifierMessageType.Harmful:
					this.frontBarImage.color = GrawlyColors.colorDict[GrawlyColorTypes.Red];
					this.dropshadowBarImage.color = Color.white;
					// This also makes the message change color.
					prependString = "<c=white>";
					// Make sure I also change to the dropshadow material.
					this.label.textMaterial = DataController.GetDefaultSTMMaterial(materialName: "UI Dropshadow 2");
					break;
				case BattleNotifierMessageType.Supportive:
					this.frontBarImage.color = GrawlyColors.colorDict[GrawlyColorTypes.Green];
					this.dropshadowBarImage.color = Color.white;
					// This also makes the message change color.
					prependString = "<c=white>";
					// Make sure I also change to the dropshadow material.
					this.label.textMaterial = DataController.GetDefaultSTMMaterial(materialName: "UI Dropshadow 2");
					break;
				default:
					Debug.LogError("Couldn't determine notifier message type!");
					break;
			}

			// Kill any tweens on the images. This is important for when a message might show up before the tween of another has completed.
			this.frontBarImage.GetComponent<RectTransform>().DOKill(complete: true);
			this.dropshadowBarImage.GetComponent<RectTransform>().DOKill(complete: true);

			// Reset the positions of the images.
			this.frontBarImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(x: -20f, y: 0f);
			this.dropshadowBarImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(x: -20f, y: -6f);

			this.Toggle(true);
			this.label.Text = prependString + text;

			// Tween the bar graphics.
			this.frontBarImage.GetComponent<RectTransform>().DOAnchorPosX(endValue: 0f, duration: this.tweenTime, snapping: true).SetEase(ease: this.tweenEaseType);
			this.dropshadowBarImage.GetComponent<RectTransform>().DOAnchorPosX(endValue: 11f, duration: this.tweenTime, snapping: true).SetEase(ease: this.tweenEaseType);

			yield return new WaitForSeconds(time);
			this.Toggle(false);
			this.label.Text = "";
		}

		/// <summary>
		/// The different kinds of notifications the notifier can display.
		/// </summary>
		public enum BattleNotifierMessageType {
			Normal = 0,				// Context agnostic.
			BattleBehavior = 1,		// Usually for when a move is being used.
			Harmful = 2,			// Useful for things like if a combatant got burned and I need to say as much.
			Supportive = 3,			// For when ailments are cured or something.
		}

	}



}