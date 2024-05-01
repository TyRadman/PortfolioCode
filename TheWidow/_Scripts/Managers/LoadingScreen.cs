using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public GameObject LoadingScreenMenu;
    public Image LoadingBar;
    public Text LoadingText;
    public Image TipImage;
    public Text TipText;

    public void StartLoadingScreen(int _scene)
    {
        MainMenuManager.CursorState(false);
        LoadingScreenMenu.SetActive(true);
        var tip = SharedManager.Instance.GetRandomTip();
        TipImage.sprite = tip.Image;
        TipText.text = tip.Info;

        StartCoroutine(loadingTheScene(_scene));
    }
    IEnumerator loadingTheScene(int _scene)
    {
        yield return new WaitForSeconds(0.9f);
        AsyncOperation operation = SceneManager.LoadSceneAsync(_scene);
        while (operation.isDone == false)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            LoadingText.text = $"Loading { progress * 100f:0} %";
            LoadingBar.fillAmount = progress;

            yield return null;
        }
    }
}