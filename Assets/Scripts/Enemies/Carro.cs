using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carro : MonoBehaviour
{
    // Carro

    Rigidbody2D carRigidBody;
    [SerializeField]
    float moveSpeed;
    [SerializeField]
    bool derechaAIzquierda;

    // Start is called before the first frame update
    void Start()
    {
        carRigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!derechaAIzquierda) {
            carRigidBody.velocity = new Vector2(-moveSpeed, 0);
        } 
        else {
            carRigidBody.velocity = new Vector2(moveSpeed, 0);
        }
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name.StartsWith("Player"))
        {
            print("COLISION");
        }
    }
}
