using UnityEngine;
using System.Collections;

public class Obstacle : MonoBehaviour
{
    private const float OFFSET = 6.5f;
    private float maxOffset = 6.5f, minOffset = -6.5f;
    [SerializeField] private bool randomSpeed = false;

    enum directions
    {
        xAxis, zAxis
    }
    [SerializeField] private directions direction;
    public enum ObstacleTypeEnum
    {
        Stable, MovingSideWays, MovingUpAndDown
    }
    public ObstacleTypeEnum ObstacleType;

    public enum ObstacleState
    {
        Normal, FlyingAway, Transparent
    }

    public static ObstacleState CurrentState;
    private float particlesOffset = 0.04f;      // related to the steel power up when multiple effects are played
    private float movementSpeed = 40f;
    [Tooltip("500 is the minimum speed, very slow. 2000 is pretty challenging")]
    [SerializeField] private float minimumSpeed = 1000f;
    [Tooltip("500 is the minimum speed, very slow. 2000 is pretty challenging")]
    [SerializeField] private float maxnimumSpeed = 2000f;
    private Rigidbody rb;
    private Vector3 directionOfMovement;
    private Vector3 pointA, pointB;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();

        directionOfMovement = direction == directions.xAxis ? Vector3.right : Vector3.forward;

        if (randomSpeed)
        {
            movementSpeed = Random.Range(minimumSpeed, maxnimumSpeed);
        }

        getOffset();

        if (ObstacleType == ObstacleTypeEnum.MovingSideWays)
        {
            float tSpeed = Mathf.InverseLerp(minimumSpeed, maxnimumSpeed, movementSpeed);   // how much is the current speed between zero and one
            gameObject.AddComponent<ObstacleColor>().SetUp(ObstaclesManager.lightType.MovingObstacleLight, tSpeed);


            if (movementSpeed > 0)
            {
                movementSpeed *= -1f;
            }

            Invoke(nameof(move), 0.1f);
        }
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void move()
    {
        StartCoroutine(MovingSideWays(true));
    }

    IEnumerator MovingSideWays(bool front)
    {
        movementSpeed *= -1;

        while (movementCondition(front))
        {
            rb.velocity = directionOfMovement * movementSpeed * Time.deltaTime;

            yield return null;
        }

        rb.velocity = Vector3.zero;

        StartCoroutine(MovingSideWays(!front));
    }

    private bool movementCondition(bool front)
    {
        float pos;
        if(direction == directions.xAxis)
        {
            pos = transform.position.x;
        }
        else
        {
            pos = transform.position.z;
        }

        

        return !front? pos > maxOffset : pos < minOffset;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            StopAllCoroutines();
            rb.constraints = RigidbodyConstraints.None;
            var collisionPos = collision.contacts[0].point;

            playParticles(collisionPos);
            steelPushCheck(collisionPos);

            if(CurrentState != ObstacleState.Normal)
            {
                return;
            }

            if (GamePlayManager.Instance.GameOver)
            {
                return;
            }

            GamePlayManager.Instance.GameOver = true;
        }
    }

    private void playParticles(Vector3 position)
    {
        EffectsPoolingManager.instance.SpawnEffect("Walls Hit", position);
    }

    private void steelPushCheck(Vector3 position)
    {
        if(CurrentState == ObstacleState.FlyingAway)
        {
            GetComponent<Rigidbody>().AddExplosionForce(MetalPowerUp.PushForce, position, MetalPowerUp.ExplosionRaduis);

            for (int i = 0; i < Random.Range(2, 4); i++)
            {
                var newPos = new Vector3(position.x + Random.Range(-particlesOffset, particlesOffset),
                    position.y + Random.Range(-particlesOffset, particlesOffset),
                    position.z + Random.Range(-particlesOffset, particlesOffset));
                playParticles(newPos);
            }
        }
    }

    private void passThroughCheck(Vector3 position)
    {
        if(CurrentState == ObstacleState.Transparent)
        {
            // play some green particles
            
        }
    }

    // the function that gets the limits of where the obstacle can move
    private void getOffset()
    {
        GetComponent<BoxCollider>().enabled = false;
        float push = 1f;
        Vector3 dir = direction == directions.xAxis ? Vector3.right : Vector3.forward;

        for (int i = 0; i < 1000; i++)
        {
            var lookingPos = transform.position + Vector3.up * 10f + dir * push;
            push += 1f;

            if (Physics.Raycast(lookingPos, Vector3.down, out RaycastHit hit, 20f))
            {
                if (!hit.collider.CompareTag("Ground") || hit.collider == null)
                {
                    var side = lookingPos - Vector3.up * 10f + dir * (push - 2f);
                    maxOffset = direction == directions.xAxis ? side.x : side.z;
                }
            }
            else
            {
                var side = lookingPos - Vector3.up * 10f - dir * 2f;
                minOffset = direction == directions.xAxis ? side.x : side.z;
                new GameObject("Point A").transform.position = side;
                pointA = side;
                break;
            }
        }

        push = -1f;

        for (int i = 0; i < 1000; i++)
        {
            var lookingPos = transform.position + Vector3.up * 10f + dir * push;
            push -= 1f;
            if (Physics.Raycast(lookingPos, Vector3.down, out RaycastHit hit, 20f))
            {
                if (!hit.collider.CompareTag("Ground") || hit.collider == null)
                {
                    var side = lookingPos - Vector3.up * 10f - dir * (push + 2f);
                    maxOffset = direction == directions.xAxis ? side.x : side.z;
                    break;
                }
            }
            else
            {
                var side = lookingPos - Vector3.up * 10f + dir * 2f;
                maxOffset = direction == directions.xAxis ? side.x : side.z;
                new GameObject("Point B").transform.position = side;
                pointB = side;
                break;
            }
        }

        GetComponent<BoxCollider>().enabled = true;
    }
}
