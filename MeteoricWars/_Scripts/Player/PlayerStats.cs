using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Space.UI.HUD;
using SpaceWar.Shop;

namespace SpaceWar
{
    public class PlayerStats : Stats
    {
        [HideInInspector] public PlayerComponents Components;
        public bool CanTakeDamage = false;
        [HideInInspector] public int PlayerIndex = -1;
        [SerializeField] private int m_XPPointsLevelIndex;
        public float MaxShooterPoints;
        private float m_CurrentShooterPoints;
        private int m_Lives;
        #region Stats
        private float m_Score;
        private int m_Deaths;
        private int m_ShipsDestroyed;
        #endregion
        public float MissChance;
        public float Resilience;
        private float m_Scale;
        private bool m_EnableComponents = false;
        private static string[] m_LevelsNumbering = new string[10] { "1", "2", "3", "4", "5", "5 i", "5 ii", "5 iii", "5 iv", "5 v" };
        private Collider2D m_Collider;

        #region Defaults
        protected override void Awake()
        {
            m_Collider = GetComponent<Collider2D>();
            Components = GetComponent<PlayerComponents>();
            m_Scale = transform.localScale.x;
        }

        private void Start()
        {
            m_XPPointsLevelIndex = 0;

            // just to distinguish between the main menu and the gameplay scene
            if (PlayerLevelUp.i == null)
            {
                return;
            }

            MaxShooterPoints = PlayerLevelUp.i.ShooterPointsLevels[m_XPPointsLevelIndex];
            m_CurrentShooterPoints = 0f;
            AddXPPoints(0f);
            GameManager.i.HUDManager.UpdateScoreText(0f, PlayerIndex);
            GameManager.i.HUDManager.UpdatePlayerShootingPoints(m_CurrentShooterPoints / MaxShooterPoints, PlayerIndex, GetShooterPointsAmountText(), m_LevelsNumbering[m_XPPointsLevelIndex]);
            GetComponent<Shooter>().UserIndex = PlayerIndex;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                AddXPPoints(1000f);
                Components.Coins.AddCoins(1000);
            }
        }
        #endregion

        #region Setting and resetting
        public override void SetDamage(float _damage)
        {
            base.SetDamage(_damage);

            Components.ShipShooter.Damage = _damage;
        }

        public void ResetStats()
        {
            Components.Health.ResetHealth();

            // shooting points
            // reset the shooting points
            //m_CurrentShooterPoints = 0f;
            // reset the level of the shooter
            //m_XPPointsLevelIndex = 0;
            // reset the max shooting points
            //MaxShooterPoints = PlayerLevelUp.Instance.ShooterPointsLevels[m_XPPointsLevelIndex];
            // set UI
            //GameManager.i.PlayerHUDnstance.UpdatePlayerShootingPoints(m_CurrentShooterPoints / MaxShooterPoints, PlayerIndex, GetShooterPointsAmountText(), m_LevelsNumbering[m_XPPointsLevelIndex]);
            // reset the weapon of the shooter to the first weapon
            //GameManager.Instance.PlayersManager.UpgradePlayerShooting(m_XPPointsLevelIndex, PlayerIndex);
        }
        #endregion

        #region Health

        #region Shooter Points Functions
        public void AddXPPoints(float _points)
        {
            if (m_XPPointsLevelIndex >= PlayerLevelUp.i.ShooterPointsLevels.Count)
            {
                return;
            }

            m_CurrentShooterPoints += _points;

            // if we level up
            if (m_CurrentShooterPoints >= MaxShooterPoints && m_XPPointsLevelIndex <= PlayerLevelUp.i.ShooterPointsLevels.Count - 1)
            {
                Components.PlayLevelUpParticles();
                m_XPPointsLevelIndex++;
                // for stats
                GameStatsManager.i.AddStats(1, PlayerIndex, StatTag.LevelUps);
                GameStatsManager.i.SetMaxStat(m_XPPointsLevelIndex, PlayerIndex, StatTag.HighestLevel);
                m_CurrentShooterPoints -= MaxShooterPoints;

                if (m_XPPointsLevelIndex <= PlayerLevelUp.i.ShooterPointsLevels.Count - 1)
                {
                    MaxShooterPoints = PlayerLevelUp.i.ShooterPointsLevels[m_XPPointsLevelIndex];
                }
                else
                {
                    m_CurrentShooterPoints = MaxShooterPoints;
                }

                // level up
                GameManager.i.PlayersManager.UpgradePlayerShooting(m_XPPointsLevelIndex, PlayerIndex);

                // just in case the amount was enough to level up twice which would only occur if the game was hacked
                if (m_CurrentShooterPoints >= MaxShooterPoints)
                {
                    AddXPPoints(0);
                }
            }

            GameManager.i.HUDManager.UpdatePlayerShootingPoints(m_CurrentShooterPoints / MaxShooterPoints, PlayerIndex, GetShooterPointsAmountText(), m_LevelsNumbering[m_XPPointsLevelIndex]);
        }

