using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class CanvasScaleControler : MonoBehaviour
{
    private CanvasScaler mCurrentScaler;

    private void Awake()
    {
        if(mCurrentScaler == null)
        {
            mCurrentScaler = GetComponent<CanvasScaler>();
        }

        float currentAspect = Camera.main.aspect;
        Vector2 refResolution = mCurrentScaler.referenceResolution;
        float referenceAspect = refResolution.x / refResolution.y;

        if (currentAspect > referenceAspect)
        {
            mCurrentScaler.matchWidthOrHeight = 1;
        }
        else
        {
            mCurrentScaler.matchWidthOrHeight = 0;
        }
    }
}
