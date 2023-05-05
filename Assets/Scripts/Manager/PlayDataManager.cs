using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayDataManager : Singleton<PlayDataManager>
{
    /// <summary>
    /// 폴더이름
    /// 처음에 생각한것이 Map에 생성 정보나 이런게 결합되어 있어서인데
    /// 컨트롤해야할 여지가 생김
    /// </summary>
    public string ConceptName;

    /// <summary>
    /// 맵 파일 이름
    /// </summary>
    public string MapName;

    /// <summary>
    /// 챕터 번호 
    /// 아웃 게임에서 ChapterNumber를 설정하고
    /// 인게임에서 이 값으로 적절한 행동을 취한다.
    /// </summary>
    public int ChapterNumber;

    /// <summary>
    /// 챕터 기믹
    /// 맵 기믹에 대한 정보
    /// </summary>
    public MapGimmickInfo ChapterMapGimmickInfo;
}
