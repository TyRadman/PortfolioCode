using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TankLike.Environment
{
    using Utils;

    public class RoomSpawnPoints : MonoBehaviour
    {
        [System.Serializable]
        public class SpawnPoint
        {
            public Transform Point;
            [HideInInspector] public bool Taken = false;
        }

        [field: SerializeField] public List<SpawnPoint> Points { get; private set; } = new List<SpawnPoint>();

        public void AddSpawnPoint(Vector3 position, Transform parent)
        {
            Transform point = new GameObject("SpawnPoint").transform;
            point.parent = parent;
            point.position = position;
            SpawnPoint spawnPoint = new SpawnPoint() { Point = point, Taken = false };
            Points.Add(spawnPoint);
        }

        public Transform GetRandomSpawnPoint(bool markSpawnPointAsTaken = true)
        {
            if (Points.Count <= 0)
            {
                Debug.LogError($"No points available in {gameObject.name}");
                return null;
            }

            SpawnPoint point;

            // if all the points are taken, then we reset them and reuse them
            if (!Points.Exists(p => !p.Taken))
            {
                SetAllPointsAsNotTaken();
                Debug.LogError($"Requests for points is higher than the number of points at room {gameObject.name}");
            }

            point = Points.FindAll(p => !p.Taken).RandomItem(true);

            if (markSpawnPointAsTaken)
            {
                point.Taken = true;
            }

            return point.Point;
        }

        public void SetAllPointsAsNotTaken()
        {
            Points.ForEach(p => p.Taken = false);
        }

        public Transform GetClosestSpawnPointToPosition(Vector3 position, int order)
        {
            return Points.OrderBy(p => Vector3.Distance(position, p.Point.position)).ToArray()[order].Point;
        }
    }
}
