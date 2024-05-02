using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCreator : MonoBehaviour
{
    public static LevelCreator Instance;
    [SerializeField] private GameObject winningLine;
    [SerializeField] private List<GameObject> levelPiecesPrefabs = new List<GameObject>();
    [SerializeField] private Level level;
    private List<LevelPiece> levelPiecesCreated = new List<LevelPiece>();
    [SerializeField] private Transform playerDetecor;
    [SerializeField] private int pieceAheadNum = 3;
    [SerializeField] private int maxPiecesNum = 6;
    [SerializeField] private LevelPiece startingPiece;
    private int pieceIndex = 1;

    private void OnValidate()
    {
        if(pieceAheadNum > maxPiecesNum - 3)
        {
            maxPiecesNum = pieceAheadNum + 3;
        }
    }

    private void Awake()
    {
        Instance = this;
        playerDetecor.GetComponent<PlayerDetector>().IsRandomLevel(false);
    }

    private void Start()
    {
        startingPiece.Initiate();
        startingPiece.EnableMesh(true);

        createPieces();
        createStartingPieces();
    }

    private void createStartingPieces()
    {
        for (int i = 0; i < pieceAheadNum; i++)
        {
            levelPiecesCreated[pieceIndex].EnableMesh(true);
            pieceIndex++;
        }
    }

    public void EnableNewPiece()
    {
        if(pieceIndex == levelPiecesCreated.Count)
        {
            print("Placed all pieces");
            return;
        }

        levelPiecesCreated[pieceIndex].EnableMesh(true);
        pieceIndex++;

        if(levelPiecesCreated.FindAll(p => p.IsEnabled()).Count > maxPiecesNum)
        {
            levelPiecesCreated.FindAll(p => p.IsEnabled())[0].DisablePiece();
        }
    }

    private void createPieces()
    {
        Transform levelParent = new GameObject("Level").transform;

        levelPiecesCreated.Add(startingPiece);

        for (int i = 0; i < level.GetLevelSize(); i++)
        {
            var toBuild = levelPiecesPrefabs.Find(p => p.GetComponent<LevelPiece>().GetPieceType() == level.GetPieceName(i));
            
            if (toBuild == null)
            {
                print("piece that does not exist is " + level.GetPieceName(i).ToString());
            }

            var piece = Instantiate(toBuild, levelParent).GetComponent<LevelPiece>();
            piece.Initiate();
            piece.EnableMesh(false);
            piece.SetToPosition(levelPiecesCreated[i].GetEdge());
            levelPiecesCreated.Add(piece);
        }

        createAnEmptyWinningPiece();
    }

    private void createAnEmptyWinningPiece()
    {
        var piece = Instantiate(levelPiecesPrefabs.Find(p => 
        p.GetComponent<LevelPiece>().GetPieceType() == LevelPiece.PieceNameEnum.Empty)).GetComponent<LevelPiece>();
        piece.Initiate();
        piece.EnableMesh(false);
        var pos = levelPiecesCreated[levelPiecesCreated.Count - 1].GetEdge();
        piece.SetToPosition(pos);
        levelPiecesCreated.Add(piece);
        Instantiate(winningLine, piece.transform);
    }
}
