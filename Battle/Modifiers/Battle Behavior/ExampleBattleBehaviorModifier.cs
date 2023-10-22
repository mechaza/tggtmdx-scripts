using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Grawly.Battle.BattleMenu;
using Sirenix.OdinInspector;

namespace Grawly.Battle.Modifiers {

	[System.Serializable]
	[TypeInfoBox("Just testing some shit out.")]
	public class ExampleBattleBehaviorModifier : BattleBehaviorModifier, IOnTurnStart {

		#region INTERFACE IMPLEMENTATION - IONTURNSTART
		public BattleReaction OnTurnStart() {
			// throw new System.Exception("This does not work anymore!");
			return delegate (BattleReactionSequence battleReactionSequence) {
				// First off, create a sequence.
				Sequence seq = DOTween.Sequence();
				seq.AppendCallback(new TweenCallback(delegate {
					BattleNotifier.DisplayNotifier("OOOOOO HERES ONE...", 3f);
				}));
				seq.AppendInterval(3f);
				seq.AppendCallback(new TweenCallback(delegate {
					BattleNotifier.DisplayNotifier("ANTOHER.", 3f);
				}));
				seq.AppendInterval(3f);
				seq.OnComplete(new TweenCallback(delegate { battleReactionSequence.ExecuteNextReaction(); }));
				seq.Play();
			};
			/*seq.AppendCallback(new TweenCallback(delegate {
				BattleNotifier.DisplayNotifier("OOOOOO HERES ONE...", 3f);
			}));
			seq.AppendInterval(3f);
			seq.AppendCallback(new TweenCallback(delegate {
				BattleNotifier.DisplayNotifier("ANTOHER.", 3f);
			}));
			seq.AppendInterval(3f);
			return seq;*/
		}
		#endregion

		/*#region INSPECTOR STUFF
		private static string inspectorDescription = "Just testing some shit out.";
		protected override string InspectorDescription {
			get {
				return inspectorDescription;
			}
		}
		#endregion*/


	}


}