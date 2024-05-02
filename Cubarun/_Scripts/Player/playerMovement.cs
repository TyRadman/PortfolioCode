using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    public static playerMovement Instance;
    private PlayerComponents components;

    private float horizontalInput;

    // speeds
    public float speed = 0.1f;
    [SerializeField] private float originalTurnSpeed = 0.1f;
    private float turnSpeed;
    [HideInInspector]
    public float originalSpeed;

    public Transform buttonPosL;
    public Transform buttonPosR;

    // status
    public bool isMoving = false;

    // speeding up and down and power up
    [SerializeField] private SpeedUpAndDown speedUp;
    [SerializeField] private SpeedUpAndDown speedDown;
    [SerializeField] private PowerUpsManager powerUps;

    private void Awake()
    {
        Instance = this;
        components = GetComponent<PlayerComponents>();
        originalSpeed = speed;
        turnSpeed = originalTurnSpeed;
    }

    private void Update()
    {
        if (isMoving)
        {
            performTurning();
            speedChangesInput();
            powerUpInput();

            #region Android inputs
            // phone input
            //if(Input.touchCount > 0)
            //{
            //    getTouch(0);
            //    getTouch(1);
            //}
            #endregion
        }
    }

    void powerUpInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            powerUps.ActivatePowerUp();
        }
    }

    void speedChangesInput()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            speedUp.ChangeSpeed();

            if (speedUp.HasStaminaLeft())
            {
                PlayerEffects.Instance.SpeedStateChange(PlayerEffects.speedingStates.SpeedUp);
            }
        }
        
        if (Input.GetKeyUp(KeyCode.W))
        {
            speedUp.ResetSpeed();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            speedDown.ChangeSpeed();            // change the speed

            if (speedDown.HasStaminaLeft())     // only if he can slow down
            {
                PlayerEffects.Instance.SpeedStateChange(PlayerEffects.speedingStates.SlowDown);  // do effects that come with that change of speed
            }
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            speedDown.ResetSpeed();
        }
    }

    void performTurning()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        float threshold = 0.1f;
        components.Turn(horizontalInput >= threshold || horizontalInput <= -threshold);
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            components.MoveRigidBody(new Vector3(horizontalInput * turnSpeed, -500f, speed) * Time.fixedDeltaTime);
        }
    }

    public void PlayState(bool dead)
    {
        components.PlayerState(dead);
        isMoving = !dead;
        components.ResetSize();
        PlayerEffects.Instance.TurnOffEffects();
    }

    //void getTouch(int index)
    //{
    //    Touch touch = Input.GetTouch(index);

    //    if (touch.position.y > buttonPosL.position.y - 124 && touch.position.y < buttonPosL.position.y + 124)
    //    {
    //        if (touch.position.x < buttonPosL.position.x + 129 && touch.position.x > buttonPosL.position.x - 129)
    //            // move right
    //        else if (touch.position.x < buttonPosR.position.x + 129 && touch.position.x > buttonPosR.position.x - 129)
    //            // move left
    //    }
    //}

    /// <summary>
    /// A value of one resets the speed
    /// </summary>
    /// <param name="multiplier"></param>
    public void ChangeTurnSpeed(float multiplier)
    {
        turnSpeed = originalTurnSpeed;
        turnSpeed *= multiplier;
    }

    public void ChangeMovementSpeed(float multiplier)
    {
        speed = originalSpeed;
        speed *= multiplier;
    }

    public void SetPlayerAfterReviving(Vector3 position)
    {
        transform.position = new Vector3(position.x, 1f, position.z);
        components.PlayerState(false);
        components.FreezeMovement();
        transform.rotation = Quaternion.identity;
    }

    public void DisableMovement()
    {
        isMoving = false;
        components.MoveRigidBody(Vector3.zero);
    }
}
