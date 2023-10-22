using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Crosstales.RTVoice;

namespace Grawly.Battle.Navigator {

	/// <summary>
	/// The "standard" voice to use.
	/// This basically acts as a hook into RT voice.
	/// </summary>
	[System.Serializable]
	public class TextToSpeechVoice : BattleNavigatorVoice {

		#region FIELDS - RESOURCES
		/// <summary>
		/// The prefab to instansiate which contains the speaker object.
		/// </summary>
		[SerializeField]
		private GameObject voicePrefab;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// Contains the voice for this text to speech module.
		/// </summary>
		private LiveSpeaker voice;
		#endregion

		#region MAIN CALLS
		/// <summary>
		/// Prepares this voice for use by instansiating the voice prefab.
		/// </summary>
		public override void Initialize() {
			// If the voice is null, 
			if (this.voice == null) {
				// Instansiate the prefab and grab a reference to its speaker component.
				GameObject voiceGameObject = GameObject.Instantiate(this.voicePrefab);
				this.voice = voiceGameObject.GetComponent<LiveSpeaker>();
				// Ensure that the GameObject isn't destroyed on load, obviously.
				GameObject.DontDestroyOnLoad(voiceGameObject);
			}
		}
		/// <summary>
		/// Calls upon the RT-Voice plugin to speak a thing!
		/// </summary>
		/// <param name="navigatorParams">The navigator params used to speak up.</param>
		public override void Speak(BattleNavigatorParams navigatorParams) {
			Debug.Log("SPEAKING: " + navigatorParams.dialogueText);
			this.voice.Speak(navigatorParams.dialogueText);
		}
		#endregion

	}

}
