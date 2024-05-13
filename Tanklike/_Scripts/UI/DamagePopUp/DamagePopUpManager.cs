using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace TankLike.UI.DamagePopUp
{
    public class DamagePopUpManager : MonoBehaviour
    {
        [SerializeField] private bool _enabled;
        [SerializeField] private DamagePopUp _damagePopUpPrefab;
        [SerializeField] private Pool<DamagePopUp> _popUpsPool;
        [SerializeField] private List<DamagePopUpInfo> _popUpInfo;

        public void SetUp()
        {
            if (!_enabled)
                return;

            CreatePools();
        }

        public void DisplayPopUp(DamagePopUpType type, int damageText, Vector3 position)
        {
            if (!_enabled)
                return;

            DamagePopUp popUp = _popUpsPool.RequestObject(position, Quaternion.identity);
            popUp.gameObject.SetActive(true);
            popUp.SetUp(damageText, _popUpInfo.Find(i => i.Type == type));
        }

        #region Pool methods
        private void CreatePools()
        {
            _popUpsPool = new Pool<DamagePopUp>(CreateNewInstance, OnObjRequest,
                OnObjRelease, OnObjClear,0);
        }

        private DamagePopUp CreateNewInstance()
        {
            DamagePopUp obj = Instantiate(_damagePopUpPrefab);
            GameManager.Instance.SetParentToSpawnables(obj.gameObject);
            return obj;
        }

        private void OnObjRequest(DamagePopUp obj)
        {
            obj.GetComponent<IPoolable>().OnRequest();
        }

        private void OnObjRelease(DamagePopUp obj)
        {
            obj.GetComponent<IPoolable>().OnRelease();
        }

        private void OnObjClear(DamagePopUp obj)
        {
            obj.GetComponent<IPoolable>().Clear();
        }
        #endregion
    }

    public enum DamagePopUpType
    {
        Damage = 0, Heal = 1, Fire = 2
    }
}
