using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactSetInfo : MonoBehaviour
{
    [Serializable]
    public class ArtifactSetInfoContent
    {
        public Image CheckImage;
        public TextMeshProUGUI ArtifactPieceText;
    }

    private ArtifactsManager AM;
    //[SerializeField] Image CheckImage_2Piece;
    //[SerializeField] Image CheckImage_4Piece;
    [SerializeField] TextMeshProUGUI ArtifactSetText;
    [SerializeField] ArtifactSetInfoContent[] artifactSetInfoContent;
    //[SerializeField] TextMeshProUGUI Artifact2PieceText;
    //[SerializeField] TextMeshProUGUI Artifact4PieceText;


    [Header("Marker Indication")]
    [SerializeField] Sprite CheckSprite;
    [SerializeField] Sprite UnCheckSprite;

    [Header("Color Indication")]
    [SerializeField] Color SuccessColorText;
    [SerializeField] Color DefaultColorText;
    [SerializeField] Color SuccessColorCheckerImage;
    [SerializeField] Color DefaultColorCheckerImage;

    private ArtifactsInfo ArtifactsInfo;

    private void Start()
    {
        AM = ArtifactsManager.GetInstance();
    }
    public void SetArtifactSO(ArtifactsInfo AI)
    {
        ArtifactsInfo = AI;

        if (ArtifactsInfo == null)
            return;

        if (ArtifactSetText)
            ArtifactSetText.text = ArtifactsInfo.ArtifactsSetName + ":";

        if (artifactSetInfoContent[0] != null)
            artifactSetInfoContent[0].ArtifactPieceText.text = "2-Piece Set: " + ArtifactsInfo.TwoPieceDesc;
        if (artifactSetInfoContent[1] != null)
            artifactSetInfoContent[1].ArtifactPieceText.text = "4-Piece Set: " + ArtifactsInfo.FourPieceDesc;
    }

    // Update is called once per frame
    public void UpdateContent(CharacterData characterData)
    {
        if (ArtifactsInfo == null)
            return;

        if (AM == null)
            AM = ArtifactsManager.GetInstance();

        if (characterData != null)
        {
            int length = AM.GetTotalDuplicatePieceCount(characterData, ArtifactsInfo);

            for (int i = 0; i < artifactSetInfoContent.Length; i++)
            {
                ArtifactSetInfoContent a = artifactSetInfoContent[i];
                a.CheckImage.sprite = UnCheckSprite;
                a.CheckImage.color = DefaultColorCheckerImage;
                a.ArtifactPieceText.color = DefaultColorText;
            }

            for (int i = 0; i < artifactSetInfoContent.Length; i++)
            {
                ArtifactSetInfoContent a = artifactSetInfoContent[i];
                if (i < length)
                {
                    a.CheckImage.sprite = CheckSprite;
                    a.CheckImage.color = SuccessColorCheckerImage;
                    a.ArtifactPieceText.color = SuccessColorText;
                }
            }
        }
    }
}
