using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.Utils {

	/// <summary>
	/// A class I use to abstract out calling of garbage collection.
	/// </summary>
	public static class GrawlyGarbageCollection {

		/// <summary>
		/// Calls System.GC.Collect()
		/// </summary>
		public static void Collect() {
			Debug.Log("Calling garbage collection."); // Fun Fact, I'm making this string and then throwing it away. lmao
			System.GC.Collect();
		}

	}


}