using UnityEngine;

public class HidingSpot : MonoBehaviour
{
    public float ControllerHeight;
    public float CameraHeight;
    [HideInInspector] public Vector3 EnemyStandingPlace;
    private int m_NumberOfTimesSpotted;
    private UnityEngine.AI.NavMeshObstacle m_Blocker;

    void Start()
    {
        EnemyStandingPlace = transform.GetChild(0).transform.position;
        m_NumberOfTimesSpotted = GameManager.Instance.HidingPlaceDuration;
        m_Blocker = GetComponent<UnityEngine.AI.NavMeshObstacle>();
    }

    void OnTriggerEnter(Collider cinfo)
    {
        if (cinfo.CompareTag("Player") && PlayerMovement.Instance.IsCrouching)
        {
            Statistics.HidingTime += Time.deltaTime;                     // statistics

            PlayerActions.Instance.FlashLightStateChanged += hidingValueChange; // subscribes the function to the event of the flashlight toggle, to avoid making every hiding spot waiting for the message
            PlayerStats.Instance.HidingSpot = this;
            PlayerMovement.Instance.ToggleCrouch(ControllerHeight, CameraHeight);// change the heights in accordance

            if (EnemiesManager.Instance.EnemiesExistInLevel())                                                 // if the enemy exists
            {
                if (!PlayerStats.Instance.IsSeen)
                {
                    PlayerStats.Instance.IsHidden = true;                                // in case the player hid without the enemy watching him or the flashlight on
                    PlayerMovement.Instance.CurrentState = PlayerMovement.States.hiding; // changes  the player's state
                }
                else
                {
                    m_Blocker.enabled = false;
                }
            }

            if (PlayerActions.Instance.FlashLight.enabled)
            {
                hidingValueChange(true);
            }
        }
    }

    // when the player leaves the hiding zone
    void OnTriggerExit(Collider cinfo)
    {
        if (cinfo.CompareTag("Player") && PlayerMovement.Instance.IsCrouching)
        {
            // the pathfinding blocker is enabled again so that the enemy can't pass by the hiding spot
            m_Blocker.enabled = true;
            // unsubscribing the function since the player is not hiding
            PlayerActions.Instance.FlashLightStateChanged -= hidingValueChange;                    
            PlayerStats.Instance.IsHidden = false;
            // the player has no hiding spot any more
            PlayerStats.Instance.HidingSpot = null;
            // since the player cannot hide without crouching then the state of the player right after leaving the hiding spot is crouching
            PlayerMovement.Instance.CurrentState = PlayerMovement.States.crouching;
            // once the player enter hiding zone, the height of the camera is changed depending on how low the hiding zone is. Once the player leaves the hiding zone, the default height is set
            PlayerMovement.Instance.ToggleCrouch(PlayerMovement.Instance.m_CrouchingControllerHeight, PlayerMovement.Instance.m_CrouchingCameraHeight);
        }
    }

    // does everything the OnTriggerEnter function does when a player enters it if the enemy was spawned while the player is hidden
    public void AssignEnemy()                                         
    {
        PlayerActions.Instance.FlashLightStateChanged += hidingValueChange;
        PlayerMovement.Instance.CurrentState = PlayerMovement.States.hiding;
    }

    // if the flash light is on then the player is not hidden, otherwise he is
    void hidingValueChange(bool _on)
    {
        if (!PlayerStats.Instance.IsSeen)
        {
            PlayerStats.Instance.IsHidden = !_on;
            m_Blocker.enabled = !_on;
        }
        else
        {
            PlayerStats.Instance.IsHidden = false;
            m_Blocker.enabled = false;
        }
    }

    // if the enemy is chasing the player and eventually loses him because he's hidden, a counter for this particular hiding spot starts. After the player hides in the same spot a couple of times, the enemy will be able to see through the hiding spot making it unusable
    public void IncreaseSpottingTimes()
    {
        if (m_NumberOfTimesSpotted > 1)
        {
            PlayerStats.Instance.IsHidden = false;
        }
        else
        {
            m_NumberOfTimesSpotted++;
        }
    }
}