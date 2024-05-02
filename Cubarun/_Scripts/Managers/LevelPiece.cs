using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelPiece : MonoBehaviour
{
    public enum PieceNameEnum
    {
        Empty, OneMiddleObstacle, ThreeObstacles, OneMovingObstacle, TwoMovingObstacles, ThreeMovingObstacles,
        ObstacleLeftPush, ObstacleRightPush, TwoGapsObstacle, MovingObstaclesTrain,
        twoStableLasers, fourStableLasers, leftPushLasers, rightPushLasers, oneRotatingLaser, ThreeRotatingLasersSameSpot, ThreeRotatingLasersSequense,
        ThreeRotatingLasersSpace,
        fourMovingLasers, twoRotatingAndTwoMovingLasers, LaserMovingPath
    }

    [System.Serializable]
    public class PreferredPiece
    {
        [SerializeField] public PieceNameEnum PreferredPieceName;
        [SerializeField] [Range(1, 100)] public float spawnChance = 1f;

        public PreferredPiece(PieceNameEnum name, float chance)
        {
            PreferredPieceName = name;
            spawnChance = chance;
        }
    }

    [SerializeField] private PieceNameEnum pieceType;
    [HideInInspector] public string pieceName;
    [SerializeField] [Range(1, 100)] private float selfSpawnChance= 1f;

    private GameObject Mesh;
    [Range(0, 5)]
    public int Difficulty;
    [SerializeField] private List<PreferredPiece> PreferredPieces = new List<PreferredPiece>();
    private List<PieceElements> children = new List<PieceElements>();
    private GameObject coinsParent;
    [SerializeField] private Transform[] coinsLocations;
    private float coinsChance = 70f;
    private bool willHaveCOins = true;

    private void InEnable()
    {
        if (!willHaveCOins)
            return;

        if(Random.Range(0f, 100f) <= coinsChance)
        {
            for (int i = 0; i < coinsLocations.Length; i++)
            {
                if (coinsLocations[i].childCount == 0)  // making sure this location doesn't have a coin already
                {
                    if(Keys.instance == null)
                    {
                    print("No keys");

                    }
                    if(coinsLocations.Length == 0)
                    {
                        print("No coins");
                    }
                    var coin = Instantiate(Keys.instance.GetCoin(), coinsLocations[i]);
                    coin.transform.localPosition = Vector3.zero;
                }
            }
        }
    }

    public void Initiate()
    { 
        pieceName = pieceType.ToString();
        Mesh = transform.GetChild(0).gameObject;
        PreferredPieces.Add(new PreferredPiece(pieceType, selfSpawnChance));     // add the same piece to its preferrable pieces

        coinsParent = transform.GetChild(transform.childCount - 1).gameObject;
        coinsLocations = new Transform[coinsParent.transform.childCount];

        for (int i = 0; i < coinsParent.transform.childCount; i++)
        {
            coinsLocations[i] = coinsParent.transform.GetChild(i);
        }

        getChildren();
    }

    public void SetToPosition(Vector3 newPos)
    {
        transform.position = newPos;
    }

    public string NextPieceName(List<LevelPiece> allPieces)
    {
        var tempPreferred = PreferredPieces.FindAll(p => allPieces.Exists(pp => pp.pieceName == p.PreferredPieceName.ToString()));
        string selectedPiece = "";

        float chance = Random.Range(1f, 100f);

        if (tempPreferred.Exists(p => p.spawnChance >= chance))
        {
            var pieces = tempPreferred.FindAll(p => p.spawnChance >= chance);
            selectedPiece = pieces[Random.Range(0, pieces.Count)].PreferredPieceName.ToString();
        }
        else
        {
            // gets the highest most likely piece to be selected
            selectedPiece = tempPreferred.OrderByDescending(p => p.spawnChance).First().PreferredPieceName.ToString();
        }

        if(selectedPiece == "")
        {
            print($"{pieceName} has an error");
            int num = 0;
            for (int i = 0; i < tempPreferred.Count; i++)
            {
                if (PreferredPieces.Contains(tempPreferred[i]))
                {
                    num++;
                }
            }

            print(num);
        }

        return selectedPiece;
    }

    public void DisablePiece()
    {
        Mesh.SetActive(false);
        coinsParent.gameObject.SetActive(false);
        children.ForEach(c => c.ResetTransform());
    }

    void getChildren()
    {
        var num = Mesh.transform.childCount;
        for (int i = 0; i < num; i++)
        {
            if (Mesh.transform.GetChild(i).GetComponent<PieceElements>() != null)
                children.Add(Mesh.transform.GetChild(i).GetComponent<PieceElements>());
        }
    }

    public void EnableMesh(bool enable)
    {
        Mesh.SetActive(enable);
        coinsParent.SetActive(enable);

        if (enable)
        {
            InEnable();
        }
    }

    public Vector3 GetEdge()
    {
        return transform.position + Vector3.forward * 60f;
    }

    public PieceNameEnum GetPieceType()
    {
        return pieceType;
    }

    public bool IsEnabled()
    {
        return Mesh.activeSelf;
    }
}