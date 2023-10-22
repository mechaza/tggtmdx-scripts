using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public static class GrawlyExtensions {

	// https://stackoverflow.com/questions/273313/randomize-a-listt

	private static System.Random rng = new System.Random();
	/// <summary>
	/// Returns a random element.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="list"></param>
	/// <returns></returns>
	public static T Random<T>(this IList<T> list) {
		if (list.Count == 0) { return default(T); }
		return list[rng.Next(minValue: 0, maxValue: list.Count)];
	}
	/// <summary>
	/// Returns a random element while also removing it from the list.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="list"></param>
	/// <returns></returns>
	public static T PullRandom<T>(this IList<T> list) {
		if (list.Count == 0) { return default(T); }
		int index = rng.Next(minValue: 0, maxValue: list.Count);
		T el = list[index];
		list.RemoveAt(index);
		return el;
	}
	public static void Shuffle<T>(this IList<T> list) {
		int n = list.Count;
		while (n > 1) {
			n--;
			int k = rng.Next(n + 1);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}
	/// <summary>
	/// Returns a random number within the range of 0 and the number of elements in this list.
	/// </summary>
	/// <param name="list"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public static int RandomIndex<T>(this IList<T> list) {
		int randomIndex = rng.Next(minValue: 0, maxValue: list.Count);
		return randomIndex;
	}
	/// <summary>
	/// Returns a random value between the two values of this vector.
	/// </summary>
	/// <param name="range"></param>
	/// <returns></returns>
	public static int Random(this Vector2Int range) {
		return UnityEngine.Random.Range(minInclusive: range.x, maxExclusive: range.y);
	}
	

}
