using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Level : ScriptableObject
{
    [SerializeField] private string Name;
    [SerializeField] private LevelPiece.PieceNameEnum[] levelPieces;

    public int GetLevelSize()
    {
        return levelPieces.Length;
    }

    public LevelPiece.PieceNameEnum GetPieceName(int index)
    {
        return levelPieces[index];
    }
}
