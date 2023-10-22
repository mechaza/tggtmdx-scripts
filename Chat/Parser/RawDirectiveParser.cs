using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

namespace Grawly.Chat {

	/// <summary>
	/// A dedicated class for parsing out a raw directive and generating other directives from it.
	/// </summary>
	public static class RawDirectiveParser {

		/// <summary>
		/// A way for me to store data when passing it around. I would love to use tuples! but those arent really ready yet in C#6 :)
		/// </summary>
		private class MatchCollectionStringTuple {
			public MatchCollection matchCollection { get; private set; }
			public string parsedString { get; private set; }
			public MatchCollectionStringTuple(MatchCollection matchCollection, string parsedString) {
				this.matchCollection = matchCollection;
				this.parsedString = parsedString;
			}
		}

		#region PATTERN MATCHING
		/// <summary>
		/// A singular place for me to get matches of lines.
		/// </summary>
		/// <param name="line">The line to regex out.</param>
		/// <returns></returns>
		private static MatchCollectionStringTuple ChatRegex(string line) {
			return new MatchCollectionStringTuple(
				matchCollection: Regex.Matches(line, @"(\w*):\s([^;]*);?"),
				parsedString: line);
		}
		/// <summary>
		/// Gets the first group in a match.
		/// </summary>
		public static string GetLabel(Match match) {
			return FixLine(match.Groups[1].Value);
		}
		/// <summary>
		/// Gets the second group in a match.
		/// </summary>
		public static string GetProperty(Match match) {
			return FixLine(match.Groups[2].Value);
		}
		/// <summary>
		/// stupid fucking windows carriage returns
		/// </summary>
		private static string FixLine(string line) {
			return line.Trim(new char[] { '\r', '\n' });
		}
		#endregion

		#region PARSING
		/// <summary>
		/// Parses out a list of directives from a passed in string.
		/// </summary>
		/// <param name="text">The text that should be parsed.</param>
		/// <returns>A list of directives made from that good shit.</returns>
		public static List<ChatDirective> GetDirectives(string text) {
			
			// Trim out carriage returns from the incoming text and split by newlines. Also parse out my own comments.
			// List<string> script = text.Trim('\r').Split('\n').ToList();
			List<string> script = text
				.Trim('\r')
				.Split('\n')
				.Where(s => s.StartsWith("//") == false)
				.ToList();

			// I felt clever for this.
			return script
				.Select(s => ChatRegex(line: FixLine(line: s)))																// Go through each string and regex the shit out of it. (i.e., make them MatchCollections)
				.Select(mcst => new ChatDirectiveParams(matches: mcst.matchCollection, parsedString: mcst.parsedString))	// Create new ChatDirectiveParams from those MatchCollections
				.Select(cdp => DecideDirective(directiveParams: cdp))														// Call ParseDirectiveParams to turn those params into an actual directive.
				.ToList();

		}
		/// <summary>
		/// Creates a ChatDirective from a set of DirectiveParams.
		/// </summary>
		/// <param name="directiveParams">The parameters to parse.</param>
		/// <returns></returns>
		private static ChatDirective DecideDirective(ChatDirectiveParams directiveParams) {
			// Use System.Activator to create an Instance of a ChatDirective by asking the directive params what type it is,
			// and then also passing those very same params into the constructor.
			return (ChatDirective)System.Activator.CreateInstance(directiveParams.ChatDirectiveType, directiveParams);
		}
		#endregion

	}

}