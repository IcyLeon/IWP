using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private float FlightTime = 10f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, FlightTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerCharacters>() == null)
        {
            Destroy(gameObject);
        }
    }
}
