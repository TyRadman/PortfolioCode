using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// since Unity doesn't have a time class that displays hours and minutes, it is a neccesity to make one
public class TimeClass : MonoBehaviour
{
    public static TimeClass Instance;

    private void Awake()
    {
        Instance = this;
    }

    public Vector3Int GetTime()
    {
        var timeInSecs = Time.time;
        int hours, minutes, seconds;
        hours = (int)(timeInSecs / 3600);
        minutes = (int)((timeInSecs - hours * 3600) / 60);
        seconds = (int)(timeInSecs - (hours * 3600 + minutes * 60));

        return new Vector3Int(hours, minutes, seconds);
    }

    public static float ConvertTimeToSeconds(Vector3Int _time)
    {
        return (_time.x * 3600 + _time.y * 60 + _time.z);
    }

    public static void PerformTask(UnityAction _function, float _time)
    {
        Instance.StartCoroutine(Instance.performFunction(_function, _time));
    }

    private IEnumerator performFunction(UnityAction _func, float _time)
    {
        yield return new WaitForSeconds(_time);
        _func.Invoke();
    }
}
