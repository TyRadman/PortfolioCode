using System.Collections;
using UnityEngine;

public class SlowMotionPowerUp : PowerUp
{
    [SerializeField] [Range(0.05f, 0.8f)] private float minTimeScale = 0.3f;

    public override void Effect()
    {
        base.Effect();

        StartCoroutine(toSlowMotion());
    }

    public override void Deeffect()
    {
        base.Deeffect();

        StartCoroutine(fromSlowMotion());
    }

    private IEnumerator toSlowMotion()
    {
        float tempValue = 1f;

        while (Time.timeScale > minTimeScale)
        {
            Time.timeScale -= Time.deltaTime;

            tempValue = Mathf.Lerp(1f, 1f / minTimeScale, minTimeScale / Time.timeScale);
            PlayerEffects.Instance.SlowMotionEffect(tempValue);
            yield return null;
        }

        Time.timeScale = minTimeScale;
    }

    IEnumerator fromSlowMotion()
    {
        PlayerEffects.Instance.SlowMotionEffect(1);

        while (Time.timeScale < 1f)
        {
            Time.timeScale += Time.deltaTime / 2f;
            yield return null;
        }

        Time.timeScale = 1f;
    }
}
