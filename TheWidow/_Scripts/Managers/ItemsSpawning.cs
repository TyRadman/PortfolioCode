using System.Collections.Generic;
using UnityEngine;

// the calss that spawns items randomly across the level
public class ItemsSpawning : MonoBehaviour
{
    #region Structures
    [System.Serializable]
    // a structure for spawnable items
    struct Spawnable
    {
        // every spawning point has type/s depending on what item can be spawned there
        public ItemName PointType;
        // a prefab of the object to be spawned
        public GameObject Prefab;
        // how many items of this type are to be spawned in the level
        public int Number;
    }

    [SerializeField] private Spawnable[] m_Spawnables;
    [SerializeField] private Spawnable m_ExtraBatteries;
    #endregion

    #region Variables
    static public ItemsSpawning Instance;
    [HideInInspector] public List<Transform> SpawningPoints = new List<Transform>();
    // just to avoid making the player reach a dead end at some point, once all batteries in the level are picked, more batteries will spawn because having a flashlight in the game is crucial
    private int m_CurrentNumberOfBatteries;
    public int CurrentNumberOfBatteries
    {
        get => m_CurrentNumberOfBatteries;
        set
        {
            m_CurrentNumberOfBatteries = value;

            if (value == 1)
            {
                spawnItems(m_ExtraBatteries);
            }
        }
    }
    // the minimum distance allowed between identical items
    public float MinDistance = 10f;
    #endregion

    void Awake()
    {
        #region Singlton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        #endregion

        System.Array.ForEach(FindObjectsOfType<SpawnPoint>(), sp => SpawningPoints.Add(sp.transform));
        CurrentNumberOfBatteries = GetNumberOfItems("battery");                                             // gets the number of batteries spawned    
    }

    public void SpawnAllItems()
    {
        System.Array.ForEach(m_Spawnables, si => spawnItems(si));
    }

    #region Spawning Items Process
    void spawnItems(Spawnable _spawnee)
    {
        // we add an item in the dictionary of the items manager to cache all instances of this item
        // first we make sure this key doesn't already exist (exceptions should be fixed later)
        if (!ItemsManager.Instance.Items.ContainsKey(_spawnee.PointType))
        {
            ItemsManager.Instance.Items.Add(_spawnee.PointType, new List<i_ObjectiveItem>());
        }
        // if there are no points of the type that we want then we just return and log that because it's most likely an error
        if (!SpawningPoints.Exists(sp => sp.GetComponent<SpawnPoint>().Types.Contains(_spawnee.PointType)))
        {
            TestingManager.Instance.Log("No points of type \"" + _spawnee.PointType + "\"");
            print("No points of type \"" + _spawnee.PointType + "\"");
            return;
        }

        // we cache all points that can hold the type of the item to be spawned
        List<Transform> spawnPoints = SpawningPoints.FindAll(sp => sp.GetComponent<SpawnPoint>().Types.Contains(_spawnee.PointType));
        // this will make sure we don't spawn two items at the same point
        List<Transform> spawnedPoints = new List<Transform>();

        for (int i = 0; i < _spawnee.Number; i++)
        {
            // we select a random point if this is our first item on the list to spawn. Otherwise, we select a point that is further than the minimum distance allowed between items
            int index = i == 0 ? Random.Range(0, spawnPoints.Count) : getFarPoint(spawnPoints, spawnedPoints);
            // after we get the index of the point where we'll spawn our item, we create this item. And we cache the interaction component to use it
            var item = Instantiate(_spawnee.Prefab, spawnPoints[index].position, spawnPoints[index].rotation, spawnPoints[index].parent).GetComponent<InteractionClass>();
            // we set the tag of the object to use it later (for the objectives system)
            item.ItemTag = _spawnee.PointType;
            // we cache the item in the items manager. Technically we cache it as its interface since we'll only need to trigger that one later on
            ItemsManager.Instance.Items[_spawnee.PointType].Add(item.GetComponent<i_ObjectiveItem>());
            // we remove the selected point from the list of points so that it's not used again
            SpawningPoints.Remove(spawnPoints[index]);
            // and add it to the list so that we account for it when calculating the minimum distance
            spawnedPoints.Add(spawnPoints[index]);
            spawnPoints.RemoveAt(index);
        }
    }

    // the function that returns the index of the point is far enough from other occupied points
    int getFarPoint(List<Transform> _spawnPoints, List<Transform> _spawnedPoints)
    {
        float dis = MinDistance;
        List<Transform> availablePoints = new List<Transform>();

        while (true)
        {
            //points = _spawnPoints;
            // caches all points that are far enough from all occupied points with the same item
            availablePoints = _spawnPoints.FindAll(p => _spawnedPoints.TrueForAll(sp => Vector3.Distance(sp.position, p.position) > dis));
            // _spawnedPoints.ForEach(spp => points = points.FindAll(sp => Vector3.Distance(sp.position, spp.position) > dis));

            // if there are points that meet this codition then we stop the while loop
            if (availablePoints.Count > 0)
            {
                break;
            }

            // otherwise, we reduce the minimum distance because apparantly there are no points that meet the codition
            dis -= 2f;
        }

        return _spawnPoints.IndexOf(availablePoints[Random.Range(0, availablePoints.Count)]);
    }

    public int GetNumberOfItems(string _name)
    {
        var medicines = System.Array.FindAll(m_Spawnables, s => s.Prefab.name == _name);
        int total = 0;
        System.Array.ForEach(medicines, m => total += m.Number);
        return total;
    }
    #endregion
}