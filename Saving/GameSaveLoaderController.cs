using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Grawly.Battle;
using System.Linq;
using UnityEngine.UI;
using Grawly.Battle.TurnBehaviors;
using System;

namespace Grawly {
	/// <summary>
	/// This works closely with the save controller and basically just contains things that can't otherwise be serialized.
	/// This should ONLY exist within the LoadGame Scene.
	/// </summary>
	public class GameSaveLoaderController : MonoBehaviour {

		public static GameSaveLoaderController instance;

		#region FIELDS - STATE
		/// <summary>
		/// Gets set by the save controller to load this up next.
		/// </summary>
		public static string sceneToLoad = "";
		/// <summary>
		/// The
		/// </summary>
		public static Action onSceneLoaded;
		#endregion

		#region FIELDS - ASSETS
		[SerializeField]
		private GameVariablesTemplate gameVariablesTemplate;
		private List<PlayerTemplate> playerTemplates;
		private List<PersonaTemplate> personaTemplates;
		private List<BattleBehavior> battleBehaviors;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// Just to make things look more professional than they actually are. I'm a bitch.
		/// </summary>
		[SerializeField]
		private Image blockerImage;
		#endregion

		#region UNITY EVENTS
		private void Awake() {
			instance = this;
		}
		private void OnEnable() {
			// Set the blocker image to true.
			// this.blockerImage.gameObject.SetActive(true);
		}
		#endregion

		#region ASSET HELPERS

		// I think these are relics from when I didn't know what I was doing but it's still good for caching I suppose.

		/// <summary>
		/// Returns the player template associated with the given serializable player.
		/// </summary>
		/// <param name="sp"></param>
		/// <returns></returns>
		public PlayerTemplate GetPlayerTemplate(Player.SerializablePlayer sp) {
			return this.gameVariablesTemplate.playerTemplates.Find(pt => pt.characterIDType == sp.characterIDType);
		}
		/// <summary>
		/// Returns the persona template associated with the given serializable persona.
		/// </summary>
		/// <param name="sp"></param>
		/// <returns></returns>
		public PersonaTemplate GetPersonaTemplate(Persona.SerializablePersona sp) {
			return this.gameVariablesTemplate
				.personaTemplates
				.Concat(this.gameVariablesTemplate.playerTemplates.Select(playerTemplate => playerTemplate.personaTemplate))
				.ToList()
				.Find(pt => pt.metaData.name == sp.metaData.name);
		}
		/// <summary>
		/// Why did i make this
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public BattleBehavior GetBattleBehavior(string str) {
			if (this.battleBehaviors == null) { this.battleBehaviors = DataController.GetAllBattleBehaviors(); }
			return this.battleBehaviors.Find(bb => bb.behaviorName == str);
		}
		#endregion
		
		#region FINISH UP
		/// <summary>
		/// Loads up the scene from the game save.
		/// </summary>
		public void LoadGameSaveScene() {
			
			// Previously, the save controller set the sceneToLoad field.
			// Try loading that.
			SceneController.instance.LoadScene(sceneName: sceneToLoad);
			// Null that Out I suppose.
			sceneToLoad = "";
			onSceneLoaded = null;
			
		}
		#endregion

	}

}