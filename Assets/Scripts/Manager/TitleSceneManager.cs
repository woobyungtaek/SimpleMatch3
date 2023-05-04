using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSceneManager : MonoBehaviour
{
    public void OnClickedStartButton()
    {
        SceneLoader.Instance.LoadSceneByName("LobbyScene");
    }
}
