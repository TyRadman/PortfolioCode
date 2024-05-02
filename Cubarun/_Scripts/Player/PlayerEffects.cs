using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffects : MonoBehaviour
{
    public static PlayerEffects Instance;
    private Color PlayerColor;
    public enum speedingStates
    {
        SpeedUp, SlowDown, Idle
    }
    private speedingStates _speedingState;
    private speedingStates speedingState
    {
        get
        {
            return _speedingState;
        }

        set
        {
            _speedingState = value;

            switch (value)
            {
                case speedingStates.Idle:
                    {
                        SpeedNormal();
                        break;
                    }
                case speedingStates.SlowDown:
                    {
                        SlowDown();
                        break;
                    }
                case speedingStates.SpeedUp:
                    {
                        SpeedUp();
                        break;
                    }
            }
        }
    }
    
    [Header("Trail")]
    [SerializeField] private TrailRenderer trail;
    [SerializeField] private float transitionDuration = 1f;
    [SerializeField] [Range(1, 15)] private float trailIntensity = 10f;
    private float trailOriginalWidth = 1f;
    [SerializeField] private float trailLength = 2;
    private Coroutine changingSizeCoroutine;
    
    [Header("Lines Particles")]
    [SerializeField] private ParticleSystem speedLinesParticles;
    [SerializeField] private float linesHightEmission = 70f;
    [SerializeField] private float linesMediumEmission = 30f;
    [SerializeField] private float linesLowEmission = 10;
    [SerializeField] private float linesHighSpeed;
    [SerializeField] private float linesMediumSpeed;
    [SerializeField] private float linesLowSpeed;

    private void Awake()
    {
        Instance = this;
        trail.time = 0f;
        trailOriginalWidth = trail.widthCurve.keys[0].value;
    }
    private void Start()
    {
        setUpTrailMaterial();
    }

    private void setUpTrailMaterial()
    {
        var material = PlayerInfoManager.Instance.GetTrailMaterial();

        if (material != null)
        {
            trail.material = material;
        }
        else
        {
            trail.material.EnableKeyword("_EMISSION");
            trail.material.SetColor("_EmissionColor", PlayerColor * trailIntensity);
        }
    }

    private void SpeedUp()
    {
        setLinesEmissionAndSpeed(linesHightEmission, linesHighSpeed);
        enableTrail(true);
    }


    private void SpeedNormal()
    {
        setLinesEmissionAndSpeed(linesMediumEmission, linesMediumSpeed);
        enableTrail(false);
    }

    private void SlowDown()
    {
        setLinesEmissionAndSpeed(linesLowEmission, linesLowSpeed);
    }

    #region Trail Functions
    public void SetColor(Color color)
    {
        PlayerColor = color;
    }

    private void enableTrail(bool enable)
    {
        if (changingSizeCoroutine != null)
        {
            StopCoroutine(changingSizeCoroutine);
            changingSizeCoroutine = null;
        }

        if (enable)
        {
            trail.enabled = true;
            changingSizeCoroutine = StartCoroutine(changeLength());
        }
        else
        {
            changingSizeCoroutine = StartCoroutine(shrink());
        }
    }

    IEnumerator shrink()
    {
        float time = 0f;
        float vanishTime = transitionDuration * 2f;

        while(time < vanishTime)
        {
            time += Time.deltaTime;
            trail.widthMultiplier = Mathf.Lerp(trail.widthMultiplier, 0f, time / vanishTime);

            yield return null;
        }

        trail.enabled = false;
        changingSizeCoroutine = null;
    }

    IEnumerator changeLength()
    {
        float time = 0f;

        while (time < transitionDuration)
        {
            time += Time.deltaTime;
            float t = time / transitionDuration;

            trail.time = Mathf.Lerp(trail.time, trailLength, t);
            trail.widthMultiplier = Mathf.Lerp(trail.widthMultiplier, trailOriginalWidth, t);

            yield return null;
        }

        trail.time = trailLength;
        changingSizeCoroutine = null;
    }
    #endregion

    public void SlowMotionEffect(float scale)
    {
        playerMovement.Instance.ChangeTurnSpeed(scale);
        CubeAnimator.Instance.ChangeShrinkSpeed(scale);
    }

    #region Speeding lines Particles functions
    private void setLinesEmissionAndSpeed(float emission, float speed)
    {
        var theEmission = speedLinesParticles.emission;
        theEmission.rateOverTime = emission;

        var main = speedLinesParticles.main;
        main.startSpeed = speed;
    }
    #endregion

    public void SpeedStateChange(speedingStates state)
    {
        speedingState = state;
    }

    public void StartEffects()
    {
        SpeedNormal();
    }

    public void TurnOffEffects()
    {
        enableTrail(false);
        setLinesEmissionAndSpeed(0, 0);
    }
}
