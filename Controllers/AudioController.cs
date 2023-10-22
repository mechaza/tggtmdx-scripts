using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly {
	public class AudioController : MonoBehaviour {

		public static AudioController instance;

		#region FIELDS - STATE : GENERAL
		/// <summary>
		/// The current lock set on music playback, which if turned on, ignores requests for control.
		/// </summary>
		public MusicPlaybackLockType CurrentPlaybackLockType { get; private set; } = MusicPlaybackLockType.Unlocked;
		#endregion
		
		#region FIELDS - STATE - VOLUME
		/// <summary>
		/// The amount to multiply the music volume scale by.
		/// This is set via a GameToggle.
		/// </summary>
		private float musicVolumeMultiplier = 1f;
		/// <summary>
		/// The amount to multiply the sfx volume scale by.
		/// This is set via a GameToggle.
		/// </summary>
		private float sfxVolumeMultiplier = 1f;
		#endregion
		
		#region FIELDS - PLAYERS
		/// <summary>
		/// The audio source for sound effects.
		/// </summary>
		[SerializeField]
		private AudioSource sfxSource;
		/// <summary>
		/// Contains the different players used to control music.
		/// </summary>
		[SerializeField]
		private List<IntroloopPlayer> introLoopPlayers = new List<IntroloopPlayer>();
		#endregion

		#region UNITY CALLS
		private void Awake() {
			if (instance == null) {
				instance = this;
			}
		}
		#endregion

		#region SETTERS
		/// <summary>
		/// Sets the lock state on the music playback lock.
		/// If locked, calls to manipulate music playback are ignored.
		/// </summary>
		/// <param name="playbackLockType"></param>
		public void SetMusicPlaybackLock(MusicPlaybackLockType playbackLockType) {
			Debug.Log("Setting MusicPlaybackLockType to " + playbackLockType.ToString());
			this.CurrentPlaybackLockType = playbackLockType;
		}
		/// <summary>
		/// Sets the volume of the music.
		/// </summary>
		/// <param name="volume">The volume of the music. Should be between 0 and 1.</param>
		public void SetMusicVolume(float volume) {
			
			// this.introLoopPlayers.ForEach(ip => ip.)
		}
		/// <summary>
		/// Sets the volume on the SFX source.
		/// Mostly doing this from a GameToggle.
		/// </summary>
		/// <param name="volume">The volume to set the SFX source to.</param>
		public void SetSFXVolume(float volume) {
			this.sfxSource.volume = volume;
		}
		#endregion

		#region PLAYSFX
		/// <summary>
		/// Plays the specified sound effect.
		/// </summary>
		public void PlaySFX(SFXType type, float scale = 1.0f) {
			if (DataController.Instance == null) {
				Debug.LogWarning("DataController is not present. Using default SFX.");
				this.PlaySFX(sfx: DataController.GetDefaultSFX(type: type), scale: scale);
			} else {
				PlaySFX(DataController.Instance.GetSFX(type), scale);
			}

		}
		/// <summary>
		/// Plays the specified sound effect.
		/// </summary>
		public void PlaySFX(AudioClip sfx, float scale = 1.0f) {
			sfxSource.PlayOneShot(sfx, scale);
			// sfxSource.PlayOneShot(sfx, scale);
		}
		/// <summary>
		/// Plays the specified sound effect.
		/// </summary>
		public void PlaySFX(string sfx, float scale = 1.0f) {
			// When given a string, use that to load the appropriate sfx from the data controller.
			PlaySFX(DataController.Instance.GetSFX(sfx), scale);
		}
		#endregion

		#region PLAY MUSIC
		/// <summary>
		/// Plays music in the background.
		/// </summary>
		/// <param name="type">The type of music to play.</param>
		/// <param name="track">The track to play the music on.</param>
		public void PlayMusic(MusicType type, int track = 0) {
			// Call the version of this function that also takes a type.
			this.PlayMusic(type: type, track: track, fade: 0f, delay: 0f);
		}
		/// <summary>
		/// Play music in the background.
		/// </summary>
		/// <param name="type">The type of music to play.</param>
		/// <param name="track">The track to play this music on.</param>
		/// <param name="fade">How long to fade in the track by.</param>
		/// <param name="delay">How long to delay playing the track by.</param>
		public void PlayMusic(MusicType type, int track, float fade, float delay) {
			// Load up the audio of the given type and use the standard PlayMusic function.
			IntroloopAudio audio = DataController.Instance.GetMusic(type);
			PlayMusic(audio: audio, track: track, fade: fade, delay: delay);
		}
		/// <summary>
		/// Plays music in the background.
		/// </summary>
		/// <param name="audio"></param>
		/// <param name="track"></param>
		public void PlayMusic(IntroloopAudio audio, int track = 0) {
			this.PlayMusic(audio: audio, track: track, fade: 0f, delay: 0f);
		}
		/// <summary>
		/// Plays music in the background.
		/// </summary>
		/// <param name="audio">The introloopaudio to use.</param>
		/// <param name="track">The track this audio should go on.</param>
		/// <param name="fade">How long the song should take to fade in.</param>
		/// <param name="delay">The delay to get the song going.</param>
		public void PlayMusic(IntroloopAudio audio, int track, float fade, float delay) {

			if (this.CurrentPlaybackLockType == MusicPlaybackLockType.Locked) {
				Debug.Log("Playback Lock on AudioController is set to Locked. Ignoring request.");
				return;
			}
			
			StartCoroutine(PlayMusicRoutine(audio, track, fade, delay));
		}
		/// <summary>
		/// Pauses the music on the given track.
		/// </summary>
		public void PauseMusic(int track, float fade) {
			
			if (this.CurrentPlaybackLockType == MusicPlaybackLockType.Locked) {
				Debug.Log("Playback Lock on AudioController is set to Locked. Ignoring request.");
				return;
			}
			
			introLoopPlayers[track].PauseFade(fade);
		}
		/// <summary>
		/// Resumes the music on the given track.
		/// </summary>
		public void ResumeMusic(int track, float fade) {
			
			if (this.CurrentPlaybackLockType == MusicPlaybackLockType.Locked) {
				Debug.Log("Playback Lock on AudioController is set to Locked. Ignoring request.");
				return;
			}
			
			introLoopPlayers[track].ResumeFade(fade);
		}
		/// <summary>
		/// Stops the music on the given track.
		/// </summary>
		public void StopMusic(int track, float fade) {
			
			if (this.CurrentPlaybackLockType == MusicPlaybackLockType.Locked) {
				Debug.Log("Playback Lock on AudioController is set to Locked. Ignoring request.");
				return;
			}
			
			introLoopPlayers[track].StopFade(fade);
		}
		/// <summary>
		/// Coroutine meant to help get PlayMusic working properly.
		/// </summary>
		private IEnumerator PlayMusicRoutine(IntroloopAudio audio, int track, float fade, float delay) {
			audio.Volume = 0.6f;
			yield return new WaitForSeconds(delay);
			introLoopPlayers[track].PlayFade(audio, fade);
		}
		#endregion
		
	}
}
