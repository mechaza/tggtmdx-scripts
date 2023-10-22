using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*

	Just moving a lot of the junk that was in Definitions.cs over here.
	 
*/

namespace Grawly.Chat {

	/// <summary>
	/// Sets a standard for chat scripts.
	/// </summary>
	public interface IChatScript {
		/// <summary>
		/// A list of chat speakers to be used in the RuntimeScript.
		/// </summary>
		List<RuntimeChatSpeaker> RuntimeChatSpeakers { get; }
		/// <summary>
		/// Returns all the directives for this script.
		/// </summary>
		List<ChatDirective> Directives { get; }
	}

	public enum ChatSpeakerPositionType {
		None = -1,
		Left = 0,
		Right = 1,
		TopLeft = 2,
		TopRight = 3,
		BottomCenter = 4,
		TopCenter = 5,
		Alpha = 10,
		Omega = 11,
	}

	/// <summary>
	/// The different types of "themes" to use for the chat box.
	/// </summary>
	public enum ChatBoxThemeType {
		SolidColor		= 0,
		CheckerPan		= 1,		// This is used when manipulating an image that is a child of the front.
	}

	/// <summary>
	/// A callback for when the chat is opened by a chat user.
	/// </summary>
	public delegate void ChatOpenedCallback();

	/// <summary>
	/// A callback for when the chat is closed.
	/// </summary>
	/// <param name="str"></param>
	/// <param name="num"></param>
	/// <param name="toggle"></param>
	public delegate void ChatClosedCallback(string str = "", int num = 0, bool toggle = false);


}