using UnityEngine;

// this class makes certain UI elements that use the world axis face the player all the time and makes them fade in and out depending on how close/far the player is from the UI element
public class UIFacingPlayer : MonoBehaviour
{
    private Transform m_PlayerCamera;
    private SpriteRenderer m_Color;
    private float m_Radius, m_HalfRadius;
    private const float VANISHING_DISTANCE = 1.5f;
    private bool m_HidingIconEverSeen = false;

    private void Start()
    {
        m_PlayerCamera = Camera.main.transform;
        m_Color = GetComponent<SpriteRenderer>();
        m_Radius = GetComponent<SphereCollider>().radius;
        m_HalfRadius = (m_Radius - VANISHING_DISTANCE) / 2f + VANISHING_DISTANCE;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!m_HidingIconEverSeen)
        {
            if (other.CompareTag("Player"))
            {
                TipsManager.Instance.DisplayTip(Tips.TipsTag.Hiding);
                m_HidingIconEverSeen = false;
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Color color = m_Color.color;
            float alpha;
            float distance = Vector3.Distance(transform.position, m_PlayerCamera.position);

            // interpolates the alpha value of the UI depending on the distance
            alpha = distance >= m_HalfRadius ?
                Mathf.InverseLerp(m_Radius, m_HalfRadius, distance) : Mathf.InverseLerp(VANISHING_DISTANCE, m_HalfRadius, distance);
            
            m_Color.color = new Color(color.r, color.g, color.b, alpha);
            // makes the UI element face the player
            transform.LookAt(m_PlayerCamera);
        }
    }
}