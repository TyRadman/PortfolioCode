using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class TankBodyParts : MonoBehaviour
    {
        [SerializeField] private List<TankBodyPart> _parts;

        private Dictionary<BodyPartType, TankBodyPart> _partsDictionary = new Dictionary<BodyPartType, TankBodyPart>();

        public List<TankBodyPart> Parts => _parts;

        public void SetUp()
        {
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

        public TankBodyPart GetBodyPartOfType(BodyPartType type)
        {
            if (_partsDictionary.ContainsKey(type))
                return _partsDictionary[type];
            else
                return null;
        }
    }

    public enum BodyPartType
    {
        Body = 0,
        Turret = 1,
        Carrier = 2,
    }
}
