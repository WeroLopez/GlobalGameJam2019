using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class Carro : MonoBehaviour
{
    // Carro

    Rigidbody2D carRigidBody;
    [SerializeField]
    float moveSpeed;
    [SerializeField]
    bool derechaAIzquierda;
    [SerializeField]
    bool puedeDisparar;

    // Start is called before the first frame update
    void Start()
    {
        carRigidBody = GetComponent<Rigidbody2D>();
        
        // El carro se mueve a la izquierda
        if (!derechaAIzquierda) {
            carRigidBody.velocity = new Vector2(-moveSpeed, 0);
        } 
        // El carro se mueve a la derecha
        else {
            carRigidBody.velocity = new Vector2(moveSpeed, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // El carro choca con el jugador
        if (other.name.StartsWith("Player"))
        {
            print("COLISION");
        }
    }
}
