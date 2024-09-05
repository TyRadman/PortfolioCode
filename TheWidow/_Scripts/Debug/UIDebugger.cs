using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDebugger : MonoBehaviour
{
    [SerializeField] private Image _barImage;

    public void UpdateBarImageAmount(float amount)
    {
        _barImage.fillAmount = amount;
    }
}
