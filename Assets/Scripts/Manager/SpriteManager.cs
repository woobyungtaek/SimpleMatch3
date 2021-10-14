using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SpriteManager : SceneSingleton<SpriteManager>
{
    [SerializeField] private SpriteAtlas GameAtlas;
    [SerializeField] private SpriteAtlas UIAtlas;

    public Sprite GetBlockSpriteByBlockName(string blockName)
    {
        return GameAtlas.GetSprite(blockName);
    }
    public Sprite GetUISpriteByName(string name)
    {
        return UIAtlas.GetSprite(name);
    }
}
