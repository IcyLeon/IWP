using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    private Elements elements;
    private CharacterData BowCharacters;
    private float FlightTime = 10f;

    private float gravityScale = 1f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, FlightTime);
    }

    void FixedUpdate()
    {
        rb.AddForce(Physics.gravity * gravityScale, ForceMode.Acceleration);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetElements(Elements elements)
    {
        this.elements = elements;
    }
    private void OnTriggerEnter(Collider other)
    {
        PlayerCharacters player = other.transform.GetComponent<PlayerCharacters>();
    
        if (player == null)
        {
            if (other.gameObject.GetComponent<IDamage>() != null)
            {
                other.gameObject.GetComponent<IDamage>().TakeDamage(transform.position, elements, BowCharacters.GetDamage());
            }
            Destroy(gameObject);
        }
    }

    public void SetCharacterData(CharacterData characterData)
    {
        BowCharacters = characterData;
    }
}
