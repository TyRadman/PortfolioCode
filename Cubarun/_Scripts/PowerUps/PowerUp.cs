using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class PowerUp : MonoBehaviour
{
    [SerializeField] private int powerUpIndex;
    [SerializeField] protected float powerUpEffectDuration = 0f;    // after this duration, the power up will start recharging
    [SerializeField] private float rechargeDuration;
    [SerializeField] private string soundEffectName;
    private Animator anim;
    private bool canBeUsed = true;
    private Image powerUpBar;

    private void Start()
    {
        PowerUpsManager.Instance.AddPowerUp(this);
    }

    public void Activate()
    {
        if (!canBeUsed)
            return;

        anim.SetBool("Full", false);
        canBeUsed = false;
        Effect();
        StartCoroutine(effectCountDown());
    }

    protected void Deactivate()
    {
        Deeffect();
        StartCoroutine(rechargeCountDown());
    }

    protected virtual void InstantDeactiviation()
    {

    }

    public virtual void Effect()
    {

    }

    public virtual void Deeffect()
    {

    }

    IEnumerator effectCountDown()
    {
        float time = powerUpEffectDuration;

        while (time > 0)
        {
            time -= Time.deltaTime;
            powerUpBar.fillAmount = time / powerUpEffectDuration;
            yield return null;
        }

        Deactivate();
    }

    IEnumerator rechargeCountDown()
    {
        float time = 0;
        while (time < rechargeDuration)
        {
            time += Time.deltaTime;
            powerUpBar.fillAmount = time / rechargeDuration;
            yield return null;
        }
        canBeUsed = true;
    }

    public int GetIndex()
    {
        return powerUpIndex;
    }

    public void SetUIBar(Image bar, Animator buttonAnimator)
    {
        powerUpBar = bar;
        anim = buttonAnimator;
    }

    public void FullyCharge()
    {
        StopAllCoroutines();
        Time.timeScale = 1f;
        InstantDeactiviation();
        powerUpBar.fillAmount = 1f;
        canBeUsed = true;
    }
}
