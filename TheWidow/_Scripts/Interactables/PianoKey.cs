using System.Collections;
using UnityEngine;

public class PianoKey : InteractionClass
{
    [SerializeField] private float timeToMove = 1f;
    [SerializeField] private float distance = 0.2f;
    [SerializeField] private float stayTime = 0.2f;
    [SerializeField] private string m_Tone;
    private bool usable = true;

    protected override void Interact()
    {
        if (!usable)
        {
            return;
        }

        base.Interact();
        TonesPuzzle.Instance.PlayTone(m_Tone);
        StartCoroutine(holdDown());
    }

    IEnumerator holdDown()
    {
        usable = false;
        var time = 0f;
        var firstPosition = transform.position;
        var newPosition = new Vector3(transform.position.x, transform.position.y - distance, transform.position.z);

        while (time < timeToMove)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, newPosition, time / timeToMove);

            yield return null;
        }

        yield return new WaitForSeconds(stayTime);

        time = 0f;
        
        while (time < timeToMove)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, firstPosition, time / timeToMove);

            yield return null;
        }

        usable = true;
    }
}
