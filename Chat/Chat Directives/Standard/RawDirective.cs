using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Utilities;

namespace Grawly.Chat {
	
	[Title("Raw")]
	[GUIColor(r: 0.8f, g: 0.8f, b: 1f, a: 1f)]
	public class RawDirective : ChatDirective {

		#region CONSTANTS
		/// <summary>
		/// The maximum length of characters that can be displayed in the foldout group.
		/// </summary>
		private const int NOTES_CUTOFF_POINT = 50;
		#endregion
		
		#region FIELDS - ODIN HELPERS
		/// <summary>
		/// A string to display in the foldout group when this directive is displayed in the inspector.
		/// </summary>
		[SerializeField, GUIColor(1f, 1f, 1f, 1f), LabelWidth(DEFAULT_LABEL_WIDTH)]
		[FoldoutGroup("$FoldoutGroupTitle")]
		private string directiveNotes = "";
		#endregion
		
		#region FIELDS - RAW TEXT
		/// <summary>
		/// The text to be parsed from this directive.
		/// </summary>
		[SerializeField, MultiLineProperty(10), Space(5), GUIColor(1f, 1f, 1f, 1f), LabelWidth(DEFAULT_LABEL_WIDTH)]
		[FoldoutGroup("$FoldoutGroupTitle")]
		public string rawText = "";
		#endregion

		
		
		#region CHAT DIRECTIVE IMPLEMENTATION
		/// <summary>
		/// Evaluate the raw directive. This should only be called once by the ChatController.
		/// </summary>
		/// <param name="chatController"></param>
		public override void EvaluateDirective(ChatControllerDX chatController) {
			throw new Exception("This should never happen.");
			/*this.chatController = chatController;
			this.parser = new RawDirectiveParser();
			this.parser.PrepareScript(rawText);
			Debug.LogWarning("NOTE: Remember to get the tags!");
			NextDirective(this.chatController);*/
		}
		#endregion

		
		#region ODIN HELPERS
		protected override string FoldoutGroupTitle {
			get {

				string foldoutStr = "Raw - ";
				string finalNotes = (this.directiveNotes.IsNullOrWhitespace() == true) 
					? this.rawText 
					: this.directiveNotes;
				
				
				int notesLength = finalNotes.Length;
				if (notesLength >= NOTES_CUTOFF_POINT) {
					finalNotes = finalNotes.Substring(startIndex: 0, length: NOTES_CUTOFF_POINT - 3) + "...";
				}
				
				
				foldoutStr += "[" + finalNotes + "]";
				return foldoutStr;
				
				
			}
		}
		#endregion

	}

	

}