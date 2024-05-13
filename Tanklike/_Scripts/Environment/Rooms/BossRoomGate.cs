using System.Collections;
using System.Collections.Generic;
using TankLike.Cam;
using TankLike.Misc;
using TankLike.Sound;
using UnityEngine;

namespace TankLike.Environment
{
    public class BossRoomGate : RoomGate
    {
        [SerializeField] private InteractableArea _interactableArea;
        [SerializeField] private Transform[] _keySlots;
        [SerializeField] private GameObject[] _gateMeshes;
        [SerializeField] private AnimationEventPublisher _openGateAnimationEvent;
        [SerializeField] private Audio _gateOpeningAudio;
        [SerializeField] private BossKey_GateSnap _keyPrefab;
        [SerializeField] private CameraShakeSettings _onOpenedCameraShake;
        private BossKey_GateSnap[] _keys;
        [Header("Animation values")]
        [SerializeField] private float _snapToAirDuration = 0.5f;
        [SerializeField] private float _snapToKeySlots = 0.5f;

        private const string NOT_ENOUGH_KEYS_MESSAGE = "Not enough keys";
        private const string OPEN_DOOR_ANIMATION_NAME = "Open";
        private const float BREAK_BETWEEN_ANIMATION_STATES= 0.2f;

        public void PlaceKeys(int playerIndex)
        {
            if (GameManager.Instance.BossKeysManager.HasEnoughKeys())
            {
                _interactableArea.StopInteraction();
                _interactableArea.EnableInteraction(false);
                StartCoroutine(SnapKeysRoutine(playerIndex));
            }
            else
            {
                GameManager.Instance.InteractableAreasManager.SetFeedbackText(NOT_ENOUGH_KEYS_MESSAGE, playerIndex);
            }
        }

        public override void Setup(bool roomHasEnemies, Room parentRoom)
        {
            if (_newRoomEnterTrigger != null)
            {
                _newRoomEnterTrigger.gameObject.SetActive(false);
            }

            //print("Got here");
            _parentRoom = parentRoom;
            InstantiateKeys();
            _interactableArea.EnableInteraction(true);
            GameManager.Instance.BossKeysManager.SetBossRoomGate(this);
            _openGateAnimationEvent.OnEventFired += OnGateOpenedHandler;
        }

        private void InstantiateKeys()
        {
            _keys = new BossKey_GateSnap[_keySlots.Length];
            Transform keysParent = new GameObject("Keys").transform;
            keysParent.parent = transform;
            keysParent.localPosition = Vector3.zero;

            for (int i = 0; i < _keys.Length; i++)
            {
                _keys[i] = Instantiate(_keyPrefab, keysParent);
                _keys[i].gameObject.SetActive(false);
            }
        }

        private void OnGateOpenedHandler()
        {
            foreach (var mesh in _gateMeshes)
            {
                mesh.SetActive(false);
            }

            _openGateAnimationEvent.OnEventFired -= OnGateOpenedHandler;
        }

        public override void CloseGate()
        {
            _interactableArea.StopInteraction();
            _interactableArea.EnableInteraction(false);
            _gateExitTrigger.gameObject.SetActive(false);
        }

        public override void OpenGate()
        {
            _interactableArea.EnableInteraction(true);
        }

        private IEnumerator SnapKeysRoutine(int playerIndex)
        {
            Transform player = GameManager.Instance.PlayersManager.GetPlayer(playerIndex).transform;
            Vector3 startPos = player.position;
            float timer;

            for (int i = 0; i < _keySlots.Length; i++)
            {
                Vector3 targetPos = startPos;
                Transform keyslot = _keySlots[i];
                targetPos.y = keyslot.position.y;
                targetPos.x = keyslot.position.x;

                BossKey_GateSnap key = _keys[i];
                key.transform.SetPositionAndRotation(startPos, keyslot.rotation);
                key.gameObject.SetActive(true);
                key.PlayAnimation();
                timer = 0f;

                while (timer < _snapToAirDuration)
                {
                    timer += Time.deltaTime;
                    float t = timer / _snapToAirDuration;
                    key.transform.position = Vector3.Lerp(key.transform.position, targetPos, t);
                    yield return null;
                }
            }

            yield return new WaitForSeconds(BREAK_BETWEEN_ANIMATION_STATES);
            timer = 0f;

            while (timer < _snapToKeySlots)
            {
                for (int i = 0; i < _keys.Length; i++)
                {
                    timer += Time.deltaTime;
                    float t = timer / _snapToKeySlots;
                    Vector3 targetPos = _keySlots[i].position;
                    BossKey_GateSnap key = _keys[i];
                    key.transform.position = Vector3.Lerp(key.transform.position, targetPos, t);
                }

                yield return null;
            }

            for (int i = 0; i < _keySlots.Length; i++)
            {
                Vector3 position = _keySlots[i].position;
                Quaternion rotation = Quaternion.Euler(0f, 0f, 0f);

                ParticleSystemHandler vfx = GameManager.Instance.VisualEffectsManager.Misc.BossKeyImpact;
                vfx.transform.SetPositionAndRotation(position, rotation);
                vfx.gameObject.SetActive(true);
                vfx.Play();
            }
  
            GameManager.Instance.CameraManager.Shake.ShakeCamera(_onOpenedCameraShake);
            GameManager.Instance.AudioManager.Play(_gateOpeningAudio);

            yield return new WaitForSeconds(BREAK_BETWEEN_ANIMATION_STATES);

            _gateAnimator.Play(OPEN_DOOR_ANIMATION_NAME);
            _newRoomEnterTrigger.gameObject.SetActive(true);
        }
    }
}