using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

public class MissionCreateEditSceneManager : MonoBehaviour
{
    private const int MAX_COUNT = 5;

    private string[] mBlockImageNameArr = {
    "NormalBlock_{0}",
    "HomingExplosionBlock_{0}",
    "VerticalBombBlock_{0}",
    "HorizontalBombBlock_{0}",
    "AroundBombBlock_{0}",
    "ColorExplosionBlock"};

    [SerializeField] private Image[] mBlockImages = new Image[MAX_COUNT];
    [SerializeField] private Sprite mNullImage;

    [SerializeField] private SpriteAtlas mGameAtlas;

    public void OnRollButtonClicked()
    {
        foreach(var image in mBlockImages)
        {
            image.sprite = mNullImage;
        }

        int randomCount = Random.Range(1, MAX_COUNT + 1);

        for(int idx = 0; idx < randomCount; ++idx)
        {
            int randomColor = Random.Range(0, 5);
            int randomType = Random.Range(0, mBlockImageNameArr.Length);

            var sprite = mGameAtlas.GetSprite(string.Format(mBlockImageNameArr[randomType],randomColor));
            mBlockImages[idx].sprite = sprite;
            mBlockImages[idx].SetNativeSize();
        }
    }
}
