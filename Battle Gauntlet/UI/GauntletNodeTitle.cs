using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Grawly.Battle;
using Grawly.Gauntlet.Nodes;
using DG.Tweening;
using System.Linq;

namespace Grawly.Gauntlet {

	/// <summary>
	/// This is what controls most of the visuals for the node title.
	/// </summary>
	public class GauntletNodeTitle : MonoBehaviour {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The time it should take to tween the visuals in when a node is highlighted.
		/// </summary>
		[SerializeField, TabGroup("Node Title", "Toggles")]
		private float tweenInTime = 0.5f;
		/// <summary>
		/// The ease type to use when tweening the visuals of the title.
		/// </summary>
		[SerializeField, TabGroup("Node Title", "Toggles")]
		private Ease tweenEaseType = Ease.InOutCirc;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// The GameObject that contains all other visuals for this node.
		/// </summary>
		[SerializeField, TabGroup("Node Title", "Scene References")]
		private GameObject allVisualsGameObject;
		/// <summary>
		/// The GameObject that contains the visuals specifically for the metadata.
		/// </summary>
		[SerializeField, TabGroup("Node Title", "Scene References")]
		private GameObject metadataVisualsGameObject;
		/// <summary>
		/// The text mesh that shows the title for this node.
		/// </summary>
		[SerializeField, TabGroup("Node Title", "Scene References")]
		private SuperTextMesh nodeTitleLabel;
		/// <summary>
		/// The text mesh that shows the level for this node.
		/// </summary>
		[SerializeField, TabGroup("Node Title", "Scene References")]
		private SuperTextMesh nodeLevelLabel;
		/// <summary>
		/// The text mesh that shows the enemy count for this node.
		/// </summary>
		[SerializeField, TabGroup("Node Title", "Scene References")]
		private SuperTextMesh enemyCountLabel;
		/// <summary>
		/// The text mesh that shows the status of the node.
		/// </summary>
		[SerializeField, TabGroup("Node Title", "Scene References")]
		private SuperTextMesh completedTextLabel;
		/// <summary>
		/// The Image showing the icon for this node.
		/// </summary>
		[SerializeField, TabGroup("Node Title", "Scene References")]
		private Image nodeIconImage;
		/// <summary>
		/// The image that serves as the front of the backing.
		/// </summary>
		[SerializeField, TabGroup("Node Title", "Scene References")]
		private Image nodeTitleBackingFrontImage;
		/// <summary>
		/// The image that serves as the dropshadow of the backing.
		/// </summary>
		[SerializeField, TabGroup("Node Title", "Scene References")]
		private Image nodeTitleBackingDropshadowImage;
		#endregion

		#region UNITY CALLS
		private void Awake() {
			// Instance = this;
		}
		#endregion

