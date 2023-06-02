using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
    private const float FADE_DURATION = 1f;
    private static WaitForSecondsRealtime FadeWaitForSecond = new WaitForSecondsRealtime(FADE_DURATION);

    public WaitForSecondsRealtime FadeSecond
    {
        get { return FadeWaitForSecond; }
    }
 
    [SerializeField] private Image          mDimmedImage;
    [SerializeField] private CanvasGroup    mCanvasGroup;
    [SerializeField] private Slider         mLoadingBar;

    [SerializeField] private string mLoadSceneName;
    
    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        if (!IsExist)
        {
            Instantiate(Resources.Load("Prefabs/SceneLoader"));
        }
        Instance.gameObject.SetActive(false);
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void LoadSceneByName(string name)
    {
        gameObject.SetActive(true);
        mLoadSceneName = name;
        SceneManager.sceneLoaded += LoadSceneEnd;
        StartCoroutine(LoadScene(mLoadSceneName));
    }
    public void FadeInOutByExternal(bool bFadeIn)
    {
        gameObject.SetActive(true);
        StartCoroutine(FadeEffect(bFadeIn));
    }

    private void LoadSceneEnd(Scene scene,LoadSceneMode loadSceneMode)
    {
        if(scene.name.Equals(mLoadSceneName))
        {
            StartCoroutine(FadeEffect(false));
            SceneManager.sceneLoaded -= LoadSceneEnd;
        }
    }
    private IEnumerator LoadScene(string sceneName)
    {
        ObjectPool.ClearAll();
        GameObjectPool.ClearPool();

        mLoadingBar.value = 0;
        mLoadingBar.gameObject.SetActive(true);

        yield return StartCoroutine(FadeEffect(true));

        AsyncOperation asyncOP =  SceneManager.LoadSceneAsync(sceneName);

        //False상태라면 0.9진행도에서 Fake로 돌게 된다.
        //준비된 장면이 활성화 되는 것을 허용
        asyncOP.allowSceneActivation = false;

        float time = 0f;

        while(!asyncOP.isDone)
        {
            yield return null;
            time += Time.unscaledDeltaTime;

            if(asyncOP.progress < 0.9f)
            {
                mLoadingBar.value = Mathf.Lerp(mLoadingBar.value, asyncOP.progress, time);
                if(mLoadingBar.value >= asyncOP.progress) { time = 0; }
            }
            else
            {
                mLoadingBar.value = Mathf.Lerp(asyncOP.progress,1f, time);
                if(mLoadingBar.value >= 1f)
                {
                    asyncOP.allowSceneActivation = true;
                    mLoadingBar.gameObject.SetActive(false);
                    yield return StartCoroutine(FadeEffect(false));
                    yield break; 
                }
            }
        }

    }
    private IEnumerator FadeEffect(bool bFadeIn)
    {
        float time = 0;
        while(time <= 1f)
        {
            yield return null;
            time += Time.unscaledDeltaTime / FADE_DURATION;
            mCanvasGroup.alpha = Mathf.Lerp(bFadeIn ? 0 : 1, bFadeIn ? 1 : 0, time);
        }
        if(!bFadeIn)
        {
            gameObject.SetActive(false);
        }
    }
}
