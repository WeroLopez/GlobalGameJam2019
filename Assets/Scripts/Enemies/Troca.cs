using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class Troca : MonoBehaviour
{
    // Troca

    Rigidbody2D carRigidBody;
    [SerializeField]
    Rigidbody2D prefabBala;
    [SerializeField]
    bool disparaArriba;
    [SerializeField]
    float moveSpeed;
    bool disparando = false;

    // Start is called before the first frame update
    void Start()
    {
        carRigidBody = GetComponent<Rigidbody2D>();
        
        // La troca se mueve a la izquierda
        carRigidBody.velocity = new Vector2(-moveSpeed, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (!disparando)
        {
            if (disparaArriba)
            {
                StartCoroutine("DispararArriba");
            }
            else
            {
                StartCoroutine("DispararAbajo");
            }
        }
    }

    IEnumerator DispararAbajo()
    {
        disparando = true;
        Rigidbody2D test;
        test = (Instantiate(prefabBala, transform.position+1.0f*transform.forward,transform.rotation));
        test.AddRelativeForce(new Vector2(0, -400));
        yield return new WaitForSeconds(0.1f);
        disparando = false;
    }

    IEnumerator DispararArriba()
    {
        disparando = true;
        Rigidbody2D test;
        test = (Instantiate(prefabBala, transform.position+1.0f*transform.forward,transform.rotation));
        test.AddRelativeForce(new Vector2(0, 400));
        yield return new WaitForSeconds(0.1f);
        disparando = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // La troca choca con el jugador
        if (other.name.StartsWith("Player"))
        {
            print("COLISION CON TROCA");
        }
    }
}
