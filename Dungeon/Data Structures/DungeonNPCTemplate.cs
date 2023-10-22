using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using Grawly.Chat;
using Grawly.UI;
using Sirenix.Serialization;

namespace Grawly.Dungeon {
	
	/// <summary>
	/// A template that can be used to create an NPC at runtime.
	/// </summary>
	[CreateAssetMenu(menuName = "Grawly/NPC/NPC Template")]
	public class DungeonNPCTemplate : SerializedScriptableObject {

		#region FIELDS - TOGGLES
		/// <summary>
		/// The sprite that should be used to represent this NPC on the overworld.
		/// </summary>
		[OdinSerialize]
		[PropertyTooltip("The sprite that should be used to represent this NPC on the overworld.")]
		public Sprite NPCOverworldSprite { get; private set; }
		/// <summary>
		/// Should a particular asset be used when interacting with this NPC?
		/// </summary>
		[OdinSerialize]
		[PropertyTooltip("Should a particular asset be used when interacting with this NPC?")]
		public bool UseChatScriptAsset { get; private set; } = false;
		/// <summary>
		/// The text to use in chat when interacting with this NPC.
		/// Only relevant when not using an actual chat script asset.
		/// </summary>
		[OdinSerialize, HideIf("UseChatScriptAsset")]
		[PropertyTooltip("The text to use in chat when interacting with this NPC.")]
		public string NPCDialogueText { get; private set; } = "";
		/// <summary>
		/// The asset to use when interacting with this NPC if a particular asset should be used.
		/// </summary>
		[OdinSerialize, ShowIf("UseChatScriptAsset")]
		[PropertyTooltip("The asset to use when interacting with this NPC if a particular asset should be used.")]
		public SerializedChatScriptDX NPCChatScriptAsset { get; private set; }
		#endregion

		#region GETTERS
		/// <summary>
		/// Gets the chat script that should be used when interacting with this NPC.
		/// </summary>
		/// <returns></returns>
		public IChatScript GetNPCChatScript() {
			
			// If using a specific asset, just return that.
			if (this.UseChatScriptAsset) {
				return this.NPCChatScriptAsset;
			} else {
				// If using raw text, create a plain chat script and return a new object with that dialogue passed in.
				PlainChatScript plainScript = new PlainChatScript(rawText: this.NPCDialogueText);
				return plainScript;
			}
			
		}
		#endregion
		
	}
}