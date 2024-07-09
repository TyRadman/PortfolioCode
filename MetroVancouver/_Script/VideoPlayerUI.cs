using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoPlayerUI : Singleton<VideoPlayerUI>
{
    public GameObject VideoPlayerButtons;
    public Button ReturnButton;
    [SerializeField] private Transform _hand;
    [SerializeField] private Animation _ButtonsAnim;
    [SerializeField] private AnimationClip _showClip;
    [SerializeField] private AnimationClip _hideClip;
    [SerializeField] private Image _progressBar;
    public bool IsActive = true;

    private void Start()
    {
        EnableUI(false);
    }

    private void Update()
    {
        if (!IsActive)
        {
            return;
        }

        float z = _hand.eulerAngles.z;

        if(z > 90f && z < 270 || z > -90 && z < -270)
        {
            ShowButtons();
        }
        else
        {
            HideButtons();
        }
    }

    private void ShowButtons()
    {
        if(_ButtonsAnim.clip == _showClip)
        {
            return;
        }

        _ButtonsAnim.clip = _showClip;
        _ButtonsAnim.Play();
    }

    public void HideButtons()
    {
        if (_ButtonsAnim.clip == _hideClip)
        {
            return;
        }

        _ButtonsAnim.clip = _hideClip;
        _ButtonsAnim.Play();
    }

    public void SetActiveVideoControllerButtons(bool enable)
    {
        VideoPlayerButtons.SetActive(enable);
    }

    public void EnableReturnButton(bool enable)
    {
        ReturnButton.interactable = enable;
    }

    public void EnableUI(bool enable)
    {
        VideoPlayerButtons.SetActive(enable);
    }

    public void UpdateProgressBar(float amount)
    {
        _progressBar.fillAmount = amount;
    }
}
