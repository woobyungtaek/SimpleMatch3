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

    public void SetButtonByIndex(string itemName)
    {
        ItemName = itemName;
        // �׽�Ʈ�� ������ ��ȣ�� ����ϱ�
        mItemNameText.text = itemName;
    }
}
