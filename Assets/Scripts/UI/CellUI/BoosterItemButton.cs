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

    public string ItemName { get; set; }

    public void Init()
    {
        //아무것도 없는 상태로 만들어 줘야한다.

        onClick.RemoveAllListeners();
    }

    public void SetButtonByIndex(string itemName)
    {
        ItemName = itemName;
        // 테스트용 데이터 번호로 사용하기
        mItemNameText.text = itemName;
    }
}
