using UnityEngine;

public class followCUbe : MonoBehaviour
{
    private Transform cube;
    [SerializeField]
    public Vector3 offset;

    private void Start()
    {
        cube = FindObjectOfType<playerMovement>().transform;
    }

    void Update()
    {
        if (cube != null)
        {
            transform.position = cube.position + offset;
        }
    }
}
