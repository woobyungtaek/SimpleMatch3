using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Day Clear가 아니라 Break Time으로
    하루 장사를 오전, 오후, 저녁으로 나누고
    오전 파트가 끝나면 보드가 갱시되는 시점, 즉 시간대별 장사가 완료된 것
    AllClear팝업이 DayClear가 된다.
 */

public class DayClearPopup : Popup
{
    public void OnCancelButtonClicked()
    {
        OnOkButtonClicked();
    }

    public void OnOkButtonClicked()
    {
        StartCoroutine(DelayFadeInEffect());
    }

    private IEnumerator DelayFadeInEffect()
    {
        Time.timeScale = 1;
        if (SceneLoader.IsExist)
        {
            SceneLoader.Instance.FadeInOutByExternal(true);
            yield return SceneLoader.Instance.FadeSecond;
        }
        yield return null;
        ClosePopup();
        PuzzleManager.Instance.ChangeCurrentGameStateWithNoti(EGameState.Loading);
    }
}
