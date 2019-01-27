using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class Bala : MonoBehaviour
{
    Rigidbody2D bulletRigidBody;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // La bala choca con el jugador
        if (other.name.StartsWith("Player"))
        {
            Destroy(gameObject);
        }
    }
}