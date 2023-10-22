using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
namespace Grawly.Math {

	// A delegate name for functions that take a level between 1 and 100 and spit out a value between 1 and 100.
	public delegate int StatCurveFunction(int lvl);

	// The different kinds of curves a stat can follow
	public enum StatCurveType {
		SinEaseInOut,
		Bezier1,
		Bezier2,
		Bezier3,
		Linear1,
		Linear2,
		Linear3,
		Linear4,
	}

	public enum ModifiedStatCurveType {
		Type1,
		Type2,
		Type3,
		Type4,
		Type5,
	}

	
	public static class GrawlMath {

		// Holds the different functions to be used with any given curve type.
		public static Dictionary<StatCurveType, StatCurveFunction> statCurves = new Dictionary<StatCurveType, StatCurveFunction>();
		public static Dictionary<ModifiedStatCurveType, StatCurveFunction> modifiedStatCurves = new Dictionary<ModifiedStatCurveType, StatCurveFunction>();

		static GrawlMath() {
			statCurves[StatCurveType.SinEaseInOut] = delegate (int lvl) {
				float x = ConvertLevelToDecimal(lvl);
				float s = Mathf.Sin((Mathf.PI * Mathf.Pow(x, 0.5f)) / 2);
				return ConvertDecimalToLevel(s * s);
			};
			statCurves[StatCurveType.Bezier1] = delegate (int lvl) {
				float x = ConvertLevelToDecimal(lvl);
				float y = GetYFromBezierCurve(Vector3.zero, new Vector3(.14f, .73f), Vector3.one, new Vector3(.24f, .98f), x);
				return ConvertDecimalToLevel(y);
			};
			statCurves[StatCurveType.Bezier2] = delegate (int lvl) {
				float x = ConvertLevelToDecimal(lvl);
				float y = GetYFromBezierCurve(Vector3.zero, new Vector3(.17f, .67f), Vector3.one, new Vector3(.83f, .67f), x);
				return ConvertDecimalToLevel(y);
			};
			statCurves[StatCurveType.Bezier3] = delegate (int lvl) {
				float x = ConvertLevelToDecimal(lvl);
				float y = GetYFromBezierCurve(Vector3.zero, new Vector3(.38f, .29f), Vector3.one, new Vector3(.98f, .45f), x);
				return ConvertDecimalToLevel(y);
			};
			statCurves[StatCurveType.Linear1] = delegate (int lvl) {
				float t = Mathf.InverseLerp(1f, 99f, lvl);
				float stat = Mathf.Lerp(1f, 40f, t);
				return Mathf.CeilToInt(stat);
			};
			statCurves[StatCurveType.Linear2] = delegate (int lvl) {
				float t = Mathf.InverseLerp(1f, 99f, lvl);
				float stat = Mathf.Lerp(1f, 55f, t);
				return Mathf.CeilToInt(stat);
			};
			statCurves[StatCurveType.Linear3] = delegate (int lvl) {
				float t = Mathf.InverseLerp(1f, 99f, lvl);
				float stat = Mathf.Lerp(1f, 70f, t);
				return Mathf.CeilToInt(stat);
			};
			statCurves[StatCurveType.Linear4] = delegate (int lvl) {
				float t = Mathf.InverseLerp(1f, 99f, lvl);
				float stat = Mathf.Lerp(1f, 85f, t);
				return Mathf.CeilToInt(stat);
			};
		}

		// http://cubic-bezier.com
		// https://denisrizov.com/2016/06/02/bezier-curves-unity-package-included/
		public static Vector3 GetPointOnBezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
			float u = 1f - t;
			float t2 = t * t;
			float u2 = u * u;
			float u3 = u2 * u;
			float t3 = t2 * t;

			Vector3 result = (u3) * p0 + (3f * u2 * t) * p1 + (3f * u * t2) * p2 + (t3) * p3;

			return result;
		}
		public static float GetYFromBezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
			return GetPointOnBezierCurve(p0, p1, p2, p3, t).y;
		}
		/// <summary>
		/// Gives a ratio of a number over 100.
		/// </summary>
		/// <param name="lvl"></param>
		/// <returns></returns>
		private static float ConvertLevelToDecimal(int lvl) {
			if (lvl < 1 || lvl > 100) {
				Debug.LogError("Level is not between 1 and 100!");
			}
			return (float)lvl / 100f;
		}
		/// <summary>
		/// Ceils a float between 0 and 1, multiplied by 100.
		/// </summary>
		/// <param name="dec"></param>
		/// <returns></returns>
		private static int ConvertDecimalToLevel(float dec) {
			if (dec < 0f || dec > 1f) {
				Debug.LogError("Decimal is not within 0 and 1!");
			}
			return Mathf.CeilToInt(dec * 100);
		}

		public static float ClampAngle(float angle, float min, float max) {
			// http://answers.unity3d.com/questions/574457/limit-rotation-using-mathclamp.html
			angle = Mathf.Repeat(angle, 360);
			min = Mathf.Repeat(min, 360);
			max = Mathf.Repeat(max, 360);
			bool inverse = false;
			var tmin = min;
			var tangle = angle;
			if (min > 180) {
				inverse = !inverse;
				tmin -= 180;
			}
			if (angle > 180) {
				inverse = !inverse;
				tangle -= 180;
			}
			var result = !inverse ? tangle > tmin : tangle < tmin;
			if (!result)
				angle = min;
			inverse = false;
			tangle = angle;
			var tmax = max;
			if (angle > 180) {
				inverse = !inverse;
				tangle -= 180;
			}
			if (max > 180) {
				inverse = !inverse;
				tmax -= 180;
			}

			result = !inverse ? tangle < tmax : tangle > tmax;
			if (!result)
				angle = max;
			return angle;
		}
	}


}*/
