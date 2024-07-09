using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Esri.ArcGISMapsSDK.Components;

[RequireComponent(typeof(VideoPlayer))]
public class VideoPlayerManager : MonoBehaviour
{
    public static VideoPlayerManager Instance;
    private VideoPlayer _videoPlayer;
    [SerializeField] private Color _disabledColor;
    [SerializeField] private GameObject _minimapGameObject;
    [SerializeField] private ArcGISMapComponent _arcGISMap;
    [Header("Sky boxes")]
    [SerializeField] private Material _videoSkyBoxMaterial;
    [SerializeField] private Material _defaultSkyBoxMaterial;
    [SerializeField] private Material _blackSkyBox;
    [Header("Animations")]
    [SerializeField] private TransitionController _screenOverlay;
    private WaitForSeconds _showFadeClipHalfWait;
    private bool _isTransitioning = false;
    public bool VideoIsPlaying { private set; get; } = false;
    public bool SkipOrbs{ set; get; } = false;

    private void Awake()
    {
        // singlton reference
        Instance = this;

        // cache the video player
        _videoPlayer = GetComponent<VideoPlayer>();

        // create new WaitForSeconds to avoid creating them in run-time
        _showFadeClipHalfWait = new WaitForSeconds(_screenOverlay.fadeInClip.length);
    }

    #region Play the video
    public void PlayVideo(Video_SO video)
    {
        // if there is a transition taking place already, then ignore any extra input
        if (_isTransitioning)
        {
            return;
        }

        // if there is no video file passed, then return
        if (video == null)
        {
            Debug.LogError("Missing video clip reference");
            return;
        }

        // start the process of displaying the video
        StartCoroutine(PlayVideoProcess(video));
    }

    private IEnumerator PlayVideoProcess(Video_SO video)
    {
        _isTransitioning = true;
        VideoIsPlaying = true;

        // assign the video clip as the video source
        _videoPlayer.clip = video.Video;

        // show the screen overlay for the transition and wait until it finishes
        _screenOverlay.FadeIn();
        yield return _showFadeClipHalfWait;

        // disable all the orbs
        OrbsManager.Instance.EnableOrbs(false);
        // set the rendering texture to the skybox material
        _videoSkyBoxMaterial.SetTexture(Orb.TEXTURE_REFERENCE_ID, video.RenderTexture);
        RenderSettings.skybox = _videoSkyBoxMaterial;

        // disable the return button
        VideoPlayerUI.Instance.EnableReturnButton(true);
        // disable minimap
        _minimapGameObject.SetActive(false);
        // disable arcGIS map
        _arcGISMap.enabled = false;

        // play the video
        _videoPlayer.Play();
        StartCoroutine(OnVideoPlayProcess((float)video.Video.length));


        _screenOverlay.FadeOut();
        yield return _showFadeClipHalfWait;

        _isTransitioning = false;
        VideoPlayerUI.Instance.EnableUI(true);
    }

    private IEnumerator OnVideoPlayProcess(float duration)
    {
        float time = 0f;

        while (time <= duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            VideoPlayerUI.Instance.UpdateProgressBar(t);
            yield return null;
        }

        ReturnToOrbsView();
    }
    #endregion

    #region Stop the video (Return to orbs selection)
    public void ReturnToOrbsView()
    {
        StopAllCoroutines();
        _videoPlayer.clip = null;
        _videoPlayer.Stop();

        VideoPlayerUI.Instance.EnableReturnButton(false);
        VideoPlayerUI.Instance.HideButtons();
        VideoPlayerUI.Instance.IsActive = false;

        OrbsManager.Instance.SelectedOrbsGroup.Orbs.FindAll(o => o.IsWatched).ForEach(o => o.OnOrbWatched(_disabledColor));
        StartCoroutine(StopVideoProcess());
    }

    private IEnumerator StopVideoProcess()
    {
        if (SkipOrbs)
        {
            VideoIsPlaying = false;
            StopAllCoroutines();
            yield break;
        }

        // show the screen overlay for the transition
        _screenOverlay.FadeIn();
        yield return _showFadeClipHalfWait;

        // display all the orbs
        OrbsManager.Instance.EnableOrbs(true);

        // hide the UI buttons 
        VideoPlayerUI.Instance.SetActiveVideoControllerButtons(false);

        // assign the video material to the sky box
        RenderSettings.skybox = _defaultSkyBoxMaterial;

        _minimapGameObject.SetActive(true);
        _arcGISMap.enabled = true;
        _screenOverlay.FadeOut();
        yield return _showFadeClipHalfWait;
        VideoPlayerUI.Instance.IsActive = true;
        // play the video
        _videoPlayer.Stop();
        VideoIsPlaying = false;
    }
    #endregion

    public void StartEndExperienceCountDown()
    {
        StartCoroutine(ExitingProcess());
    }

    [SerializeField] private GameObject _endScreen;

    private IEnumerator ExitingProcess()
    {
        _screenOverlay.FadeIn();
        yield return _showFadeClipHalfWait;
        RenderSettings.skybox = _blackSkyBox;
        OrbsManager.Instance.EnableOrbs(false);
        _endScreen.SetActive(true);
        _screenOverlay.FadeOut();
        yield return _showFadeClipHalfWait;
    }
}
