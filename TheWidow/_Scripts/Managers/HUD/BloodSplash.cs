using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BloodSplash : MonoBehaviour
{
    [SerializeField] private Image m_BloodSplashImage;
    [SerializeField] private float m_StartingSplashPercentage;
    [SerializeField] private Animation m_Anim;


    public void SetCriticalLifePoint(float _amount)
    {
        m_StartingSplashPercentage = _amount;
    }

    public void UpdateBloodSplash(float _percentage)
    {
        Color oldColor = m_BloodSplashImage.color;
        float alphaValue = Mathf.InverseLerp(m_StartingSplashPercentage, 0f, _percentage);
        Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, alphaValue);
        m_BloodSplashImage.color = newColor;

        // if the damage is low enough to show the blood splash
        if(alphaValue <= 1f)
        {
            m_Anim.Play();
        }
        else
        {
            m_Anim.Stop();
        }
    }
}
