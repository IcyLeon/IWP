using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public enum ArtifactType
{
    FLOWER,
    PLUME,
    SANDS,
    GOBLET,
    CIRCLET
}
public enum ArtifactsSet
{
    THUNDERING_FURY,
    NOBLESSE_OBLIGE
}

[CreateAssetMenu(fileName = "ArtifactsListInfoSO", menuName = "ScriptableObjects/ArtifactsListInfoSO")]
public class ArtifactsListInfo : ScriptableObject
{
    [System.Serializable]
    public class CommonArtifactPiece
    {
        public ArtifactType artifactType;
        public string artifactPieceName;
    }

    [Header("Artifacts Set Information")]
    public ArtifactsInfo[] artifactsInfo;

    [Header("Artifacts Piece Name")]
    public CommonArtifactPiece[] artifactsInfoTypeName;
}
