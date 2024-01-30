using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] Button PlayButton, OptionsButton;


    // Start is called before the first frame update
    void Start()
    {
        PlayButton.onClick.AddListener(Play);
        OptionsButton.onClick.AddListener(Options);
    }

    void Play()
    {
        SceneManager s = SceneManager.GetInstance();
        s.ChangeScene(SceneEnum.SHOP);
    }

    void Options()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
