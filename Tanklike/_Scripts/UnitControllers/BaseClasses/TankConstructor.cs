using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class TankConstructor : MonoBehaviour
    {
        [SerializeField] protected Transform _graphicsParent;
        [SerializeField] protected TankTurret[] _turretReferences;
        [SerializeField] protected TankBody[] _bodyReferences;
        [SerializeField] protected TankCarrier[] _carrierReferences;
        [SerializeField] protected bool _noConstruction;
        protected TankTurret _currentTurret;
        protected TankBody _currentBody;
        protected TankCarrier _currentCarrier;

        public virtual void BuildTank()
        {
            // just for testing, in the real game, we'll either destroy older parts or have them deactivated for pooling (in case the player has to respawn with zero upgrades)
            if (_currentBody != null)
            {
                _currentBody.gameObject.SetActive(false);
                _currentCarrier.gameObject.SetActive(false);
                _currentTurret.gameObject.SetActive(false);
            }

            // create the parts if construction is required
            //if(!_noConstruction)
            //{
            //    // the position of the carriers is usually at the height of 0.5 (the ground level.) If a tank is floating for example, then we elevate it in the carriers prefab
            //    _currentCarrier = Instantiate(_carrierReferences[0], _graphicsParent.position, _graphicsParent.rotation, _graphicsParent);
            //    // the position of the body depends on the position of the carriers, so it's stored there
            //    _currentBody = Instantiate(_bodyReferences[0], _currentCarrier.BodyPosition.position, _currentCarrier.BodyPosition.rotation, _graphicsParent);
            //    // the position of the turret is dependant on the body, so it's stored there
            //    _currentTurret = Instantiate(_turretReferences[0], _currentBody.TurretPosition.position, _currentBody.TurretPosition.rotation, _graphicsParent);
            //    _currentCarrier.transform.parent = _currentBody.transform;
            //}
            //else
            //{
            //    _currentCarrier = _carrierReferences[0];
            //    _currentBody = _bodyReferences[0];
            //    _currentTurret = _turretReferences[0];
            //}

            // temporarily here
            if(_currentBody.Meshes.Count > 0)
                GetComponent<TankVisuals>().AddMeshes(_currentBody.Meshes);
            if (_currentTurret.Meshes.Count > 0)
                GetComponent<TankVisuals>().AddMeshes(_currentTurret.Meshes);
            if (_currentCarrier.Meshes.Count > 0)
                GetComponent<TankVisuals>().AddMeshes(_currentCarrier.Meshes);
        }

        //public Transform GetShootingPoint()
        //{
        //    return _currentTurret.ShootingPoints;
        //}
    }
}
