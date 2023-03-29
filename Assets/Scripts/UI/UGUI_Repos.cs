using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class UGUI_Repos : MonoBehaviour
{
    private RectTransform mCurrentRectTransform;

    [SerializeField] private RectTransform mTargetRect;
    [SerializeField] private Vector3 mRePosVec;

    private void OnEnable()
    {
        if(mCurrentRectTransform == null)
        {
            mCurrentRectTransform = gameObject.transform as RectTransform;
        }
    }
}
