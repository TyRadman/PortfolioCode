using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class PlayerEffects : MonoBehaviour
{
    public static PlayerEffects Instance;
    [SerializeField] private float m_ExhaustionPercentageStart = 0.9f;
    [SerializeField] private Volume m_Volume;
    [SerializeField] private Volume m_BlurringVolume;
    [Header("Depth of field variables")]
    [SerializeField] private bool m_EnableDepthOfField = true;
    [SerializeField] private float m_ClarityDistance = 1f;
    [SerializeField] private float m_NearBlurDistance;
    [SerializeField] private float m_FarBlurDistance;
    [SerializeField] private float m_BlendingSpeed = 5f;
    private float distance;
    private DepthOfField depth;
    private bool m_IsPaused = false;
    [Header("Exhaustion Variables")]
    [Header("Vignette")]
    [SerializeField] private float m_NormalIntensity;
    [SerializeField] private float m_MaxIntensity;
    [SerializeField] private float m_NormalSmoothness;
    [SerializeField] private float m_MaxSmoothness;
    private Vignette m_Vignette;
    [Header("Color Adjustments")]
    [SerializeField] private float m_NormalPostExposure;
    [SerializeField] private float m_MaxPostExposure;
    [SerializeField] private float m_NormalContrast;
    [SerializeField] private float m_MaxContrast;
    [SerializeField] private float m_NormalSaturation;
    [SerializeField] private float m_MaxSaturation;
    private ColorAdjustments m_Adjustments;
    [Header("Motion Blur")]
    [SerializeField] private float m_NormalMotionBlurIntensity;
    [SerializeField] private float m_MaxMotionBlurIntensity;
    private MotionBlur m_MotionBlur;
    [Header("Lens Distortion")]
    [SerializeField] private float m_NormalLensIntensity;
    [SerializeField] private float m_MaxLensIntensity;
    private LensDistortion m_LensDistortion;
    [Header("Chromatic Aberration")]
    [SerializeField] private float m_NormalChromaticIntensity;
    [SerializeField] private float m_MaxChromaticIntensity;
    private ChromaticAberration m_ChromaticAberration;
    [Header("Film Grain")]
    [SerializeField] private float m_NormalFilmIntensity;
    [SerializeField] private float m_MaxFilmIntensity;
    private FilmGrain m_FilmGrain;


    private void Awake()
    {
        Instance = this;
        m_BlurringVolume.profile.TryGet(out depth);
        m_BlurringVolume.profile.TryGet(out m_Vignette);
        m_BlurringVolume.profile.TryGet(out m_Adjustments);
        m_BlurringVolume.profile.TryGet(out m_MotionBlur);
        m_BlurringVolume.profile.TryGet(out m_LensDistortion);
        m_BlurringVolume.profile.TryGet(out m_ChromaticAberration);
        m_BlurringVolume.profile.TryGet(out m_FilmGrain);
        // we need the half value because we need its divided value more than its real one
        m_ClarityDistance /= 2;

        if (!m_EnableDepthOfField)
        {
            depth.active = false;
        }
    }



    private void Update()
    {
        if (m_IsPaused)
        {
            return;
        }

        // if the player's stamina goes low, a different postprocessing effect starts to slowly taking place depending on how much stamina is left to indicate exhaustion
        if (PlayerStats.Instance.GetStaminaPercentage() < m_ExhaustionPercentageStart)
        {
            // we interpolate between both volumes, the normal and the exhausted one
            float t = Mathf.InverseLerp(m_ExhaustionPercentageStart, 0f, PlayerStats.Instance.GetStaminaPercentage());
            applyExhaustionScreen(t);
        }

        if (m_EnableDepthOfField)
        {
            applyDepthOfField();
        }
    }

    #region Exhaustion
    private void applyExhaustionScreen(float _prct)
    {
        // vignette
        m_Vignette.intensity.value = Mathf.Lerp(m_NormalIntensity, m_MaxIntensity, _prct);
        m_Vignette.smoothness.value = Mathf.Lerp(m_NormalSmoothness, m_MaxSmoothness, _prct);
        // color adjustments
        m_Adjustments.postExposure.value = Mathf.Lerp(m_NormalPostExposure, m_MaxPostExposure, _prct);
        m_Adjustments.contrast.value = Mathf.Lerp(m_NormalContrast, m_MaxContrast, _prct);
        m_Adjustments.saturation.value = Mathf.Lerp(m_NormalSaturation, m_MaxSaturation, _prct);
        // motion blur
        m_MotionBlur.intensity.value = Mathf.Lerp(m_NormalMotionBlurIntensity, m_MaxMotionBlurIntensity, _prct);
        // lens distortion
        m_LensDistortion.intensity.value = Mathf.Lerp(m_NormalLensIntensity, m_MaxLensIntensity, _prct);
        // chromatic aberration
        m_ChromaticAberration.intensity.value = Mathf.Lerp(m_NormalChromaticIntensity, m_MaxChromaticIntensity, _prct);
        // film grain
        m_FilmGrain.intensity.value = Mathf.Lerp(m_NormalFilmIntensity, m_MaxFilmIntensity, _prct);
    }
    #endregion

    #region Blur
    // the depth of field changes dynamically according to where the player is looking
    private void applyDepthOfField()
    {
        // if there is change in what the player sees then there is no need to do any unnecessary calculations
        if(distance == PlayerSight.DistanceFromPlayer)
        {
            return;
        }

        // we make a transition from the previous blur values to the new one with a speed that we control
        distance = Mathf.Lerp(distance, PlayerSight.DistanceFromPlayer, Time.deltaTime * m_BlendingSpeed);
        // calculating near blur
        depth.nearFocusEnd.value = distance - m_ClarityDistance;
        depth.nearFocusStart.value = distance - m_ClarityDistance - m_NearBlurDistance;
        // calculating far blur
        depth.farFocusStart.value = distance + m_ClarityDistance;
        depth.farFocusEnd.value = distance + m_ClarityDistance + m_FarBlurDistance;
    }

    public void BlurAllScreen(bool _pause)
    {
        m_IsPaused = _pause;

        // blur the screen
        if (_pause)
        {
            distance = 100f;
            // calculating near blur
            depth.nearFocusEnd.value = distance - m_ClarityDistance;
            depth.nearFocusStart.value = distance - m_ClarityDistance - m_NearBlurDistance;
            // calculating far blur
            depth.farFocusStart.value = distance + m_ClarityDistance;
            depth.farFocusEnd.value = distance + m_ClarityDistance + m_FarBlurDistance;
        }
    }
    #endregion
}