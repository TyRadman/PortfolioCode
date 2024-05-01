using UnityEngine;

public class CinameticTigger : MonoBehaviour
{
    [SerializeField] private bool m_StopMovement = true;
    [SerializeField] private CameraObject[] m_ObjectsToLookAt;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            lookAtObjects();
            gameObject.SetActive(false);
        }
    }

    private void lookAtObjects()
    {
        PlayerCameraEvent.Instance.RotateToObjects(m_ObjectsToLookAt, m_StopMovement);
    }
}

[System.Serializable]
public struct CameraObject
{
    public Transform Object;
    public DialogueMessage Message;
    public float RotationDuration;
    public float LookingDuration;
    public AnimationCurve SmoothnessCurve;
}
