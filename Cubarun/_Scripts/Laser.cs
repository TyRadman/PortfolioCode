using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    enum direction
    {
        xAxis, zAxis
    }
    [SerializeField] private direction theDirection = direction.zAxis;

    [SerializeField] private bool stable = true;
    [SerializeField] private bool randomSpeed = false;
    [SerializeField] private float startingAngle = 0;
    [SerializeField] private float rotationSpeed = 100f; 
    [SerializeField] private float minimumRotationSpeed = 100f;
    [SerializeField] private Vector3 _startShootingAngle = new Vector3(90f, 0f, 0f);
    [Header("References")]
    [SerializeField] private Transform body;
    [SerializeField] private Transform shootingPoint;
    [SerializeField] private LineRenderer line;
    [SerializeField] private ParticleSystem _impactParticles;
    [SerializeField] private TrailRenderer _trail;
    [SerializeField] private Transform particles;
    [SerializeField] private Transform trail;
    private static bool playerLost = false;
    private bool _hasTarget = false;
    private const float maxScanDistance = 50f;

    private Vector3 rotationDirection;


    private void OnEnable()
    {
        _hasTarget = false;
        _trail.emitting = false;
        _impactParticles.Stop();
        shootingPoint.localEulerAngles = _startShootingAngle;
        rotationDirection = theDirection == direction.xAxis ? Vector3.right : Vector3.forward;

        if (randomSpeed)
        {
            rotationSpeed = Random.Range(minimumRotationSpeed, rotationSpeed);
        }

        if (stable)
        {
            if(Physics.Raycast(shootingPoint.position, shootingPoint.forward, out RaycastHit hit, maxScanDistance))
            {
                if (hit.collider.CompareTag("Ground"))
                {
                    line.SetPosition(0, shootingPoint.localPosition);
                    particles.position = hit.point;
                    line.SetPosition(1, particles.localPosition);
                    trail.eulerAngles = hit.normal;
                    trail.position = particles.position;
                }
            }

            StartCoroutine(PlayerDetection());
        }
        else
        {
            body.eulerAngles = new Vector3(0f, 0f, startingAngle);

            StartCoroutine(updateLaser());
        }
    }


    IEnumerator PlayerDetection()
    {
        while (gameObject.activeSelf)
        {
            if (Physics.Raycast(shootingPoint.position, shootingPoint.forward, out RaycastHit hit, maxScanDistance))
            {
                if (!_hasTarget)
                {
                    _hasTarget = true;
                    _impactParticles.Play();
                    _trail.Clear();
                    _trail.emitting = true;
                }

                if (hit.collider.CompareTag("Player"))
                {
                    if (!playerLost)
                    {
                        GamePlayManager.Instance.GameOver = true;
                        playerLost = true;
                    }

                    hit.collider.GetComponent<PlayerComponents>().ApplyLaserPush(hit.point);
                }

                line.SetPosition(0, shootingPoint.localPosition);
                particles.position = hit.point;
                line.SetPosition(1, particles.localPosition);
            }
            else
            {
                if (_hasTarget)
                {
                    _hasTarget = false;
                    _trail.emitting = false;
                    _impactParticles.Stop();
                }
            }

            yield return null;
        }
    }
    [SerializeField] private bool _debug = false;
    IEnumerator updateLaser()
    {
        while (gameObject.activeSelf)
        {
            body.Rotate(rotationDirection * rotationSpeed * Time.deltaTime);

            if (Physics.Raycast(shootingPoint.position, shootingPoint.forward, out RaycastHit hit, maxScanDistance))
            {
                if (hit.collider.CompareTag("Ground"))
                {
                    if (!_hasTarget)
                    {
                        if (_debug) print($"Enabled at {Time.time}");
                        _hasTarget = true;
                        _impactParticles.Play();
                        _trail.Clear();
                        _trail.emitting = true;
                    }

                    line.SetPosition(0, shootingPoint.localPosition);
                    particles.position = hit.point;
                    line.SetPosition(1, particles.localPosition);
                    trail.eulerAngles = hit.normal;
                    trail.position = particles.position;
                }
                else if (hit.collider.CompareTag("Player"))
                {
                    if (!playerLost)
                    {
                        GamePlayManager.Instance.GameOver = true;
                        playerLost = true;
                    }

                    hit.collider.GetComponent<PlayerComponents>().ApplyLaserPush(hit.point);

                    line.SetPosition(0, shootingPoint.localPosition);
                    particles.position = hit.point;
                    line.SetPosition(1, particles.localPosition);
                }
                else
                {
                    if (_hasTarget)
                    {
                        if (_debug) print($"Disabled at {Time.time}");
                        _hasTarget = false;
                        _trail.emitting = false;
                        _impactParticles.Stop();
                    }
                }

                if (_debug) print($"{hit.collider.name} at {Time.time}");
            }
            else
            {
                if(_debug)print($"Nothing at {Time.time}");

                if (_hasTarget)
                {
                    if (_debug) print($"Disabled at {Time.time}");
                    _hasTarget = false;
                    _trail.emitting = false;
                    _impactParticles.Stop();
                }
            }

            yield return null;
        }
    }

    public void UpdateLaser()
    {
        if (Physics.Raycast(shootingPoint.position, shootingPoint.forward, out RaycastHit hit, maxScanDistance))
        {
            line.SetPosition(0, shootingPoint.localPosition);
            particles.position = hit.point;
            line.SetPosition(1, particles.localPosition);
        }
    }
    public void RemoveLaser()
    {
        line.SetPosition(0, transform.position);
        line.SetPosition(1, transform.position);
    }
}
