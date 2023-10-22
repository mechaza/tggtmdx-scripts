using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using System.Linq;
using System;
using Sirenix.Serialization;

namespace Grawly {

	/// <summary>
	/// The different types of tweening relations there may be. 
	/// I have a lot of situations where I tween things in/out so this helps.
	/// </summary>
	public enum TweenRelationType {
		In = 0,
		Out = 1,
	}

	/// <summary>
	/// The different kinds of contexts I'll be tweening shit.
	/// </summary>
	public enum TweenContextType {
		Time = 0,
		Ease = 1,
		Position = 2,
		Magnitude = 3,
		Color = 4,
	}

	/// <summary>
	/// I tend to have a lot of situations where I need to tween things in/out
	/// and I'm getting sick of defining a parameter for every single new thing
	/// so it's time to make this shit generic baybee!!
	/// </summary>
	/// <typeparam name="T">The type of object to associate with the in/out relationship</typeparam>
	public struct TweenInOutParams<T> {

		#region PROPERTIES AND STATE
		/// <summary>
		/// Is this part of the struct enabled?
		/// </summary>
		public bool enabled;
		/// <summary>
		/// The generic type parameter being used for this struct.
		/// </summary>
		public Type TypeParameter {
			get {
				return typeof(T);
			}
		}
		#endregion

		#region FIELDS
		/// <summary>
		/// The object to return when tweening in.
		/// </summary>
		[SerializeField, LabelText("$TweenInStringName")]
		private T tweenInObject;
		/// <summary>
		/// The object to return when tweening out.
		/// </summary>
		[SerializeField, LabelText("TweenOutStringName")]
		private T tweenOutObject;
		#endregion

		#region GETTERS
		/// <summary>
		/// Returns the object associated with the given relation type.
		/// </summary>
		/// <param name="relationType">The relation type associated with the desired object.</param>
		/// <returns>The desired object associated with the given relation type.</returns>
		public T this[TweenRelationType relationType] {
			get {
				// If this is not enabled, return the default (which is null, because I have struct types set to be nullable)
				if (this.enabled == false) {
					return default(T);
				} else {
					// If this IS enabled, return either one of the relations.
					return relationType == TweenRelationType.In ? this.tweenInObject : this.tweenOutObject;
				}
				
			}
		}
		#endregion

		#region ODIN HELPERS
		/// <summary>
		/// The string to use for the tween in object.
		/// </summary>
		private string TweenInStringName {
			get {
				return "Tween In " + typeof(T).Name;
			}
		}
		/// <summary>
		/// The string to use for the tween in object.
		/// </summary>
		private string TweenOutStringName {
			get {
				return "Tween Out " + typeof(T).Name;
			}
		}
		#endregion

	}

	/// <summary>
	/// Aggregates a list of TweenInOutStructs to a list of relevant parameters for when I'm working with menus and UI.
	/// </summary>
	public struct TweenInOutToggles {


		#region FIELDS
		/// <summary>
		/// The GameObject being associated with these toggles.
		/// </summary>
		[SerializeField, BoxGroup("Toggles", showLabel: false)]
		private GameObject targetObject;
		/// <summary>
		/// The in/out object for the time.
		/// </summary>
		[OdinSerialize, BoxGroup("Toggles", showLabel: false), Toggle("enabled")]
		private TweenInOutParams<float?> timeTweener;
		/// <summary>
		/// The in/out object for the ease.
		/// </summary>
		[OdinSerialize, BoxGroup("Toggles", showLabel: false), Toggle("enabled")]
		private TweenInOutParams<Ease?> easeTweener;
		/// <summary>
		/// The in/out object for the position.
		/// </summary>
		[OdinSerialize, BoxGroup("Toggles", showLabel: false), Toggle("enabled")]
		private TweenInOutParams<Vector2> positionTweener;
		/// <summary>
		/// The in/out object for the position.
		/// </summary>
		[OdinSerialize, BoxGroup("Toggles", showLabel: false), Toggle("enabled")]
		private TweenInOutParams<float?> magnitudeTweener;
		/// <summary>
		/// The in/out object for the position.
		/// </summary>
		[OdinSerialize, BoxGroup("Toggles", showLabel: false), Toggle("enabled")]
		private TweenInOutParams<Color> colorTweener;
		#endregion

		#region GETTERS - TARGET VALUES
		/// <summary>
		/// Gets the component T if it is attached to the target.
		/// Used for quickly grabbing RectTransforms and shit.
		/// Will fail if it isn't attached.
		/// </summary>
		/// <typeparam name="T">The type of component to grab the object for.</typeparam>
		/// <returns></returns>
		public T GetTarget<T>() {
			return this.targetObject.GetComponent<T>();
		}
		#endregion

		#region GETTERS - TOGGLE VALUES
		/// <summary>
		/// The in/out object for the time.
		/// </summary>
		public TweenInOutParams<float?> Time {
			get {
				return this.timeTweener;
			}
		}
		/// <summary>
		/// The in/out object for the ease.
		/// </summary>
		public TweenInOutParams<Ease?> Ease {
			get {
				return this.easeTweener;
			}
		}
		/// <summary>
		/// The in/out object for the position.
		/// </summary>
		public TweenInOutParams<Vector2> Position {
			get {
				return this.positionTweener;
			}
		}
		/// <summary>
		/// The in/out object for the magnitude.
		/// </summary>
		public TweenInOutParams<float?> Magnitude {
			get {
				return this.magnitudeTweener;
			}
		}
		/// <summary>
		/// The in/out object for the color.
		/// </summary>
		public TweenInOutParams<Color> Color {
			get {
				return this.colorTweener;
			}
		}
		#endregion

	}

}