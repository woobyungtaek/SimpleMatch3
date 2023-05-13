using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoosterItemButton : Button
{
    [Header("BoosterItem")]
    [SerializeField] private Image mItemImage;
    [SerializeField] private TextMeshProUGUI mItemNameText;
    [SerializeField] private int mInventoryIndex;

    public int GetInvenIndex
    {
        get => mInventoryIndex;
    }

    public void Init()
    {
        mInventoryIndex = -1;
        SetButtonByIndex(mInventoryIndex);

        onClick.RemoveAllListeners();
    }

    public void SetButtonByIndex(int invenIdx)
    {
        mInventoryIndex = invenIdx;
        if (mInventoryIndex < 0)
        {
            mItemNameText.text = "Select Item";
            return;
        }
        
        // 테스트용 데이터 번호로 사용하기
        mItemNameText.text = $"{invenIdx}";

        // 인벤토리로 부터 BoosterItem의 데이터 Index를 알아와서
        // 데이터의 이름을 넣어줘야함
    }
}