        private string GetShooterPointsAmountText()
        {
            return $"{m_CurrentShooterPoints:00} / {MaxShooterPoints:00}";
        }
        #endregion

        #region Score
        public void AddScore(float _score)
        {
            m_ShipsDestroyed++;
            m_Score += _score;
            GameManager.i.HUDManager.UpdateScoreText(m_Score, PlayerIndex);
        }
        #endregion

        #region Lives


        public void SetStartingLives(int _lives)
        {
            m_Lives = _lives;
            GameManager.i.HUDManager.SetLivesCount(m_Lives, PlayerIndex);
        }

        public override void OnShipDestroyed(int _playerIndex = -1)
        {
            // deaths is only for stats
            m_Deaths++;
            GameManager.i.ShopManager.StopAggressiveMode();
            CurrentHealth = 0f;
            Components.Effects.PlayDeathParticles();
            SpawnAndKillManager.i.ShipDeath(Components.Effects.GetSprites(), transform);
            // play the explosion particles using the method overload that takes size into account to resize the explosion
            SwitchVisuals(false);
            m_EnableComponents = false;
            SwitchComponents();
            StartCoroutine(AddLiveAfterTime(SpawnAndKillManager.i.ShrinkingTime));
        }

        private IEnumerator AddLiveAfterTime(float _time)
        {
            yield return new WaitForSeconds(_time);
            AddLives(-1);
        }

        public void AddLives(int _lives)
        {
            m_Lives += _lives;
            GameManager.i.HUDManager.SetLivesCount(m_Lives, PlayerIndex);

            // if there are no lives left then the player is dead, otherwise, it revives
            if (m_Lives < 0)
            {
                // game over 
                Components.IsAlive = false;
                // stop regeneration
                Components.Health.StopRegeneration();
                Components.gameObject.SetActive(false);
                // StopCoroutine(m_RegeneratingCoroutine);

                // Game over for both players
                if (GameManager.i.PlayersManager.AllPlayersDead())
                {
                    GameManager.i.GameOver();
                }
            }
            else
            {
                // revive the player
                // reset values
                SwitchVisuals(true);
                ResetStats();
                m_EnableComponents = true;
                Invoke(nameof(SwitchComponents), SpawnAndKillManager.i.GetRevivingDuration());
                // enable graphics
                SpawnAndKillManager.i.RevivePlayerShip(Components.Effects.GetSprites(), transform, m_Scale, PlayerIndex);
                // animate the player back in
                SpawnAndKillManager.i.MovePlayerToSpawnPosition(Components);
            }
        }

        public void SetLives(int livesCount)
        {
            m_Lives = livesCount;
        }

        private void SwitchVisuals(bool _enable)
        {
            Components.Effects.DamageRenderers[0].PartRenderer.transform.parent.gameObject.SetActive(_enable);

            if (!_enable)
            {
                Components.Effects.Thrusters.ForEach(t => t.Stop());
            }
            else
            {
                Components.Effects.Thrusters.ForEach(t => t.Play());
            }
        }

        private void SwitchComponents()
        {
            CanTakeDamage = m_EnableComponents;
            Components.ShipStats.enabled = m_EnableComponents;
            Components.ShipShooter.enabled = m_EnableComponents;
            Components.ThePlayerShooting.CanShoot = m_EnableComponents;
            Components.Movement.Enable(m_EnableComponents);
            Components.Movement.enabled = m_EnableComponents;
            Components.Abilities.enabled = m_EnableComponents;
            Components.EnableColliders(m_EnableComponents);
        }

        public void SetGameStatsValues()
        {
            GameStatsManager.i.SetStat((int)m_Score, PlayerIndex, StatTag.Score);
            GameStatsManager.i.SetStat(m_Deaths, PlayerIndex, StatTag.Deaths);
            // this is currently inaccurate as the player can spend forever selecting an ability
            GameStatsManager.i.SetStat((int)Time.time, PlayerIndex, StatTag.Time);
            GameStatsManager.i.SetStat(m_ShipsDestroyed, PlayerIndex, StatTag.ShipsDestroyed);
        }
        #endregion

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out Collectable collectable))
            {
                collectable.OnTriggerAction(Components);
                return;
            }
        }
    }
    #endregion
}