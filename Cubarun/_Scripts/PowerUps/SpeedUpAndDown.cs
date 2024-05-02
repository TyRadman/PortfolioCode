using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SpeedUpAndDown : MonoBehaviour
{
    [SerializeField] private bool activatesTrailRenderer = false;
    [SerializeField] private bool speedUp = true;
    [SerializeField] private Animator anim;
    [SerializeField] private Image UIBar;
    private playerMovement player;
    private Coroutine tempCoroutine;

    [SerializeField] private float speedChangeMaxAmount;
    [HideInInspector] public float speedChangeAmount;
    public float SpeedChangeAmount
    {
        get => speedChangeAmount;
        set
        {
            speedChangeAmount = value;
            UIBar.fillAmount = value / speedChangeMaxAmount;
        }
    }

    private float changedSpeed;
    private float originalSpeed;
    [SerializeField] [Range(1.1f, 4f)] private float speedMultiplier = 2f;
    private float waitBeforeChargeTime = 2f;

    private void Awake()
    {
        SpeedChangeAmount = speedChangeMaxAmount;
        player = FindObjectOfType<playerMovement>();

        if (!speedUp)
        {
            speedMultiplier = 1 / speedMultiplier;
        }

        changedSpeed = player.speed * speedMultiplier;
        originalSpeed = player.speed;
    }

    void checkCoroutines()
    {
        if (tempCoroutine != null)
        {
            StopCoroutine(tempCoroutine);
            tempCoroutine = null;
        }
    }

    public void ChangeSpeed()
    {
        checkCoroutines();

        if (SpeedChangeAmount <= 0f)
        {
            ResetSpeed();
            return;
        }

        player.speed = changedSpeed;
        tempCoroutine = StartCoroutine(usage());
    }

    public bool HasStaminaLeft()
    {
        return SpeedChangeAmount > 0f;
    }

    public void ResetSpeed()
    {
        PlayerEffects.Instance.SpeedStateChange(PlayerEffects.speedingStates.Idle);     // we put this here and not in the player movement script because sometimes we run out of stamina and there nothing that gets triggered in the playermovement when that happens so we put this command here

        checkCoroutines();
        player.speed = originalSpeed;

        tempCoroutine = StartCoroutine(rechargeBar());
    }

    IEnumerator usage()
    {
        anim.SetBool("Full", false);

        while (SpeedChangeAmount > 0)
        {
            SpeedChangeAmount -= Time.deltaTime;
            yield return null;
        }

        PlayerEffects.Instance.SpeedStateChange(PlayerEffects.speedingStates.Idle); // so that any effects taking place get disabled
        player.speed = originalSpeed;
    }

    IEnumerator rechargeBar()
    {
        yield return new WaitForSeconds(waitBeforeChargeTime);

        while(SpeedChangeAmount < speedChangeMaxAmount)
        {
            SpeedChangeAmount += Time.deltaTime;
            yield return null;
        }

        anim.SetBool("Full", true);
        SpeedChangeAmount = speedChangeMaxAmount;
    }

    public void FullyCharge()
    {
        SpeedChangeAmount = speedChangeMaxAmount;
    }
}
