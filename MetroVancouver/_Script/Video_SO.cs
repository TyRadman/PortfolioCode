using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "Video_VIDEONAME_0", menuName = "360 Video")]
public class Video_SO : ScriptableObject
{
    public string VideoName;
    public RenderTexture RenderTexture;
    public Sprite PreviewImage;
    public VideoSource Source;
    [Header("Video")]
    public VideoClip Video;
    [Header("URL")]
    public string VideoURL;
}