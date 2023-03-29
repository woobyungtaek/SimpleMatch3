using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[ExecuteInEditMode]
public class FlowImage_UI : MonoBehaviour
{
    [Range(0f, 100f)] [SerializeField] private float mTileX;
    [Range(0f, 100f)] [SerializeField] private float mTileY;
    [Range(0f, 100f)] [SerializeField] private float mSpeedX;
    [Range(0f, 100f)] [SerializeField] private float mSpeedY;

    private Material mMat;

    private void Start()
    {
        var image = GetComponent<Image>();
        var mat = image.material;
        image.material = Instantiate(mat);
        mMat = image.material;

        mMat.SetFloat("_TileX", mTileX);
        mMat.SetFloat("_TileY", mTileY);
        mMat.SetFloat("_SpeedX", mSpeedX);
        mMat.SetFloat("_SpeedY", mSpeedY);
    }

    private void Update()
    {
        Shader.SetGlobalFloat("_UnscaledTime", Time.unscaledTime);
    }
}
