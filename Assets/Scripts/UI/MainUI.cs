using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUI : MonoBehaviour
{
    private static MainUI instance;

    [SerializeField] GameObject PlayerHealthBarREF;

    public static MainUI GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
            
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public HealthBarScript GetPlayerHealthBar()
    {
        return PlayerHealthBarREF.GetComponent<HealthBarScript>();
    }
}
