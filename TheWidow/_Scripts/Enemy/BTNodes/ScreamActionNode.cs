using BT;
using BT.Nodes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BT.Nodes
{
    public class ScreamActionNode : ActionNode
    {
        private EyeIndicator m_Indicator;
        private StatesEntity m_Entity;
        protected EnemyAudio m_Audio;
        [SerializeField] private float m_LookingSpeed = 5f;
        [SerializeField] private float m_ScreamingDuration = 5f;
        [SerializeField] private MachineState.StateName m_StateTag;
        private bool m_ScreamingPhaseIsRunning = false;
        public BlackboardKey IsChasingKey;

        public override void OnAwake()
        {
            base.OnAwake();

            m_Entity = Agent.Entity;
            m_Indicator = Agent.EyeIndicator;
            m_Audio = Agent.Audio;
        }

        protected override void OnStart()
        {

            m_Entity.Animation.Animate(m_StateTag);
            m_Audio.PlayAudio(m_StateTag);

            m_Indicator.UpdateIdicator(EyeState.IsPlayerInSight);
            m_Indicator.UpdateIdicator(EyeState.OnPlayerSpotted);
            m_Entity.SetAgentSpeed(0f);
            m_Entity.SetAgentTarget(m_Entity.PlayerTransform.position);
            m_ScreamingPhaseIsRunning = true;

            m_Entity.StartCoroutine(FinishScreamingPhase());
        }

        protected override void OnStop()
        {

        }

        protected override NodeState OnUpdate()
        {
            m_Entity.RotateToTarget(m_Entity.PlayerTransform.position, m_LookingSpeed);

            return m_ScreamingPhaseIsRunning? NodeState.Running : NodeState.Success;
        }

        private IEnumerator FinishScreamingPhase()
        {
            yield return new WaitForSeconds(m_ScreamingDuration);
            m_ScreamingPhaseIsRunning = false;
            Blackboard.SetValue(IsChasingKey.Value, true);
        }
    }
}