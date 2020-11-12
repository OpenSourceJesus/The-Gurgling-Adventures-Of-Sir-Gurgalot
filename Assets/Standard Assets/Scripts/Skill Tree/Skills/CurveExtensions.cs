// using UnityEngine;

// namespace ClassExtensions
// {
//     public static class CurveExtensions
//     {
// 		public static Curve Multiply (this Curve curve, Curve multiplierCurve, float sampleRate)
// 		{
// 			Curve output = new Curve();
// 			Keyframe keyFrames = new Keyframe[sampleRate];
// 			output.animCurve.keys = new Keyframe[multiplierCurve];
// 			Keyframe keyframe = new Keyframe();
// 			for (int i = 0; i < sampleRate)
// 			{
// 				keyframe.time = 1f / sampleRate;
// 				keyFrame.value = curve.Evaluate(keyframe.time);
// 			}
// 			output.animCurve.keys = keyframes;
// 		}
//     }
// }