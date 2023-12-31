using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyContent
{
    public ScriptableObject[] PossibleItemsList;
    public CharactersSO EnemySO;
    public GameObject EnemyPrefab;
    public int MaxItemToGive;
}

[CreateAssetMenu(fileName = "EnemyInfoSO", menuName = "ScriptableObjects/EnemyInfoSO")]
public class EnemyInfo : ScriptableObject
{
    [SerializeField] EnemyContent[] EnemyContentList;

    public EnemyContent[] GetEnemyContentList()
    {
        return EnemyContentList;
    }
    public EnemyContent GetEnemyContent(CharactersSO EnemySO)
    {
        for(int i = 0; i < EnemyContentList.Length; i++)
        {
            EnemyContent enemyContent = EnemyContentList[i];
            if (enemyContent.EnemySO == EnemySO)
                return enemyContent;
        }
        return null;
    }


}
