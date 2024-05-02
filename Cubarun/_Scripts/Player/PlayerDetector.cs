using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    private bool randomLevel = true;

    private void Awake()
    {
        transform.position += new Vector3(0, 0, 60f);       // must be changed
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !GamePlayManager.Instance.GameOver)
        {
            transform.position += new Vector3(0, 0, 60f);
            
            if (randomLevel)
            {
                detectWithRandomGenerator();
            }
            else
            {
                detectWithPremadeLevel();
            }
        }
    }

    private void detectWithPremadeLevel()
    {
        LevelCreator.Instance.EnableNewPiece();
    }

    private void detectWithRandomGenerator()
    {
        RandomLevelGenerator.Instance.CreateNextPiece();
        RandomLevelGenerator.Instance.IncreasePiecesPassed();
    }

    public void IsRandomLevel(bool random)
    {
        randomLevel = random;
    }
}
