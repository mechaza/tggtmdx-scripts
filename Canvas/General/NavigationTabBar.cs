using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System.Linq;

namespace Grawly.UI {
	
	/// <summary>
	/// A generic tab bar that just shows graphics for shit.
	/// </summary>
	public class NavigationTabBar : MonoBehaviour {

		#region FIELDS - SCENE REFERENCES : NAVIGATION BAR
		/// <summary>
		/// The image for the left navigation arrow.
		/// </summary>
		[SerializeField, TabGroup("Tutorial Screen", "Scene References")]
		private GameObject leftNavigationArrowGameObject;
		/// <summary>
		/// The image for the right navigation arrow image.
		/// </summary>
		[SerializeField, TabGroup("Tutorial Screen", "Scene References")]
		private GameObject rightNavigationArrowGameObject;
		/// <summary>
		/// The dots that get shown to indicate what the current page is.
		/// </summary>
		[SerializeField, TabGroup("Tutorial Screen", "Scene References")]
		private List<Image> navigationDotImages = new List<Image>();
		#endregion
		
		#region MAIN CALLS
		/// <summary>
		/// Refreshes the navigation bar's visuals to be up 
		/// </summary>
		/// <param name="currentIndex"></param>
		/// <param name="totalScreens"></param>
		public void RefreshNavigationBar(int currentIndex, int totalScreens) {

			// Deactivate all the navigation images.
			this.navigationDotImages.ForEach(i => i.gameObject.SetActive(false));

			// Take only the number of ones that I need and set them active again and make them black.
			this.navigationDotImages.Take(count: totalScreens).ToList().ForEach(i => i.gameObject.SetActive(true));
			this.navigationDotImages.Take(count: totalScreens).ToList().ForEach(i => i.color = Color.black);

			// Make the currently highlighted index's dot burple.
			this.navigationDotImages[currentIndex].color = GrawlyColors.colorDict[GrawlyColorTypes.Purple];

			// Enable the left arrow if the current index is not the first.
			leftNavigationArrowGameObject.SetActive(value: (currentIndex == 0) == false);

			// Enable the right arrow if the current index is not the last.
			rightNavigationArrowGameObject.SetActive(value: (currentIndex == totalScreens - 1) == false);

		}
		#endregion
		
	}

	
}