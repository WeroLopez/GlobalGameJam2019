using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class Carro : MonoBehaviour
{
    // Carro

    SpriteRenderer carSprite;
    Rigidbody2D carRigidBody;
    [SerializeField]
    Rigidbody2D prefabBala;
    [SerializeField]
    float moveSpeed;
    [SerializeField]
    bool derechaAIzquierda;
    [SerializeField]
    bool puedeDisparar;
    bool disparando = false;

    // Start is called before the first frame update
    void Start()
    {
        carRigidBody = GetComponent<Rigidbody2D>();
        carSprite = GetComponent<SpriteRenderer>();
        
        // El carro se mueve a la izquierda
        if (derechaAIzquierda)
        {
            carRigidBody.velocity = new Vector2(-moveSpeed, 0);
        } 
        // El carro se mueve a la derecha
        else
        {
            carRigidBody.velocity = new Vector2(moveSpeed, 0);
            carSprite.flipX = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (puedeDisparar)
        {
            if (!disparando)
            {
                if (derechaAIzquierda)
                {
                    StartCoroutine("DispararAbajo");
                }
                else
                {
                    StartCoroutine("DispararArriba");
                }
            }
        }
    }

    IEnumerator DispararAbajo()
    {
        disparando = true;
        Rigidbody2D test;
        test = (Instantiate(prefabBala, transform.position+1.0f*transform.forward,transform.rotation));
        test.AddRelativeForce(new Vector2(0, -400));
        yield return new WaitForSeconds(0.5f);
        disparando = false;
    }

    IEnumerator DispararArriba()
    {
        disparando = true;
        Rigidbody2D test;
        test = (Instantiate(prefabBala, transform.position+1.0f*transform.forward,transform.rotation));
        test.AddRelativeForce(new Vector2(0, 400));
        yield return new WaitForSeconds(0.5f);
        disparando = false;
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
