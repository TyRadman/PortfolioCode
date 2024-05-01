using System.Collections;
using UnityEngine;

// the player actions controller that allows the player to interact with the world around him
public class PlayerActions : MonoBehaviour
{
    #region Variables
    public static PlayerActions Instance;
    public Light FlashLight;
    public float ZoomSpeed;
    private Camera m_Cam;
    [SerializeField] private float MaxZoomIn = 40f;
    public bool HasFlashLight = false;
    public delegate void Action(bool _on);
    public event Action FlashLightStateChanged = delegate { };
    public bool FlashLightOn = false;
    #endregion

    void Awake()
    {
        Instance = this;
        m_Cam = Camera.main;

        if (!HasFlashLight)
        {
            FlashLight.enabled = false;
        }
    }

    void Update()
    {
        if (HasFlashLight)
        {
            flashLightCalculations();
        }

        if (Input.GetMouseButton(0) && !PlayerMovement.Instance.IsWalking && !PauseMenuManager.IsPaused)
        {
            StartCoroutine(zoom(true));
        }

        if (Input.GetMouseButtonUp(0))
        {
            StopAllCoroutines();
            StartCoroutine(zoom(false));
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Statistics.GetData();
        }
    }

    void flashLightCalculations()
    {
        if (Input.GetMouseButtonDown(1))
        {
            turnOnFlashLight();
        }

        if (FlashLight.enabled)
        {
            Statistics.FlashLightTime += Time.deltaTime;                                    // statistics
            PlayerStats.Instance.BatteryLife -= Time.deltaTime;
            BatteryBar.Instance.UpdateBatteryBar(PlayerStats.Instance.GetBatteryPCT(PlayerStats.Instance.BatteryLife));

            if (PlayerStats.Instance.BatteryLife <= 0)
            {
                FlashLight.enabled = false;
            }
        }
    }

    private void turnOnFlashLight()
    {
        if (PlayerStats.Instance.BatteryLife > 0 && !PlayerStats.Instance.IsDead)
        {
            AudioManager.Instance.PlayAudio("FlashLightClick", null, true);
            FlashLight.enabled = !FlashLight.enabled;     // toggle flash light
            FlashLightOn = FlashLight.enabled;
            FlashLightStateChanged(FlashLightOn);
        }
    }

    public void GetFlashLight()
    {
        HasFlashLight = true;
        Invoke(nameof(turnOnFlashLight), 0.5f);
    }

    IEnumerator zoom(bool _in)
    {
        var newWait = new WaitForSeconds(0.01f);
        
        if (_in)
        {
            while (m_Cam.fieldOfView > MaxZoomIn)
            {
                yield return newWait;
                m_Cam.fieldOfView -= ZoomSpeed;
            }
        }
        else
        {
            while (m_Cam.fieldOfView <= 60f)
            {
                yield return newWait;
                m_Cam.fieldOfView += ZoomSpeed * 10;
            }
        }

        m_Cam.fieldOfView = _in ? MaxZoomIn : 60f;
    }
}