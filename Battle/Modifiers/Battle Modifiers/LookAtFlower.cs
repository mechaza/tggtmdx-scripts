using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Grawly.Battle.Modifiers {

	/// <summary>
	/// Basically just... find an object called Flower and make the camera look at it.
	/// Used for debugging purposes.
	/// </summary>
	public class LookAtFlower : BattleModifier, IOnPreTurn {

		#region INTERFACE IMPLEMENTATION - IONPRETURN
		/// <summary>
		/// A very basic pre-turn sequence that looks for the Flower Cam, then hangs on there for a lil bit.
		/// </summary>
		/// <returns></returns>
		public BattleReaction OnPreTurn() {
			// throw new System.Exception("This doesn't work anymore! Fix it!");

			return delegate (BattleReactionSequence battleReactionSequence) {
				// First off, create a sequence.
				Sequence seq = DOTween.Sequence();

				// Start by seeing if I can find the flower cam.
				GameObject flowerCamObject = GameObject.Find("DEBUG FLOWER CAM");
				// If I can't, return null so the sequence isn't added.
				if (flowerCamObject == null) {
					battleReactionSequence.ExecuteNextReaction();
					return;
					// return seq;
				}
				// Otherwise, make a sequence..!
				seq.AppendCallback(new TweenCallback(delegate {
					flowerCamObject.transform.GetChild(0).gameObject.SetActive(true);
				}));
				seq.AppendInterval(7f);
				seq.AppendCallback(new TweenCallback(delegate {
					flowerCamObject.transform.GetChild(0).gameObject.SetActive(false);
				}));
				seq.AppendInterval(7f);

				seq.OnComplete(new TweenCallback(delegate { battleReactionSequence.ExecuteNextReaction(); }));
				seq.Play();
			};

			/*// Start by seeing if I can find the flower cam.
			GameObject flowerCamObject = GameObject.Find("DEBUG FLOWER CAM");
			// If I can't, return null so the sequence isn't added.
			if (flowerCamObject == null) {
				return seq;
			}
			// Otherwise, make a sequence..!
			seq.AppendCallback(new TweenCallback(delegate {
				flowerCamObject.transform.GetChild(0).gameObject.SetActive(true);
			}));
			seq.AppendInterval(7f);
			seq.AppendCallback(new TweenCallback(delegate {
				flowerCamObject.transform.GetChild(0).gameObject.SetActive(false);
			}));
			seq.AppendInterval(7f);
			return seq;*/
		}
		#endregion

	}


}