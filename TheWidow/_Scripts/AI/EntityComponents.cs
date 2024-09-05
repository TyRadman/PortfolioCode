using BT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityComponents : MonoBehaviour, IController, IComponent
{
    [field: SerializeField] public StatesEntity Entity { get; private set; }
    [field: SerializeField] public EntityHearing Hearing { get; private set; }
    [field: SerializeField] public EntitySight Sight { get; private set; }
    [field: SerializeField] public EntityAnimation Animation { get; private set; }
    [field: SerializeField] public EnemyAudio Audio { get; private set; }
    [field: SerializeField] public EyeIndicator EyeIndicator { get; private set; }
    [field: SerializeField] public BehaviorTreeRunner BTRunner { get; private set; }
    [field: SerializeField] public DifficultyModifier Difficulty { get; private set; }
    public DifficultyModifier.EnemyTag m_EnemyTag;
    public bool IsActive { get; set; }
    public const float REFRESH_RATE = 0.2f;

    private void Start()
    {
        SetUp(this);
        Activate();
    }

    public virtual void SetUp(IComponent component)
    {
        Difficulty = GameManager.Instance.Settings.CurrentDifficulty;
        Entity.SetUp(this);
        Hearing.SetUp(this);
        Sight.SetUp(this);
        Audio.SetUp(this);
        Animation.SetUp(this);
    }

    public virtual void Activate()
    {
        Entity.Activate();
        Hearing.Activate();
        Sight.Activate();
        Audio.Activate();
        Animation.Activate();
        BTRunner.Run();
    }

    public virtual void Dispose()
    {
        Entity.Dispose();
        Hearing.Dispose();
        Sight.Dispose();
        Audio.Dispose();
        Animation.Dispose();
    }
}
