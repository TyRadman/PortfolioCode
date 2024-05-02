using System.Collections;
using UnityEngine;

public static class GameObjectsFunctions
{
    private static HelpingGameObjectFunctions entity;
    /// <summary>
    /// This only applies to gameObject that have UI elemnts
    /// </summary>
    /// <param name="gameObj"></param>
    /// <param name="fadeDuration">How long it takes the screen to fade in or out</param>
    /// <param name="fadeIn">Determines whether the game object should fade in or out. True will make it fade in</param>
    public static void FadeObject(this GameObject gameObj, float fadeDuration, bool fadeIn)
    {
        helpingFunctionsEntityCheck();
        var group = gameObj.GetComponent<CanvasGroup>() == null? gameObj.AddComponent<CanvasGroup>() : gameObj.GetComponent<CanvasGroup>();
        entity.FadingProcess(group, fadeDuration, fadeIn);
    }

    public static void SetActiveWithFade(this GameObject gameObj, float fadeDuration, bool active)
    {
        gameObj.SetActive(active);
        helpingFunctionsEntityCheck();
        CanvasGroup group;

        if (gameObj.GetComponent<CanvasGroup>() == null)
        {
            group = gameObj.AddComponent<CanvasGroup>();
        }
        else
        {
            group = gameObj.GetComponent<CanvasGroup>();
        }

        group.alpha = active? 0f : 1f;
        entity.FadingProcess(group, fadeDuration, active);
    }

    private static void helpingFunctionsEntityCheck()   // to make sure we have instantiated the helpingFunctions class somewhere in the scene to use monoBehaovour functions
    {
        if (entity == null)
        {
            GameObject newGameObj = new GameObject("Static Functions");
            entity = newGameObj.AddComponent<HelpingGameObjectFunctions>();
        }
    }
}

public class HelpingGameObjectFunctions : MonoBehaviour
{
    public void FadingProcess(CanvasGroup group, float duration, bool fadeIn)
    {
        StartCoroutine(fadingProcess(group, duration, fadeIn));
    }

    private IEnumerator fadingProcess(CanvasGroup group, float duration, bool fadeIn)
    {
        float time = 0f;
        float startValue = fadeIn ? 0 : 1;
        float endValue = fadeIn ? 1 : 0;

        while(time < duration)
        {
            time += Time.deltaTime;
            group.alpha = Mathf.Lerp(startValue, endValue, time / duration);
            yield return null;
        }
    }

}