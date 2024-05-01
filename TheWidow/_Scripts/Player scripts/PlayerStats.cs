using System.Collections;
using UnityEngine;

// holds data about the player like stamina, batteries or health (in the future)
public class PlayerStats : MonoBehaviour
{
    #region Other classes
    public static PlayerStats Instance;
    [HideInInspector]
    public HidingSpot HidingSpot;
    private PlayerMovement m_Fpc;
    #endregion

    #region Delegates and events
    public delegate void MedicineCollected();
    private event MedicineCollected E_OnMedicineCollected = delegate { };
    #endregion

    #region Variables
    private float m_MaxBatteryLife = 100f;
    private float m_Battery;
    public float BatteryLife
    {
        get { return m_Battery; }
        set
        {
            m_Battery = value > m_MaxBatteryLife ? m_MaxBatteryLife : value;
        }
    }

    [HideInInspector] public int MedicinesCollected;
    [HideInInspector] public int MaxNumberOfMedicine;

    private float m_MaxStamina;
    private float m_Stamina;
    public float Stamina
    {
        get => m_Stamina;
        set
        {
            m_Stamina = value;
            PlayerStatsUI.Instance.UpdateStaminaBar(GetStaminaPercentage());
        }
    }
    [SerializeField] private float CoolDownTime = 2f;
    private bool m_IsHidden = false;
    public bool IsHidden
    {
        get => m_IsHidden;
        set
        {
            //print("Player is " + (value ? "Hidden" : "Not hidden") + Time.time.ToString());

            if (value)
            {
                EyeIndicator.Instance.UpdateIdicator(EyeState.Hiding, true);
            }
            else
            {
                EyeIndicator.Instance.UpdateIdicator(EyeState.OutOfHiding, true);
            }

            m_IsHidden = value;
            ChangePlayerLayer(value ? 2 : 11);
        }
    }
    public bool IsSeen = false;

    static public Coroutine BackToStand_CO;
    [HideInInspector] public bool IsDead = false;
    public bool StaminaIsFull = true;
    #endregion

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        m_Fpc = GetComponent<PlayerMovement>();
    }

    void Start()
    {
        MaxNumberOfMedicine = ItemsSpawning.Instance.GetNumberOfItems("Medicine");
        PlayerStatsUI.Instance.UpdateMedicineText(MedicinesCollected + "/" + MaxNumberOfMedicine);
    }

    void Update()
    {
        // enables running after it gets disabled due to stamina consumption
        if (Input.GetKeyDown(KeyCode.LeftShift) && Stamina > 0 && !m_Fpc.AllowedToRun)
        {
            m_Fpc.AllowedToRun = true;
        }
    }

    public void AddBattery(float _amount)
    {
        // turns the value into a percentage for the max battery life
        BatteryLife += m_MaxBatteryLife * _amount;
        print($"Battery life is {BatteryLife} out of {m_MaxBatteryLife} and the added amount is {_amount}, and the passed value to HUD is {GetBatteryPCT(BatteryLife)}");
        // hud update is required
        BatteryBar.Instance.AddBatteryLife(GetBatteryPCT(BatteryLife));
    }

    public void AddMedicine()
    {
        MedicinesCollected++;
        PlayerStatsUI.Instance.UpdateMedicineText(MedicinesCollected + "/" + MaxNumberOfMedicine);
        E_OnMedicineCollected();                                            // an event occurance, if any listeners exist, an action is expected
    }

    #region Stamina
    // new stamina methodology
    Coroutine filling;
    public void ConsumeStamina()
    {
        StaminaIsFull = false;
        HUDManager.Instance.ToggleBars(true);

        if (filling != null)
        {
            StopCoroutine(filling);
        }

        filling = StartCoroutine(fillStamina());

        if (Stamina > 0.1f)
        {
            Stamina -= Time.deltaTime;
        }
        else
        {
            m_Fpc.AllowedToRun = false;
            Stamina = 0f;
        }
    }

    IEnumerator fillStamina()
    {
        yield return new WaitForSeconds(CoolDownTime);
        m_Fpc.AllowedToRun = true;

        while (Stamina < m_MaxStamina)
        {
            Stamina += Time.deltaTime;
            yield return null;
        }

        Stamina = m_MaxStamina;
        StaminaIsFull = true;
        HUDManager.Instance.ToggleBars(false);
    }

    public void AddStamina(float _amount)
    {
        Stamina += _amount;

        if (Stamina > m_MaxStamina)
        {
            Stamina = m_MaxStamina;
        }
    }
    #endregion

    public void ChangePlayerLayer(int _layer)
    {
        gameObject.layer = _layer;  // making the player hidden or unhidden
    }

    public void SetDifficultyValues(DifficultyModifier _difficulty)
    {
        m_MaxBatteryLife = _difficulty.BatteryMaxLife;
        m_MaxStamina = _difficulty.MaxStamina;
        Stamina = m_MaxStamina;
        m_Battery = m_MaxBatteryLife;
    }

    public float GetStaminaPercentage()
    {
        return Stamina / m_MaxStamina;
    }

    public float GetMaxStamina()
    {
        return m_MaxStamina;
    }

    public void AddListenerToMedicineCollection(MedicineCollected _method)
    {
        E_OnMedicineCollected += _method;       // assigns whatever method we add as a listener
    }

    public void RemoveListenerFromMedicineCollection(MedicineCollected _method)
    {
        E_OnMedicineCollected -= _method;
    }

    public bool BatteryIsFull()
    {
        return BatteryLife == m_MaxBatteryLife;
    }

    public float GetBatteryPCT(float _amout)
    {
        return _amout / m_MaxBatteryLife;
    }
}