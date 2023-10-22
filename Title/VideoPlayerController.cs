using System;
using System.Collections;
using System.Collections.Generic;
using Grawly.UI.Legacy;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using Grawly.Calendar;

namespace Grawly.Title {
	
	/// <summary>
	/// The script that should be used for the intro controller.
	/// </summary>
	[RequireComponent(typeof(VideoPlayer))]
	public class VideoPlayerController : MonoBehaviour {

		#region FIELDS - STATE
		/// <summary>
		/// The current state of the skip status.
		/// </summary>
		private SkipStatusType currentSkipStatus { get; set; } = SkipStatusType.Idle;
		/// <summary>
		/// The current amount of seconds that have passed in the Alert skip status.
		/// </summary>
		private float currentSkipTimer { get; set; } = 0f;
		/// <summary>
		/// Is this scene transitioning to the title?
		/// </summary>
		private bool transitioning { get; set; } = false;
		#endregion

		#region FIELDS - TOGGLES
		/// <summary>
		/// The amount of time to take when fading out audio.
		/// </summary>
		[Title("Toggles")]
		[SerializeField]
		private float audioFadeTime = 0.5f;
		/// <summary>
		/// Should the player be prompted to skip?
		/// </summary>
		[SerializeField]
		private bool promptForSkip = true;
		/// <summary>
		/// The amount of time to wait before resetting the prompt flag.
		/// </summary>
		[SerializeField, ShowIf("promptForSkip")]
		private float promptWaitTime = 3f;
		/// <summary>
		/// The kind of behavior that should occur upon the video's completion.
		/// </summary>
		[SerializeField]
		private VideoCompleteEventType eventOnComplete = VideoCompleteEventType.UnityEvent;
		/// <summary>
		/// The scene to load upon completion of this video.
		/// </summary>
		[SerializeField, ShowIf("IsLoadSceneOnComplete")]
		private string sceneToLoad = "";
		/// <summary>
		/// The action to perform when this video is complete.
		/// </summary>
		[SerializeField, ShowIf("IsUnityEventOnComplete")]
		private UnityAction onComplete;
		#endregion
		
		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The text mesh that confirms whether or not the player wants to skip the cutscene.
		/// </summary>
		[Title("Scene References")]
		[SerializeField, ShowIf("promptForSkip")]
		private SuperTextMesh promptLabel;
		/// <summary>
		/// The video player.
		/// </summary>
		private VideoPlayer videoPlayer;
		/// <summary>
		/// The audio source that handles the output for the video.
		/// </summary>
		private AudioSource outputAudioSource;
		#endregion

		#region UNITY CALLS
		private void Awake() {
			this.promptLabel?.gameObject.SetActive(false);
			this.videoPlayer = this.GetComponent<VideoPlayer>();
		}
		private void Start() {
			
			// Find the audio controller and tell the video player to output there.
			this.outputAudioSource = AudioController.instance.GetComponent<AudioSource>();
			this.videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
			this.videoPlayer.SetTargetAudioSource(trackIndex: 0, source: this.outputAudioSource);
			this.outputAudioSource.DOFade(endValue: 1f, duration: 0.1f).SetEase(Ease.Linear);
			
			this.videoPlayer.Play();
			
			// When the loop point is reached, add a callback.
			this.videoPlayer.loopPointReached += this.PlaybackComplete;
		}
		private void Update() {
			
			// If transitoining is set to true, just back out.
			if (transitioning == true) {
				return;
			}

			// If the current status is in alert,
			if (this.currentSkipStatus == SkipStatusType.Alert) {
				// Update the timer.
				this.currentSkipTimer += Time.deltaTime;
				// If it has passed the timeout, reset the current skip status.
				if (this.currentSkipTimer >= this.promptWaitTime) {
					this.currentSkipTimer = 0f;
					this.currentSkipStatus = SkipStatusType.Idle;
					this.promptLabel.gameObject.SetActive(false);
				}
			}
			
			// Check the input controller for input.
			if (InputController.Instance.GetButtonDown("Submit") || InputController.Instance.GetButtonDown("UISubmit")) {
				
				// If input was received, update the skip status.
				this.currentSkipStatus = this.ProcessSkipInput(
					currentSkipStatus: this.currentSkipStatus, 
					promptEnabled: this.promptForSkip);
				
				// If confirmed to skip,
				if (this.currentSkipStatus == SkipStatusType.Confirmed) {
					// Set the transitioning flag and begin playback interruption.
					this.transitioning = true;
					this.promptLabel?.gameObject.SetActive(false);
					this.InterruptPlayback();
					
					
				} else if (this.currentSkipStatus == SkipStatusType.Alert) {
					// If set to alert, turn on the prompt.
					this.promptLabel.gameObject.SetActive(true);
				}
			}
			
			/*// Check the input controller for input.
			if (InputController.Instance.GetButtonDown("Submit") || InputController.Instance.GetButtonDown("UISubmit")) {
				this.transitioning = true;
				this.InterruptPlayback();
			}*/
			
		}
		#endregion

