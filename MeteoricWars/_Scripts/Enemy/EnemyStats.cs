using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SpaceWar
{
    public class EnemyStats : Stats
    {
        public ShipRank Rank;
        public EnemyShooting TheEnemyShooter;
        public EnemyComponents Components;
        public string EnemyName;
        public bool SpawnCollectables = true;
        public float ScoreValue;
        public float Difficulty = 1f;
        public UnityAction OnDeathAction;
        public PolygonCollider2D Collider;

        protected override void Awake()
        {
            base.Awake();
            Collider = GetComponent<PolygonCollider2D>();
            IsInWave = true;
            TheEnemyShooter = GetComponent<EnemyShooting>();
            EnemyName = gameObject.name;
        }

        public override void OnShipDestroyed(int _playerIndex)
        {
            base.OnShipDestroyed(_playerIndex);

            // invoke any events subscribed to the enemy's death
            OnDeathAction?.Invoke();

            // play the explosion particles using the method overload that takes size into account to resize the explosion
            PoolingSystem.Instance.UseParticles(ParticlePoolTag.StandardExplosion, transform.position, Quaternion.identity, transform.localScale.x);

            // play ship's death particles
            SpawnAndKillManager.i.ShipDeath(Components.Effects.GetSprites(), transform, true);

            // add score to the player
            AddScoreToPlayer(_playerIndex);

            // report the ship's death to the enemies' manager so that it checks if there's a wave to spawn next 
            ReportDeathToEnemiesManager();

            // stop all the ship's components that are being performed on Update
            StopComponents();

            // freeze the screen (an effect that can be moved somewhere else later)
            ScreenFreezer.i.FreezeScreen(GameManager.i.GeneralValues.RankValues.Find(v => v.Rank == Rank).FreezeStrength);

            // spawn collectables if there are any
            SpawnCollectable();
        }

        public void SpawnCollectable()
        {
            if (SpawnCollectables)
            {
                CollectableSpawner.i.SpawnCollectable(transform.position, Rank, Collider.GetAverageArea());
            }
        }

        private void ReportDeathToEnemiesManager()
        {
            if (IsInWave)
            {
                // so that the wave manager starts spawning if all enemies are down
                WavesManager.i.EnemyDestroyed(transform);
            }
            else
            {
                // if the ship is fighting on the player's side, then it's not a part of the wave therefore, we don't notify the waves manager
                if (UserTag == BulletUser.Enemy)
                {
                    WavesManager.i.EnemyDestroyed(transform);
                }

                GetComponent<SummonedShip>().OnDestroyed(UserTag, transform);
            }
        }

        private void StopComponents()
        {
            Components.TurnOnDamageDetectors(false);
            // stops the enemy from shooting (a ghost keeps shooting if this line wasn't there, trust me)
            TheEnemyShooter.StopShooting();
            Components.Movement.StopMovement();
            // if the ship has an ability, then stop it from looping (remember the mother ship that wouldn't stop spawning minions?
            if (Components.HasAbility) Components.Abilities.StopSpecialAttackQueue();
        }

        public override void SetDamage(float _damage)
        {
            base.SetDamage(_damage);
            Components.ShipShooter.Damage = _damage;
        }

        private void AddScoreToPlayer(int _playerIndex)
        {
            if (_playerIndex >= 0)
            {
                GameManager.i.PlayersManager.Players[_playerIndex].Components.Stats.AddScore(ScoreValue);
            }
        }

        public void SetUserTag(BulletUser _tag)
        {
            UserTag = _tag;
            Components.ShipShooter.UserTag = _tag;
            Components.ShipShooter.SetWeaponShooting();
            transform.tag = _tag == BulletUser.Player ? GeneralValues.PlayerTag : GeneralValues.EnemyTag;
            gameObject.layer = _tag == BulletUser.Player ? GeneralValues.PlayerLayerInt : GeneralValues.EnemyLayerInt;
        }
    }
}