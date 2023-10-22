using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grawly.CodeBits {

	/// <summary>
	/// A small bit of code that is serialized in the inspector via Odin.
	/// I kept finding situations where I needed to serialize small bits of code so this is that.
	/// </summary>
	public abstract class CodeBit {

		/// <summary>
		/// Execute the codebit.
		/// </summary>
		public abstract void Run();

	}


}