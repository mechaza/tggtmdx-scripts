using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using Grawly.UI;

namespace Grawly.Battle.BattleMenu  {
	
	/// <summary>
	/// The new way in which the All Out Attack should be animated.
	/// </summary>
	public class AllOutAttackControllerDX : MonoBehaviour {
		
		public static AllOutAttackControllerDX Instance { get; private set; }

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// All of the objects that this controller is visualizing.
		/// </summary>
		[SerializeField, Title("Scene References")]
		private GameObject allObjects;
		/// <summary>
		/// The GameObject that contains the background visuals (see: scrolling diamond pattern)
		/// </summary>
		[SerializeField]
		private GameObject backgroundVisuals;
		/// <summary>
		/// The squares that rotate in the background as part of this animation.
		/// </summary>
		[SerializeField]
		private List<AllOutAttackBackgroundSquare> backgroundSquares = new List<AllOutAttackBackgroundSquare>();
		/// <summary>
		/// The portraits to use for the all out attack animation in the 4-row configuration..
		/// </summary>
		[SerializeField]
		private List<AllOutAttackPortrait> rowFourAttackPortraits = new List<AllOutAttackPortrait>();
		/// <summary>
		/// The portraits to use for the all out attack animation in the 3-row configuration..
		/// </summary>
		[SerializeField]
		private List<AllOutAttackPortrait> rowThreeAttackPortraits = new List<AllOutAttackPortrait>();
		#endregion
		
		#region UNITY CALLS
		private void Awake() {
			if (Instance == null) {
				Instance = this;
				ResetController.AddToDontDestroy(obj: this.gameObject);
			} else {
				Destroy(this.gameObject);
			}
		}
		private void Start() {
			this.ResetState();
		}
		#endregion

		#region PREPARATION
		/// <summary>
		/// Completely and totally reset the state of the controller.
		/// </summary>
		public void ResetState() {
			
			// Go through the different UI elements and reset the states there.
			this.rowFourAttackPortraits.ForEach(p => p.ResetState());
			this.rowThreeAttackPortraits.ForEach(p => p.ResetState());
			
			// Turn the squares off.
			this.backgroundSquares.ForEach(s => s.gameObject.SetActive(false));
			
			// Turn off everything.
			this.allObjects.SetActive(false);
			this.backgroundVisuals.SetActive(false);
			
		}
		#endregion

		#region PRESENTATION
		/// <summary>
		/// Plays the attack animation for the all out attack.
		/// </summary>
		/// <param name="participatingPlayers">The players who are particpating in the all out attack.</param>
		public void PlayAttackAnimation(List<Player> participatingPlayers) {
			// Use the IDs from the players to play the animation.
			this.PlayAttackAnimation(
				participatingPlayerIDs: participatingPlayers.Select(p => p.playerTemplate.characterIDType).ToList());
		}
		/// <summary>
		/// Plays the attack animation for the all out attack.
		/// </summary>
		/// <param name="participatingPlayerIDs"></param>
		private void PlayAttackAnimation(List<CharacterIDType> participatingPlayerIDs) {
			
			// Reset the state.
			this.ResetState();
			
			// Turn all of the objects on.
			this.allObjects.SetActive(true);
			
			// Turn on the background visuals.
			this.backgroundVisuals.SetActive(true);

			// Turn the squares on.
			this.backgroundSquares.ForEach(s => s.gameObject.SetActive(true));
			
			// Get the portraits that should be animated.
			List<AllOutAttackPortrait> portraitsToAnimate = this.GetRequiredPortraits(participatingPlayerIDs: participatingPlayerIDs);

			// Obviously, these values should match.
			Debug.Assert(participatingPlayerIDs.Count == portraitsToAnimate.Count);
			
			// Go through each ID and animate a portrait for it.
			for (int i = 0; i < participatingPlayerIDs.Count; i++) {
				// Get the portrait and ID to use.
				AllOutAttackPortrait targetPortrait = portraitsToAnimate[i];
				CharacterIDType targetCharacterID = participatingPlayerIDs[i];
				// Animate it.
				targetPortrait.AnimatePortrait(characterIDType: targetCharacterID);
			}
			
		}
		#endregion

		#region HELPERS
		/// <summary>
		/// Gets the portraits that will be used for the animation.
		/// </summary>
		/// <param name="participatingPlayerIDs"></param>
		/// <returns></returns>
		private List<AllOutAttackPortrait> GetRequiredPortraits(List<CharacterIDType> participatingPlayerIDs) {
			
			// Get the number of players that are going to be used.
			int playerCount = participatingPlayerIDs.Count;
			
			// Return the correct ones based on that.
			switch (playerCount) {
				case 2:
					return new List<AllOutAttackPortrait>() {
						this.rowFourAttackPortraits[1], this.rowFourAttackPortraits[2]
					};
				case 3:
					return this.rowThreeAttackPortraits;
				case 4:
					return this.rowFourAttackPortraits;
				default:
					throw new System.Exception("Invalid number of players!");
			}
			
		}
		#endregion

		#region ODIN
		/// <summary>
		/// Just a way for me to quickly prototype this.
		/// </summary>
		[Button, HideInEditorMode]
		private void PlayDebugAnimation() {
			this.PlayAttackAnimation(participatingPlayerIDs: new List<CharacterIDType>() {
				CharacterIDType.Dorothy,
				CharacterIDType.Rose,
				CharacterIDType.Sophia,
				CharacterIDType.Blanche
			});
		}
		#endregion
		
	}
}