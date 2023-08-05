using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WBTWeen
{
    public static class WBTween
    {
        public static AnimationCurve[] EaseCurveArr =
        {
            new AnimationCurve( new Keyframe(0,0), new Keyframe(1,1) )
        };

        #region Transform Tween

        /// <summary>
        /// Move [World, CurrentFrom]
        /// </summary>
        public static BTTransformTween Move(this Transform transform, Vector3 to, float duration)
        {
            var tween = BTTransformTween.GetTween(transform);
            tween.TweenType = BTTransformTween.ETweenType.Move;
            tween.From = transform.position;
            tween.To = to;
            tween.Duration = duration;
            tween.Play();
            return tween;
        }

        /// <summary>
        ///  Move [Local, CurrentFrom]
        /// </summary>
        public static BTTransformTween MoveLocal(this Transform transform, Vector3 to, float duration)
        {
            var tween = BTTransformTween.GetTween(transform);
            tween.TweenType = BTTransformTween.ETweenType.Move_Local;
            tween.From = transform.localPosition;
            tween.To = to;
            tween.Duration = duration;
            tween.Play();
            return tween;
        }

        /// <summary>
        /// Scale [Local, CurrentFrom]
        /// </summary>
        public static BTTransformTween Scale(this Transform transform, Vector3 to, float duration)
        {
            var tween = BTTransformTween.GetTween(transform);
            tween.TweenType = BTTransformTween.ETweenType.Scale;
            tween.From = transform.localScale;
            tween.To = to;
            tween.Duration = duration;
            tween.Play();

            return tween;
        }
        public static BTTransformTween Scale(this Transform transform, Vector3 from, Vector3 to, float duration)
        {
            var tween = BTTransformTween.GetTween(transform);
            tween.TweenType = BTTransformTween.ETweenType.Scale;
            tween.From = from;
            tween.To = to;
            tween.Duration = duration;
            tween.Play();

            return tween;
        }

        #endregion

        #region AudioSourceTween

        public static BTAudioSourceTween Volum(this AudioSource source, float from, float to, float duration)
        {
            var tween = BTAudioSourceTween.GetTween(source);
            tween.From = from;
            tween.To = to;
            tween.TweenType = BTAudioSourceTween.EAudioTweenType.Volume;
            tween.Duration = duration;
            tween.Play();

            return tween;
        }
        public static BTAudioSourceTween Pitch(this AudioSource source, float from, float to, float duration)
        {
            var tween = BTAudioSourceTween.GetTween(source);
            tween.From = from;
            tween.To = to;
            tween.TweenType = BTAudioSourceTween.EAudioTweenType.Pitch;
            tween.Duration = duration;
            tween.Play();

            return tween;
        }

        #endregion
    }
}