		#region DESIGN
		/// <summary>
		/// Preps the node title to be used from the given gauntlet node.
		/// </summary>
		/// <param name="sourceNode">The node that is being shown for this title thigny.</param>
		public void Prepare(IGauntletNodeTitleUser nodeTitleUser) {

			// Kill any tweens this transform was doing.
			this.transform.DOKill();

			// Reset the scale.
			this.transform.localScale = Vector3.zero;
			this.transform.DOScale(endValue: 1f, duration: this.tweenInTime).SetEase(ease: this.tweenEaseType);

			// Set the metadata visuals to be active only if the source node really wants that.
			this.metadataVisualsGameObject.SetActive(nodeTitleUser.UseMetadataVisuals);

			// Set the visual style on the title visuals.
			this.SetStatusStyle(statusType: nodeTitleUser.NodeStatusType);

			// Set the title text as well. This depends on the color tag being set from the method called above. 
			this.nodeTitleLabel.Text = this.nodeTitleLabel.Text + nodeTitleUser.NodeTitleString;

			this.completedTextLabel.Text = this.completedTextLabel.Text + nodeTitleUser.TertiaryMetadataString;

			// WILL NEED TO CHANGE THIS LATER BUT populate the metadata if it's allowed.
			if (nodeTitleUser.UseMetadataVisuals) {
				/*this.nodeLevelLabel.Text = "<size=42>Lv</size>" + (sourceNode as BattleNode).BattleTemplate.EnemyTemplates.First().Level.ToString();
				this.enemyCountLabel.Text = "<size=42>x</size>" + (sourceNode as BattleNode).BattleTemplate.EnemyTemplates.Count.ToString();*/
				this.nodeLevelLabel.Text = nodeTitleUser.PrimaryMetadataString;
				this.enemyCountLabel.Text = nodeTitleUser.SecondaryMetadataString;
			}

			// Also set the sprite.
			// this.nodeIconImage.sprite = nodeTitleUser.NodeTitleSprite;
			this.nodeIconImage.overrideSprite = nodeTitleUser.NodeTitleSprite;

		}
		/// <summary>
		/// Sets the visuals on the title to represent the status of the node.
		/// </summary>
		/// <param name="statusType">The status of the node.</param>
		private void SetStatusStyle(GauntletNodeStatusType statusType) {

			// Reset the visuals to their defaults.
			this.completedTextLabel.Text = "";
			this.nodeTitleLabel.Text = "";
			this.nodeTitleLabel.textMaterial = DataController.GetDefaultSTMMaterial("UI Default");

			this.nodeTitleBackingFrontImage.color = Color.white; 
			this.nodeTitleBackingDropshadowImage.color = GrawlyColors.colorDict[GrawlyColorTypes.Purple];
			this.nodeIconImage.color = Color.white;

			// this.nodeTitleBackingFrontImage.CrossFadeColor(targetColor: Color.white, duration: 0f, ignoreTimeScale: true, useAlpha: false);
			// this.nodeTitleBackingDropshadowImage.CrossFadeColor(targetColor: GrawlyColors.colorDict[GrawlyColorTypes.Purple], duration: 0f, ignoreTimeScale: true, useAlpha: false);
			// this.nodeIconImage.CrossFadeColor(targetColor: Color.white, duration: 0f, ignoreTimeScale: true, useAlpha: false);

			switch (statusType) {
				case GauntletNodeStatusType.New:
					// this.completedTextLabel.Text = "<c=yellowy><size=32>new";
					break;
				case GauntletNodeStatusType.Normal:
					break;
				case GauntletNodeStatusType.Completed:
					// this.completedTextLabel.Text = "completed";
					this.nodeTitleLabel.textMaterial = DataController.GetDefaultSTMMaterial("UI Dropshadow 2");
					this.nodeTitleLabel.Text = "<c=white>";
					// this.nodeTitleBackingDropshadowImage.CrossFadeColor(targetColor: Color.black, duration: 0f, ignoreTimeScale: true, useAlpha: false);
					// this.nodeTitleBackingFrontImage.CrossFadeColor(targetColor: GrawlyColors.colorDict[GrawlyColorTypes.Purple], duration: 0f, ignoreTimeScale: true, useAlpha: false);
					// this.nodeIconImage.CrossFadeColor(targetColor: Color.gray, duration: 0f, ignoreTimeScale: true, useAlpha: false);
					this.nodeTitleBackingDropshadowImage.color = Color.black;
					this.nodeTitleBackingFrontImage.color = GrawlyColors.colorDict[GrawlyColorTypes.Purple];
					this.nodeIconImage.color = Color.gray;
					break;
				case GauntletNodeStatusType.Locked:
					// this.completedTextLabel.Text = "locked";
					// this.nodeIconImage.CrossFadeColor(targetColor: Color.gray, duration: 0f, ignoreTimeScale: true, useAlpha: false);
					this.nodeIconImage.color = Color.gray;
					break;
				default:
					Debug.LogError("This theoretically should never be reached.");
					break;
			}
		}
		#endregion

		#region ANIMATIONS
		/// <summary>
		/// Sets whether or not the title is visible.
		/// </summary>
		/// <param name="status">Whether or not the title is visible.</param>
		public void SetVisualsActive(bool status) {
			Debug.Log("GAUNTLET: Setting node title visuals to " + status);
			this.allVisualsGameObject.SetActive(status);
		}
		#endregion

	}


}