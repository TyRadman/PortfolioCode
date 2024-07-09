using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Video;

public class Orb : MonoBehaviour
{
    [SerializeField] private MeshRenderer _orbMesh;
    private Material _material;
    public const string TEXTURE_REFERENCE_ID = "_BaseMap";
    [SerializeField] private Video_SO _video;
    [SerializeField] private TextMeshPro _nameText;
    [SerializeField] private TextMeshPro _watchedText;
    [SerializeField] private VideoPlayer _videoPlayer;
    private List<bool> _hands = new List<bool>(2) { false, false };
    private int _index = 0;
    [field: SerializeField] public bool IsWatched = false;
    private const string COLOR_KEY = "_BaseColor";
    private const string ALPHA_KEY = "_Alpha";
    private const float DISABLED_ALPHA = 0.85f;

    private void Awake()
    {
        _watchedText.enabled = false;
        _material = _orbMesh.material;
        _nameText.gameObject.SetActive(false);
    }

    private void Start()
    {
        SetUp();
    }

    private void SetUp()
    {
        if (_video.PreviewImage != null)
        {
            _material.SetTexture(TEXTURE_REFERENCE_ID, _video.PreviewImage.texture);
        }

        _nameText.text = _video.VideoName;
    }

    [ContextMenu("Play Video")]
    public void OnOrbSelected()
    { 
        VideoPlayerManager.Instance.PlayVideo(_video);
        IsWatched = true;
    }

    public void OnHovered()
    {
        _hands[_index++] = true;
        _nameText.gameObject.SetActive(true);
    }

    public void OnUnhovered()
    {
        _hands[--_index] = false;
        _nameText.gameObject.SetActive(_index != 0);
    }

    public void OnOrbWatched(Color disabledColor)
    {
        IsWatched = false;
        _orbMesh.material.SetColor(COLOR_KEY, disabledColor);
        _orbMesh.material.SetFloat(ALPHA_KEY, DISABLED_ALPHA);
        _watchedText.enabled = true;
    }
}
