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

    public BoosterItemData CurrentData { get; set; }

    public void Init()
    {
        //�ƹ��͵� ���� ���·� ����� ����Ѵ�.

        onClick.RemoveAllListeners();
    }

    public void SetButtonByItemIndex(BoosterItemData data)
    {
        CurrentData = data;

        if (CurrentData == null)
        {
            mItemNameText.text = "Item\nSelect";
        }
        else
        {
            Debug.Log($"Data Index_set {CurrentData.Index}");
            mItemNameText.text = CurrentData.ItemName;
        }
    }
    public void SetButtonText(string str)
    {
        mItemNameText.text = str;
    }
}
