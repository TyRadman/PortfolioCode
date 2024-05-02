using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsPoolingManager : MonoBehaviour
{
    public static EffectsPoolingManager instance;

    [System.Serializable]
    public class EffectPrefab
    {
        [SerializeField] private GameObject effect;
        [SerializeField] private int numberOfInstances;

        public int GetNumberOfInstances()
        {
            return numberOfInstances;
        }

        public GameObject GetPrefab()
        {
            return effect;
        }
    }

    [SerializeField] private EffectPrefab[] effectsPrefabs;
    private List<EffectID> effects = new List<EffectID>();
    private Transform effectsParent;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        effectsParent = new GameObject("Effects").transform;
        createEffects();
    }

    public void SpawnEffect(string effectName, Vector3 position)
    {
        var effect = effects.Find(e => e.EffectName == effectName && e.IsAvailable());

        if (effect == null)
        {
            effect = effects.Find(e => e.EffectName == effectName);
        }

        effect.UseEffect(position);
    }
    #region An overload for having a parent
    public void SpawnEffect(string effectName, Transform parent)
    {
        var effect = effects.Find(e => e.EffectName == effectName && e.IsAvailable());

        if (effect == null)
        {
            effect = effects.Find(e => e.EffectName == effectName);
        }

        effect.UseEffect(parent.transform.position);
        effect.transform.parent = parent;
    }
    #endregion

    public void SpawnEffect(string effectName, Transform parent, float duration = 0f)
    {
        var effect = effects.Find(e => e.EffectName == effectName && e.IsAvailable());

        if (effect == null)
        {
            effect = effects.Find(e => e.EffectName == effectName);
        }

        effect.PlayForDuration(duration, parent);
    }

    void createEffects()
    {
        for (int i = 0; i < effectsPrefabs.Length; i++)
        {
            for (int j = 0; j < effectsPrefabs[i].GetNumberOfInstances(); j++)
            {
                var effect = Instantiate(effectsPrefabs[i].GetPrefab(), effectsParent).GetComponent<EffectID>();
                effects.Add(effect);
            }
        }
    }
}
