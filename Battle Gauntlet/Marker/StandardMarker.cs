using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Grawly.Battle;
using Grawly.Gauntlet.Nodes;
using DG.Tweening;

namespace Grawly.Gauntlet {

	/// <summary>
	/// A regular ass marker. Doesn't do anything too special.
	/// </summary>
	public class StandardMarker : GauntletMarker {

		#region FIELDS - INITIAL STATE
		/// <summary>
		/// The initial position of the marker game object.
		/// Handy for rapid debugging of the animation.
		/// </summary>
		private Vector3 initialMarkerPosition;
		/// <summary>
		/// The initial rotation of the marker gameobject.
		/// Handy for rapid debugging of the animation.
		/// </summary>
		private Vector3 initialMarkerRotation;
		#endregion

		#region FIELDS - TOGGLES
		/// <summary>
		/// The amount of time to take when rotating the marker game object.
		/// </summary>
		[SerializeField, TabGroup("Avatar", "Toggles")]
		private float markerRotationTime = 1f;
		/// <summary>
		/// The ease type to use when rotating the marker.
		/// </summary>
		[SerializeField, TabGroup("Avatar", "Toggles")]
		private Ease markerRotationEaseType = Ease.InOutBack;
		/// <summary>
		/// The amount of time to take when bobbing the marker game object.
		/// </summary>
		[SerializeField, TabGroup("Avatar", "Toggles")]
		private float markerBobTime = 1f;
		/// <summary>
		/// The magnitude of the marker's bob animation.
		/// </summary>
		[SerializeField, TabGroup("Avatar", "Toggles")]
		private float markerBobMagnitude = 1f;
		/// <summary>
		/// The ease type to use when bobbing the marker.
		/// </summary>
		[SerializeField, TabGroup("Avatar", "Toggles")]
		private Ease markerBobEaseType = Ease.InOutBack;
		#endregion

		#region FIELDS - SCENE REFERENCES
		/// <summary>
		/// Contains all of the visuals for the player avatar.
		/// </summary>
		[SerializeField, TabGroup("Avatar", "Scene References")]
		private GameObject allAvatarVisuals;
		/// <summary>
		/// The GameObject that has the marker on it.
		/// </summary>
		[SerializeField, TabGroup("Avatar", "Scene References")]
		private GameObject markerGameObject;
		#endregion

		#region UNITY CALLS
		protected override void Awake() {
			// The base assigns the Instance.
			base.Awake();
			// Save the position/rotation.
			this.initialMarkerPosition = this.markerGameObject.transform.localPosition;
			this.initialMarkerRotation = this.markerGameObject.transform.localEulerAngles;
		}
		private void Start() {
			// On start, play the bob animation.
			this.PlayBobAnimation();
		}
		#endregion

		#region STATE
		/// <summary>
		/// Just move the marker over to the given node.
		/// </summary>
		/// <param name="node">The node to move to.</param>
		public override void MoveToNode(GauntletNode node) {
			this.transform.position = node.transform.position;
		}
		#endregion

		#region ANIMATIONS
		/// <summary>
		/// Just plays the animation that makes the marker bob up and down on loop.
		/// </summary>
		private void PlayBobAnimation() {
			// Kill any previous tweens.
			this.markerGameObject.transform.DOKill();

			// Reset the position/rotation.
			this.markerGameObject.transform.localPosition = this.initialMarkerPosition;
			this.markerGameObject.transform.localEulerAngles = this.initialMarkerRotation;

			// On start, make the marker rotate and junk forever.
			this.markerGameObject.transform.DOLocalRotate(endValue: new Vector3(x: 0f, y: 360f, z: 0f), duration: 1f, mode: RotateMode.FastBeyond360)
				.SetRelative(isRelative: true)
				.SetEase(ease: this.markerRotationEaseType)
				.SetLoops(loops: -1);


			// Also set the Bob's.
			this.markerGameObject.transform.DOLocalJump(endValue: Vector3.zero, jumpPower: this.markerBobMagnitude, numJumps: 1, duration: this.markerBobTime)
				.SetEase(ease: this.markerBobEaseType)
				.SetLoops(loops: -1, loopType: LoopType.Yoyo);
		}
		#endregion

	}


}