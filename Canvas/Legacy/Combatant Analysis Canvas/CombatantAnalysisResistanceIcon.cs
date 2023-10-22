using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grawly.Battle;
using UnityEngine.UI;

namespace Grawly.UI.Legacy {
	public class CombatantAnalysisResistanceIcon : MonoBehaviour {

		/// <summary>
		/// The label meant to fill with the resistance.
		/// </summary>
		[SerializeField]
		private SuperTextMesh resistanceLabel;
		/// <summary>
		/// The icon representing the resistance.
		/// </summary>
		[SerializeField]
		private Image elementIcon;
		/// <summary>
		/// The elemental type for this resistenace.
		/// </summary>
		[SerializeField]
		private ElementType elementType;

		private void Start() {
			// Override the sprite just to be safe.
			elementIcon.sprite = DataController.Instance.GetElementalIcon(elementType);
		}

		public void Build(Combatant combatant) {
			
			// Check what the resistance is, first and foremost.
			ResistanceType? resistanceType = (combatant is Enemy == true)
				? GameController.Instance.Variables.GetKnownEnemyResistance(enemy: (Enemy)combatant, elementType: this.elementType)
				: combatant.CheckResistance(elementType);

			if (resistanceType.HasValue == false) {
				this.elementIcon.CrossFadeColor(targetColor: Color.gray, duration: 0f, ignoreTimeScale: true, useAlpha: false);
				this.resistanceLabel.Text = "?";
				return;
			} else {
				this.elementIcon.CrossFadeColor(targetColor: Color.white, duration: 0f, ignoreTimeScale: true, useAlpha: false);
			}

			// There might need to be certain effects applied to that.
			switch (resistanceType) {
				case ResistanceType.Nm:
					// Normal shouldn't actually have any string at all.
					this.resistanceLabel.Text = "";
					break;
				case ResistanceType.Wk:
					this.resistanceLabel.Text = resistanceType.ToString();
					break;
				default:
					// Basically all other options show some kind of resistance. Make that gray.
					this.resistanceLabel.Text = "<c=gray>" + resistanceType.ToString();
					break;
			}

		}

	}

}