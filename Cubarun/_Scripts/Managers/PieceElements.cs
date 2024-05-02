using UnityEngine;

public class PieceElements : MonoBehaviour
{
    private Vector3 position;
    private Quaternion rotation;
    private Rigidbody rb;

    private void Awake()
    {
        position = transform.localPosition;
        rotation = transform.rotation;

        if (GetComponent<Rigidbody>())
        {
            rb = GetComponent<Rigidbody>();
        }
    }

    public void ResetTransform()
    {
        gameObject.SetActive(true);
        
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
        }

        transform.localPosition = position;
        transform.rotation = rotation;
    }
}
