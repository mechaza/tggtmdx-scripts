using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Grawly.Calendar.Behavior;
using Grawly.Dungeon;
using Sirenix.Serialization;
using UnityEngine.Serialization;

namespace Grawly.Calendar {
	
	/// <summary>
	/// Contains a set of information for a scene to load at a given time of day.
	/// </summary>
	[InlineProperty, HideReferenceObjectPicker]
	[System.Serializable]
	[GUIColor(r: 1f, g: 0.64f, b: 0.1f, a: 1f)]
	public class StoryBeat {

		#region CONSTANTS
		/// <summary>
		/// The maximum length of characters that can be displayed in the foldout group.
		/// </summary>
		private const int NOTES_CUTOFF_POINT = 50;
		#endregion

		#region FIELDS - NOTES
		/// <summary>
		/// A simple place to store notes for what should be happening on this story beat.
		/// </summary>
		[SerializeField, HideLabel, TextArea(minLines: 2, maxLines: 6)]
		[Title("Notes")]
		[FoldoutGroup("$FoldoutGroupTitle")]
		private string storyBeatNotes = "";
		#endregion
		
		#region FIELDS - TOGGLES : GENERAL
		/// <summary>
		/// The day number.
		/// This gets set every time a story beat is retrieved from a CalendarDayTemplate.
		/// </summary>
		public int DayNumber { get; set; } = -1;
		#endregion
		
		#region FIELDS - TOGGLES : TIME TRANSITIONS
		/// <summary>
		/// The time of day for this story beat.
		/// </summary>
		[Title("Destination")]
		[FoldoutGroup("$FoldoutGroupTitle")]
		[PropertyTooltip("The time of day for this story beat.")]
		public TimeOfDayType timeOfDay = TimeOfDayType.EarlyMorning;
		#endregion
		
		#region FIELDS - TOGGLES : SCENE DESTINATIONS
		/// <summary>
		/// The type of story beat.
		/// This is mostly just to help me design in the inspector.
		/// </summary>
		[FoldoutGroup("$FoldoutGroupTitle")]
		[LabelText("Initial Scene Type")]
		[PropertyTooltip("Whether or not the scene initially loaded into for this StoryBeat uses a pre-defined scene name or a specific one altogether.")]
		public GameTransitionType gameTransitionType;
		/// <summary>
		/// The name of the scene to load for this story beat.
		/// </summary>
		[ShowIf("UseCustomScene")]
		[FoldoutGroup("$FoldoutGroupTitle")]
		[LabelText("Target Scene Name")]
		[PropertyTooltip("The name of the scene to load for this story beat.")]
		public string sceneName;
		/// <summary>
		/// The overworld type to transition to, if not using a custom scene.
		/// </summary>
		[ShowIf("UseLocationScene")]
		[FoldoutGroup("$FoldoutGroupTitle")]
		[LabelText("Target Location Type")]
		[PropertyTooltip("The type of location that should be loaded into when loading this story beat.")]
		public LocationType locationType;
		/// <summary>
		/// The camera tag to use as the override, if set to anything other than None.
		/// </summary>
		[FoldoutGroup("$FoldoutGroupTitle")]
		public CameraTagType cameraTagOverride = CameraTagType.None;
		#endregion

		#region FIELDS - MAP SCREEN DEFINITIONS
		/// <summary>
		/// A list of locations available on the map screen.
		/// </summary>
		[Title("Map Screen")]
		[FoldoutGroup("$FoldoutGroupTitle")]
		[PropertyTooltip("The items that should be available for selection on the map screen.")]
		public List<MapScreenLocationDefinition> mapScreenDefinitions = new List<MapScreenLocationDefinition>();
		#endregion

		/*#region FIELDS - NPC AVAILABILITY
		/// <summary>
		/// The NPCs that should be available during this story beat.
		/// If an NPC is not in this list, any DungeonNPCDX that references it will be turned off.
		/// </summary>
		[Title("NPCs")]
		[FoldoutGroup("$FoldoutGroupTitle")]
		[PropertyTooltip("The NPCs that should be available during this story beat. If an NPC is not in this list, any DungeonNPCDX that references it will be turned off.")]
		public List<DungeonNPCTemplate> availableNPCs = new List<DungeonNPCTemplate>();
		#endregion*/
		
		#region FIELDS - STORY BEHAVIORS
		/// <summary>
		/// The story behaviors for this story beat.
		/// </summary>
		[OdinSerialize, Title("Behaviors"), ListDrawerSettings(Expanded = true), Space(10)]
		// [GUIColor(r: 1f, g: 0.9f, b: 0.9f, a: 1f)]
		[HideReferenceObjectPicker]
		[FoldoutGroup("$FoldoutGroupTitle")]
		private List<StoryBehavior> storyBehaviors = new List<StoryBehavior>();
		/// <summary>
		/// The story behaviors for this story beat.
		/// They are cloned before they are returned.
		/// </summary>
		public List<StoryBehavior> StoryBehaviors {
			get {
				return this.storyBehaviors.Select(b => b.Clone()).ToList();
			}
		}
		#endregion

		#region PROPERTIES - STATE
		/// <summary>
		/// Does this story beat have a camera tag override?
		/// </summary>
		public bool HasCameraOverride => this.cameraTagOverride != CameraTagType.None;
		#endregion
		
		#region ODIN HELPERS
		/// <summary>
		/// I have a button that allows me to add basic story beats inside of the CalendarDayTemplate.
		/// This should never be used in any other context.
		/// </summary>
		/// <param name="storyBehavior"></param>
		public void AddPlaceholderStoryBehavior(StoryBehavior storyBehavior) {
			this.storyBehaviors.Add(storyBehavior);
		}
		private bool UseLocationScene() {
			return this.gameTransitionType == GameTransitionType.LocationTransition ;
		}
		private bool UseCustomScene() {
			return this.gameTransitionType == GameTransitionType.CustomTransition;
		}
		private string TargetSceneNameForInspector {
			get {
				if (this.UseCustomScene()) {
					return this.sceneName;
				} else if (this.UseLocationScene()) {
					return this.locationType.ToString();
				} else {
					return "???";
				}
			}
		}
		private string FoldoutGroupTitle {
			get {
				// return this.timeOfDay.ToString() + " (" + this.TargetSceneNameForInspector + ")";
				// return this.timeOfDay.ToString() + " (" + this.TargetSceneNameForInspector + ") - " + this.storyBeatNotes;
				
				// Create the prefix.
				string titlePrefix = this.timeOfDay.ToString() + " (" + this.TargetSceneNameForInspector + ")";
				
				// If there are notes available, append them to the title.
				if (string.IsNullOrWhiteSpace(this.storyBeatNotes) == false) {
					
					// Create a placeholder thats effectively a copy of the notes. 
					string titleSuffix = this.storyBeatNotes;
					
					// If the notes are too long for the foldout label, split it up.
					int notesLength = titleSuffix.Length;
					if (notesLength >= NOTES_CUTOFF_POINT) {
						titleSuffix = titleSuffix.Substring(startIndex: 0, length: NOTES_CUTOFF_POINT - 3) + "...";
					}
					
					// Append the string to the end and return it.
					string fullTitle = titlePrefix + " - " + titleSuffix;
					return fullTitle;
					
				} else {
					// If there are no notes, just return the prefix.
					return titlePrefix;
				}

			}
		}
		#endregion
		
	}
	
}
