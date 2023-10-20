using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ArtifactsInfoSO", menuName = "ScriptableObjects/ArtifactsInfoSO")]
public class ArtifactsInfo : ScriptableObject
{
    public ArtifactsSet ArtifactSet;
    public string ArtifactsSetName;
    public ArtifactsSO[] artifactSOList;
    [TextAreaAttribute]
    public string TwoPieceDesc, FourPieceDesc;
}
