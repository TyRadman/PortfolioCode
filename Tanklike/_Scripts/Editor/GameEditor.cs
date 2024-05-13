using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TankLike.Environment.LevelGeneration;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System;
using TankLike.Combat;
using TankLike.Utils.Editor;

namespace TankLike
{
    public class GameEditor : EditorWindow
    {
        private int _playersNumber;
        private int _buildIndex = 0;
        private int _gameDifficulty;
        private GameManager _gameManager;
        private bool _stretchProperties;

        private void OnEnable()
        {
            _stretchProperties = EditorPrefs.GetBool(nameof(_stretchProperties));
        }

        private GameManager GetGameManager()
        {
            if(_gameManager == null)
            {
                _gameManager = FindObjectOfType<GameManager>();
            }

            return _gameManager;
        }

        [MenuItem("TankLike/Game Editor")]
        public static void ShowWindow()
        {
            GetWindow(typeof(GameEditor), false, "Game Editor");
        }

        private void OnGUI()
        {
            // header
            RenderHeader("Game Editor", 30);

            GUILayout.BeginVertical(GUI.skin.box);
            RenderHeader("Editor Settings", 20, 10);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            RenderStretchSettings();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Label("", GUILayout.Height(10));
            GUILayout.EndVertical();
            GUILayout.Space(20);

            RenderSection("Players", RenderPlayersNumber);
            RenderSection("Level", RenderSelectingScenes, RenderStartRoom);
            RenderSection("Enemies", RenderGameDifficultyAndSpawn);
            RenderSection("Databases", RenderRefreshDataBases);
        }

        private void RenderRefreshDataBases()
        {
            if (GetGameManager() == null)
            {
                return;
            }

            if (GUILayout.Button("Add all ammunitions to DB"))
            {
                AmmunitionDatabase database = GetGameManager().BulletsDatabase;
                database.ClearAmmunitionList();
                var ammunitionData = AssetUtils.GetAllInstances<AmmunationData>(true, new string[] { database.DirectoryToCover });

                foreach (AmmunationData b in ammunitionData)
                {
                    database.AddAmmunition(b);
                }

                // Mark the database as dirty and save the changes
                EditorUtility.SetDirty(database);
                AssetDatabase.SaveAssets();
            }
        }

        private void RenderStretchSettings()
        {
            GUILayout.Label("Stretch Settings");
            _stretchProperties = GUILayout.Toggle(_stretchProperties, "");
            EditorPrefs.SetBool(nameof(_stretchProperties), _stretchProperties);
        }

        private void RenderGameDifficultyAndSpawn()
        {
            if(GetGameManager() == null)
            {
                return;
            }

            EnemiesManager enemies = GetGameManager().EnemiesManager;

            GUILayout.Label("Spawn Enemies");
            bool spawnEnemies = GUILayout.Toggle(enemies.SpawnEnemies(), "");
            enemies.EnableSpawnEnemies(spawnEnemies);

            float difficulty = EditorGUILayout.Slider("Difficulty", enemies.Difficulty, 0f, 1f);
            enemies.SetDifficulty(difficulty);

            EditorUtility.SetDirty(enemies);
        }

        private void RenderSelectingScenes()
        {
            GUILayout.Label("Select Scene");

            _buildIndex = GUILayout.Toolbar(_buildIndex, new string[] { "Player Selection", "Gameplay", "Bosses" });

            if (GUILayout.Button("Load Scene"))
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(SceneUtility.GetScenePathByBuildIndex(_buildIndex));
                }
            }
        }

        private void RenderStartRoom()
        {
            if (GetGameManager() == null)
            {
                return;
            }

            RoomsBuilder builder = GetGameManager().LevelGenerator.RoomsBuilder;
            
            if (builder == null)
            {
                return;
            }

            GUILayout.Space(10);
            
            RoomType startRoom = builder.StartRoomType;
            int roomNumber = (int)startRoom;
            string[] roomTypes = new string[Enum.GetValues(typeof(RoomType)).Length];

            for (int i = 0; i < roomTypes.Length; i++)
            {
                roomTypes[i] = ((RoomType)i).ToString();
            }

            GUILayout.Label("Start Room");
            roomNumber = EditorGUILayout.Popup(roomNumber, roomTypes);
            //roomNumber = GUILayout.Toolbar(roomNumber, roomTypes);
            startRoom = (RoomType)roomNumber;
            builder.StartRoomType = startRoom;
            EditorUtility.SetDirty(builder);
        }

        private void RenderPlayersNumber()
        {
            if (GetGameManager() == null)
            {
                return;
            }

            GamePlayPlayerControlsStarter starter = GetGameManager().InputManager.ControlsStarter;

            if (starter == null)
            {
                return;
            }

            _playersNumber = starter.PlayersCount - 1;
            GUILayout.Space(10);
            GUILayout.Label("Number of players");
            _playersNumber = GUILayout.Toolbar(_playersNumber, new string[] { "1 Player", "2 Players" }) + 1;
            GameManager.Instance.InputManager.ControlsStarter.PlayersCount = _playersNumber;
            EditorUtility.SetDirty(starter);
            GUILayout.Space(10);
        }

        private void RenderHeader(string title, int fontSize, int beforeSpace = 20, int afterSpace = 20)
        {
            GUILayout.Space(beforeSpace);
            GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
            headerStyle.alignment = TextAnchor.MiddleCenter;
            headerStyle.fontSize = fontSize;
            GUILayout.Label(title, headerStyle);
            GUILayout.Space(afterSpace);
        }

        private void RenderSection(string title, params Action[] renders)
        {
            GUILayout.BeginVertical(GUI.skin.box);
            RenderHeader(title, 20, 10);
            GUILayout.BeginHorizontal();

            for (int i = 0; i < renders.Length; i++)
            {
                renders[i]?.Invoke();
            }

            if (!_stretchProperties) GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Label("", GUILayout.Height(10));
            GUILayout.EndVertical();
        }
    }
}
