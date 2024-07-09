using UnityEngine;

public class MouseDrag : MonoBehaviour
{
    private Vector3 screenPoint; 
    private Vector3 offset;

    private void OnMouseDown()
    {
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - GetMouseWorldPos();
    }

    private void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = curPosition;
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = screenPoint.z;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }
}
