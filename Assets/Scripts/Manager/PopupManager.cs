using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class PopupManager : SceneSingleton<PopupManager>
{
    [SerializeField] public Transform PopupTransform;

    [SerializeField] public GameObject StageSuccessPopup;
    [SerializeField] public GameObject StageFailPopup;
    [SerializeField] public GameObject DayClearPopup;
    [SerializeField] public GameObject AllClearPopup;

    [SerializeField] public GameObject PausePopup;

    [SerializeField] public GameObject TutoEndPopup;
}
