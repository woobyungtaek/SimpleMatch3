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
        //�ƹ��͵� ���� ���·� ����� ����Ѵ�.

        onClick.RemoveAllListeners();
    }

    public void SetButtonByName(string itemName)
    {
        ItemName = itemName;

        if (string.IsNullOrEmpty(itemName))
        {
            mItemNameText.text = "Item\nSelect";
        }
        else
        {
            mItemNameText.text = itemName;
        }
    }
}
