using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomLevelGenerator : MonoBehaviour
{
    public static RandomLevelGenerator Instance;

    [SerializeField] private LevelPiece lastCreatedPiece;
    [SerializeField] private LevelPiece pieceToRemove;

    [SerializeField] private List<LevelPiece> activePieces = new List<LevelPiece>();
    private List<LevelPiece> availablePieces = new List<LevelPiece>();

    private List<LevelPiece>[] piecesCategories;
    [SerializeField] private Transform levelParent;
    [SerializeField] private GameObject[] levelPiecesPrefabs;
    [SerializeField] private Transform playerDetector;
    //[SerializeField] private Transform buildingPoint;

    [SerializeField] private float pieceLength = 30f;
    [SerializeField] private int piecesPassedToIncreaseDifficulty = 25;
    private int piecesPassed = 0;
    private int difficulty = 0;
    [SerializeField] private float scoreMultiplierIncrementPerPiece = 1.1f;
    [SerializeField] private float scoreMultiplierIncrementPerDifficulty = 2f;
    [SerializeField] private int maxDifficulty = 5;

    [Header("Pooling options")]
    [Range(3, 15)] [SerializeField] private int maxActivePiecesNum = 3;
    [Range(2, 10)] [SerializeField] private int maxNummberPerPiece = 2;
    [Range(2, 10)] [SerializeField] private int numberOfPiecesCreatedAhead = 2;

    private void OnValidate()
    {
        if(numberOfPiecesCreatedAhead > maxActivePiecesNum - 3)
        {
            maxActivePiecesNum = numberOfPiecesCreatedAhead + 3;
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        pieceToRemove = lastCreatedPiece;
        lastCreatedPiece.Initiate();
        lastCreatedPiece.EnableMesh(true);
        activePieces.Add(lastCreatedPiece);
        createAllPieces();
        createFirstPieces();
    }

    private void createFirstPieces()
    {
        for (int i = 0; i < numberOfPiecesCreatedAhead; i++)
        {
            buildPiece();
        }
    }

    public void CreateNextPiece()
    {
        buildPiece();

        if(activePieces.Count > maxActivePiecesNum)
        {
            pieceToRemove.DisablePiece();
            availablePieces.Add(pieceToRemove);
            activePieces.Remove(pieceToRemove);
            // Debug.Break();
            pieceToRemove = activePieces[0];
        }
    }

    void buildPiece(string nameToCreate = "")
    {
        LevelPiece piece = null;

        if (nameToCreate != "")
        {
            piece = availablePieces.Find(p => p.pieceName == nameToCreate);
        }
        else
        {
            piece = availablePieces.Find(p => p.pieceName == lastCreatedPiece.NextPieceName(availablePieces));    
        }
        
        // in case some error was handled
        if (piece == null || activePieces.Contains(piece))
        {
            piece = availablePieces[Random.Range(0, availablePieces.Count)];
        }

        availablePieces.Remove(piece);
        activePieces.Add(piece);
        piece.EnableMesh(true);
        piece.SetToPosition(lastCreatedPiece.GetEdge());
        lastCreatedPiece = piece;
    }

    void createAllPieces()
    {
        int indexName = 0;

        for (int i = 0; i < maxNummberPerPiece; i++)
        {
            for (int j = 0; j < levelPiecesPrefabs.Length; j++)
            {
                indexName++;
                var piece = Instantiate(levelPiecesPrefabs[j], levelParent).GetComponent<LevelPiece>();
                piece.Initiate();
                piece.EnableMesh(false);
                piece.gameObject.name = "Piece " + indexName;
                availablePieces.Add(piece);
            }
        }

        piecesCategories = new List<LevelPiece>[maxDifficulty];

        for (int i = 0; i < piecesCategories.Length; i++)
        {
            piecesCategories[i] = availablePieces.FindAll(p => p.Difficulty == i + 1);
        }

        availablePieces.Clear();
        availablePieces.AddRange(piecesCategories[0]);
    }

    public void CreateRevivingSpot()
    {
        removeAllPieces();
        buildPiece(LevelPiece.PieceNameEnum.Empty.ToString());
        CreateNextPiece();
        playerMovement.Instance.SetPlayerAfterReviving(activePieces[0].transform.position);
    }

    private void removeAllPieces()
    {
        activePieces.ForEach(p => p.DisablePiece());
        activePieces.Clear();
    }

    public void IncreasePiecesPassed()
    {
        piecesPassed++;
        PlayerStats.Instance.UpdateMultiplier(scoreMultiplierIncrementPerPiece);
        
        if (piecesPassed % piecesPassedToIncreaseDifficulty == 0)
        {
            PlayerStats.Instance.UpdateMultiplier(scoreMultiplierIncrementPerDifficulty);
            difficulty++;
            if (difficulty <= maxDifficulty)
            {
                availablePieces.AddRange(piecesCategories[difficulty - 1]);
            }
        }
    }

    public int GetDifficulty()
    {
        return difficulty;
    }

    public float GetPieceLength()
    {
        return pieceLength;
    }
}
