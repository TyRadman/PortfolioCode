using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class TankBodyParts : MonoBehaviour
    {
        [SerializeField] private List<TankBodyPart> _parts;

        [Header("Death Explosion")]
        [SerializeField] protected bool _explodeOnDeath = true;
        [SerializeField] protected float _explosionForce = 10f;
        [SerializeField] protected float _explosionRadius = 2f;
        [SerializeField] protected float _upwardsModifier = 0.5f;

        private UnitData _unitData;
        private Texture2D _skinTexture;
        private Dictionary<BodyPartType, TankBodyPart> _partsDictionary = new Dictionary<BodyPartType, TankBodyPart>();

        public List<TankBodyPart> Parts => _parts;

        protected UnitParts _bodyParts;

        public void SetTextureForMainMaterial(Texture2D texture = null)
        {
            if(texture != null)
            {
                _skinTexture = texture;
            }

            _bodyParts.SetTextureForMainMaterial(_skinTexture);
        }

        public void SetUp(UnitData data)
        {
            _unitData = data;

            _partsDictionary.Clear();

            foreach (var part in _parts)
            {
                if (part is TankBody)
                {
                    _partsDictionary.Add(BodyPartType.Body, part);
                }
                else if (part is TankTurret)
                {
                    _partsDictionary.Add(BodyPartType.Turret, part);
                }
                else if (part is TankCarrier)
                {
                    _partsDictionary.Add(BodyPartType.Carrier, part);
                }
            }
        }

        public void InstantiateBodyParts()
        {
            if (_unitData is EnemyData)
            {
                _bodyParts = GameManager.Instance.EnemiesManager.GetEnemyPartsByType(((EnemyData)_unitData).EnemyType);
            }
            else if (_unitData is BossData)
            {
                _bodyParts = GameManager.Instance.BossesManager.GetBossPartsByType(((BossData)_unitData).BossType);
            }
            else if (_unitData is PlayerData)
            {
                _bodyParts = GameManager.Instance.PlayersManager.GetPlayerPartsByType(((PlayerData)_unitData).PlayerType);
            }

            _bodyParts.transform.parent = transform;
            _bodyParts.transform.localPosition = Vector3.zero;
            _bodyParts.transform.localRotation = Quaternion.identity;
            _bodyParts.gameObject.SetActive(false);
        }

        public TankBodyPart GetBodyPartOfType(BodyPartType type)
        {
            if (_partsDictionary.ContainsKey(type))
                return _partsDictionary[type];
            else
                return null;
        }

        public void HandlePartsExplosion(UnitData data)
        {
            if (!_explodeOnDeath)
            {
                return;
            }

            GameManager.Instance.SetParentToSpawnables(_bodyParts.gameObject);
            _bodyParts.gameObject.SetActive(true);

            Transform body = GetBodyPartOfType(BodyPartType.Body).transform;
            Transform turret = GetBodyPartOfType(BodyPartType.Turret).transform;

            _bodyParts.transform.position = transform.position;
            _bodyParts.transform.rotation = transform.rotation;
            _bodyParts.gameObject.SetActive(true);
            _bodyParts.StartExplosion(_explosionForce, _explosionRadius, _upwardsModifier, turret.rotation, body.rotation);

            _bodyParts = null;
        }
    }

    public enum BodyPartType
    {
        Body = 0,
        Turret = 1,
        Carrier = 2,
    }
}
