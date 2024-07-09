using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    private GameObject _videoPlayerButtons;
    private Button _returnButton;
    //private Button _pauseButton;
    //private Button _resumeButton;

    private void Start()
    {
        // cache the reference from the Video Player UI
        _videoPlayerButtons = VideoPlayerUI.Instance.VideoPlayerButtons;
        //_returnButton = VideoPlayerUI.Instance.ResumeButton;
        //_pauseButton = VideoPlayerUI.Instance.PauseButton;
        //_resumeButton = VideoPlayerUI.Instance.ResumeButton;
        // disable the buttons on start
        _videoPlayerButtons.SetActive(false);
    }

}
