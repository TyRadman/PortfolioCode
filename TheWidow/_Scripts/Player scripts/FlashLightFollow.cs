using UnityEngine;

public class FlashLightFollow : MonoBehaviour
{
    // the speed by which the light will follow the camera, the lower the value is the slower the movement is performed
    public float Speed = 3f;
    private GameObject m_MainCamera;
    private Vector3 m_Offset;

    void Awake()
    {
        m_MainCamera = Camera.main.gameObject;// references the main camera
        m_Offset = transform.position - m_MainCamera.transform.position;// *OPTIONAL* gets the offset of the flash light to the camera by subtracting the vector3 of their positions
    }

    void FixedUpdate()
    {
        transform.position = m_MainCamera.transform.position + m_Offset;// since the flashlight is not a child of the player, its position must be updated continiously
        transform.rotation = Quaternion.Slerp(transform.rotation, m_MainCamera.transform.rotation, Speed);// as well as its rotation, but with a slerp (a movement with a fixed speed)
    }
}