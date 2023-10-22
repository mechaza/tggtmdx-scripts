namespace Grawly.Chat {
	
	/// <summary>
	/// Just a way to help out with designing inspectors.
	/// </summary>
	public enum ChatScriptReferenceType {
		Inline		= 0,		// For when chats should be edited directly in the inspector.
		ChatAsset	= 1,		// For when chats should point to an actual ScriptableObject (SerializedChatScript or SimpleChatScript)
		TextAsset	= 2,		// For when chats should point to a .txt file.
	}

	/// <summary>
	/// I dunno man I'm fucking tired.
	/// </summary>
	public enum ChatScriptAssetType {
		Simple		= 0,
		Serialized	= 1,
	}
	
}