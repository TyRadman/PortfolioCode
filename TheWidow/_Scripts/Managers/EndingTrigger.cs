using UnityEngine;

public class EndingTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider _col)
    {
        if (_col.CompareTag("Player"))
        {
            GameManager.Instance.GameEnding();
        }
    }
}