using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI DamageTextTMP;
    [SerializeField] WorldTextSO worldTextSO;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public IEnumerator WorldTextAnim()
    {
        while (true)
        {

            yield return null;
        }
    }
}
