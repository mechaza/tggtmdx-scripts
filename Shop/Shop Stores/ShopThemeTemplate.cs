using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Grawly.Shop {
	
	/// <summary>
	/// Contains the data on how a store should look.
	/// I.e., colors, icons, etc.
	/// </summary>
	[CreateAssetMenu(menuName = "Grawly/Shop/Store Theme")]
	public class ShopThemeTemplate : SerializedScriptableObject {

		#region FIELDS - GRAPHICS
		/// <summary>
		/// The sprite to use for the shop keeper's bust up.
		/// </summary>
		[OdinSerialize, TabGroup("Theme", "Graphics")]
		public Sprite ShopKeeperBustUpSprite { get; private set; }
		#endregion

		#region FIELDS - COLORS : BUST UP
		/// <summary>
		/// The prefix to attach to the speech bubble for the shop keepers text bubble.
		/// </summary>
		[OdinSerialize, TabGroup("Theme", "Colors"), Title("Bust Up")]
		public string BustUpSpeechBubbleTextPrefix { get; private set; } = "";
		#endregion

		#region FIELDS - COLORS : COUNTER
		/// <summary>
		/// The color to use for the front image of the money counter.
		/// </summary>
		[OdinSerialize, TabGroup("Theme", "Colors"), Title("Counter")]
		public Color MoneyCounterFrontColor { get; private set; } = Color.black;
		/// <summary>
		/// The color to use for the dropshadow image of the money counter.
		/// </summary>
		[OdinSerialize, TabGroup("Theme", "Colors")]
		public Color MoneyCounterDropshadowColor { get; private set; } = Color.white;
		/// <summary>
		/// The prefix to use on the label displaying the total funds.
		/// </summary>
		[OdinSerialize, TabGroup("Theme", "Colors")]
		public string MoneyCounterTotalFundsLabelPrefix { get; private set; } = "<c=white>";
		/// <summary>
		/// The prefix to use on the label displaying the subtracted funds.
		/// </summary>
		[OdinSerialize, TabGroup("Theme", "Colors")]
		public string MoneyCounterSubtractedFundsLabelPrefix { get; private set; } = "<c=black>";
		/// <summary>
		/// The material to use for the total funds label.
		/// </summary>
		[OdinSerialize, TabGroup("Theme", "Colors")]
		public Material MoneyCounterTotalFundsLabelMaterial { get; private set; }
		/// <summary>
		/// The material to use for the total funds label.
		/// </summary>
		[OdinSerialize, TabGroup("Theme", "Colors")]
		public Material MoneyCounterSubtractedFundsLabelMaterial { get; private set; }
		#endregion
		
		#region FIELDS - COLORS : TOP LEVEL BUTTON
		/// <summary>
		/// The color that should be used on the diamond front when it is not highlighted.
		/// </summary>
		[OdinSerialize, TabGroup("Theme", "Colors"), Title("Top Level (Idle)")]
		public Color TopLevelDiamondFrontIdleColor { get; private set; } = Color.white;
		/// <summary>
		/// The color that should be used on the diamond dropshadow when it is not highlighted.
		/// </summary>
		[OdinSerialize, TabGroup("Theme", "Colors")]
		public Color TopLevelDiamondDropshadowIdleColor { get; private set; } = Color.black;
		/// <summary>
		/// The color that should be used on the icon image when it is not highlighted.
		/// </summary>
		[OdinSerialize, TabGroup("Theme", "Colors")]
		public Color TopLevelIconIdleColor { get; private set; } = Color.red;
		/// <summary>
		/// The material to use when the label on the top level button is idle.
		/// </summary>
		[OdinSerialize, TabGroup("Theme", "Colors")]
		public Material TopLevelLabelIdleMaterial { get; private set; }
		/// <summary>
		/// The prefix to attach to the top level label when idle.
		/// </summary>
		[OdinSerialize, TabGroup("Theme", "Colors")]
		public string TopLevelLabelIdlePrefixString { get; private set; } = "";
		/// <summary>
		/// The color that should be used on the diamond front when it is highlighted.
		/// </summary>
		[OdinSerialize, TabGroup("Theme", "Colors"), Title("Top Level (Highlight)")]
		public Color TopLevelDiamondFrontHighlightColor { get; private set; } = Color.black;
		/// <summary>
		/// The color that should be used on the diamond dropshadow when it is highlighted.
		/// </summary>
		[OdinSerialize, TabGroup("Theme", "Colors")]
		public Color TopLevelDiamondDropshadowHighlightColor { get; private set; } = Color.white;
		/// <summary>
		/// The color that should be used on the icon image when it is highlighted.
		/// </summary>
		[OdinSerialize, TabGroup("Theme", "Colors")]
		public Color TopLevelIconHighlightColor { get; private set; } = Color.white;
		/// <summary>
		/// The material to use when the label on the top level button is idle.
		/// </summary>
		[OdinSerialize, TabGroup("Theme", "Colors")]
		public Material TopLevelLabelHighlightMaterial { get; private set; }
		/// <summary>
		/// The prefix to attach to the top level label when highlighted.
		/// </summary>
		[OdinSerialize, TabGroup("Theme", "Colors")]
		public string TopLevelLabelHighlightPrefixString { get; private set; } = "";
		#endregion

		#region FIELDS - COLORS : EQUIPMENT SUMMARY
		/// <summary>
		/// The color to use for the equipment summary's foreground diamond.
		/// </summary>
		[OdinSerialize, TabGroup("Theme", "Colors"), Title("Equipment Summary")]
		public Color EquipmentSummaryForegroundDiamondColor { get; private set; } = Color.blue;
		/// <summary>
		/// The color to use for the equipment summary's foreground diamond.
		/// </summary>
		[OdinSerialize, TabGroup("Theme", "Colors")]
		public Color EquipmentSummaryBackgroundDiamondColor { get; private set; } = Color.black;
		/// <summary>
		/// The color to use for the rectangle behind a detail's icon.
		/// </summary>
		[OdinSerialize, TabGroup("Theme", "Colors")]
		public Color EquipmentSummaryIconBackingColor { get; private set; } = Color.red;
		/// <summary>
		/// The color to use for a detail's icon itself.
		/// </summary>
		[OdinSerialize, TabGroup("Theme", "Colors")]
		public Color EquipmentSummaryIconShapeColor { get; private set; } = Color.yellow;
		/// <summary>
		/// The prefix to use on labels as part of an equipment detail's label.
		/// </summary>
		[OdinSerialize, TabGroup("Theme", "Colors")]
		public string EquipmentSummaryDetailLabelPrefix { get; private set; } = "";
		/// <summary>
		/// The material to use for the text on an equipment summary's detail labels.
		/// </summary>
		[OdinSerialize, TabGroup("Theme", "Colors")]
		public Material EquipmentSummaryDetailLabelMaterial { get; private set; }
		#endregion

		#region PROTOTYPE FIELDS - COLORS
		/// <summary>
		/// A prototype color to use while debugging UI elements because I'm fed up.
		/// </summary>
		public Color PrototypeColor1 { get; private set; } = new Color(0.0f / 255.0f, 157.0f / 255.0f, 255.0f / 255.0f, 1.0f);
		/// <summary>
		/// A prototype color to use while debugging UI elements because I'm fed up.
		/// </summary>
		public Color PrototypeColor2 { get; private set; } = new Color(255.0f / 255.0f, 157.0f / 255.0f, 255.0f / 255.0f, 1.0f);
		/// <summary>
		/// A prototype color to use while debugging UI elements because I'm fed up.
		/// </summary>
		public Color PrototypeColor3 { get; private set; } = new Color(0.0f / 255.0f, 255.0f / 255.0f, 157.0f / 255.0f, 1.0f);
		/// <summary>
		/// A prototype color to use while debugging UI elements because I'm fed up.
		/// </summary>
		public Color PrototypeColor4 { get; private set; } = new Color(157.0f / 255.0f, 157.0f / 255.0f, 157.0f / 255.0f, 1.0f);
		/// <summary>
		/// A prototype prefix to use for labels.
		/// </summary>
		public string PrototypeLabelPrefix { get; private set; } = "";
		/// <summary>
		/// A prototype material to use for labels. I'm fed up! I'm fucking done.
		/// </summary>
		public Material PrototypeLabelMaterial => DataController.GetDefaultSTMMaterial("UI Dropshadow 2");
		#endregion

	}
}