using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ArtifactsListInfo;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public static GameManager GetInstance()
    {
        return instance;
    }
}
