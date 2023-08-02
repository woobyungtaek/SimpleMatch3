using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WBTWeen
{
    public static class WBTween
    {
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
    }
}
