using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[ExecuteInEditMode]
public class FlowImage : MonoBehaviour
{
    [Range(0f, 100f)] [SerializeField] private float mTileX;
    [Range(0f, 100f)] [SerializeField] private float mTileY;
    [Range(0f, 100f)] [SerializeField] private float mSpeedX;
    [Range(0f, 100f)] [SerializeField] private float mSpeedY;

    private Material mMat;

    private void Start()
    {
        var renderer = GetComponent<SpriteRenderer>();
        var mat = renderer.material;
        renderer.material = Instantiate(mat);
        renderer.material.name = mat.name;
        mMat = renderer.material;

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