		#region SKIP CONTROLS
		/// <summary>
		/// Gets called when the user presses the skip button.
		/// </summary>
		/// <param name="currentSkipStatus">The current skip status.</param>
		/// <param name="promptEnabled">Is the player going to be prompted to skip?</param>
		/// <returns>The new skip status.</returns>
		private SkipStatusType ProcessSkipInput(SkipStatusType currentSkipStatus, bool promptEnabled) {

			// If the prompt isn't enabled, just return confirmed.
			if (promptEnabled == false) {
				return SkipStatusType.Confirmed;
			}
			
			// Depending on the current skip status, return the "next" one.
			switch (currentSkipStatus) {
				
				case SkipStatusType.Idle:
					return SkipStatusType.Alert;
	
				case SkipStatusType.Alert:
					return SkipStatusType.Confirmed;
				
				case SkipStatusType.Confirmed:
					// If already confirmed, just return confirmed again.
					return SkipStatusType.Confirmed;
				default:
					throw new System.Exception("Couldn't figure out what skip status to return!");
			}
			
		}
		#endregion
		
		#region PLAYBACK
		private void InterruptPlayback() {
			// Remove the callback just in case the video finishes before its done.
			this.videoPlayer.loopPointReached -= this.PlaybackComplete;
			GameController.Instance.StartCoroutine(this.InterruptPlaybackRoutine());
		}
		private IEnumerator InterruptPlaybackRoutine() {
			
			// Fade the screen.
			Flasher.FadeOut();
			
			// Quiet the audio.
			this.outputAudioSource.DOFade(endValue: 0f, duration: this.audioFadeTime).SetEase(Ease.Linear);
			
			// Wait.
			yield return new WaitForSeconds(this.audioFadeTime + 0.1f);
			
			// When interrupting the playback, remove the callback and call the function manually.
			this.videoPlayer.Stop();
			
			// this.PlaybackComplete(this.videoPlayer);	
			
			yield return new WaitForSeconds(.5f);
			
			if (this.eventOnComplete == VideoCompleteEventType.LoadScene) {
				// Load the scene at the second index, then fade back in.
				SceneController.instance.BasicLoadScene(sceneName: this.sceneToLoad, onComplete: delegate {
					Flasher.FadeIn();
				});
			} else if (this.eventOnComplete == VideoCompleteEventType.UnityEvent) {
				this.onComplete.Invoke();
			} else if (this.eventOnComplete == VideoCompleteEventType.NextStoryBeat) {
				CalendarController.Instance.GoToNextStoryBeat(gameVariables: GameController.Instance.Variables);
			} else {
				throw new System.Exception("Couldn't figure out how to proceed after completing this video!");
			}
			
			// Load the scene at the second index, then fade back in.
			/*SceneController.instance.BasicLoadScene(sceneName: this.sceneToLoad, onComplete: delegate {
				Flasher.FadeIn();
			});*/
			
		}
		private void PlaybackComplete(UnityEngine.Video.VideoPlayer vp) {
			this.videoPlayer.loopPointReached -= this.PlaybackComplete;
			InputController.Instance.StartCoroutine(this.PlaybackCompleteRoutine());
		}
		private IEnumerator PlaybackCompleteRoutine() {
			// Fade the screen.
			Flasher.FadeOut();
			yield return new WaitForSeconds(.6f);

			if (this.eventOnComplete == VideoCompleteEventType.LoadScene) {
				// Load the scene at the second index, then fade back in.
				SceneController.instance.BasicLoadScene(sceneName: this.sceneToLoad, onComplete: delegate {
					Flasher.FadeIn();
				});
			} else if (this.eventOnComplete == VideoCompleteEventType.UnityEvent) {
				this.onComplete.Invoke();
			} else if (this.eventOnComplete == VideoCompleteEventType.NextStoryBeat) {
				CalendarController.Instance.GoToNextStoryBeat(gameVariables: GameController.Instance.Variables);
			} else {
				throw new System.Exception("Couldn't figure out how to proceed after completing this video!");
			}
			
		
			
		}
		#endregion

		#region ODIN HELPERS
		private bool IsLoadSceneOnComplete() {
			return this.eventOnComplete == VideoCompleteEventType.LoadScene;
		}
		private bool IsUnityEventOnComplete() {
			return this.eventOnComplete == VideoCompleteEventType.UnityEvent;
		}
		#endregion
		
		/// <summary>
		/// The kind of event that should fire when a video is finished.
		/// </summary>
		public enum VideoCompleteEventType {
			None			= 0,
			LoadScene		= 1,
			UnityEvent		= 2,
			NextStoryBeat	= 3,
		}

		/// <summary>
		/// The different states describing the status of skipping a cutscene.
		/// </summary>
		public enum SkipStatusType {
			Idle			= 0,
			Alert			= 1,
			Confirmed		= 2,
		}
		
	}

	
}