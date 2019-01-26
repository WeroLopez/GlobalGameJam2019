using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoreGame.SystemControls;

public class Player : MonoBehaviour
{
    //Player
    Rigidbody2D playerRigidBody;
    SpriteRenderer playerSpriteRenderer;
    Vector2 leftJoystick;

    [SerializeField]
    float moveSpeed;
    
    //Camara
    Camera camara;
    float camY;
    bool tocandoLimite;
    bool tocandoCamara;

    // Start is called before the first frame update
    void Start()
    {
        playerRigidBody = GetComponent<Rigidbody2D>();
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        camara = Camera.main;
        camY = camara.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        leftJoystick = Controllers.GetJoystick(1, 1);

        if (tocandoLimite && leftJoystick.x < 0)
        {
            playerRigidBody.velocity = new Vector2(0, leftJoystick.y) * moveSpeed;
        }
        else if (tocandoLimite && leftJoystick.x > 0)
        {
            tocandoLimite = false;
            playerRigidBody.velocity = new Vector2(leftJoystick.x, leftJoystick.y) * moveSpeed;
        }
        else
        {
            playerRigidBody.velocity = new Vector2(leftJoystick.x, leftJoystick.y) * moveSpeed;
        }
        
        //Flipear el sprite
        if (leftJoystick.x > 0)
        {
            playerSpriteRenderer.flipX = false;
        }
        else if (leftJoystick.x < 0)
        {
            playerSpriteRenderer.flipX = true;
        }


        if (tocandoCamara && leftJoystick.x < 0)
        {
            tocandoCamara = false;
        }
        else if(tocandoCamara && leftJoystick.x > 0)
        {
            camara.transform.position = new Vector2(camara.transform.position.x, camY);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.name == "CamaraEnfrente")
        {
            tocandoCamara = true;
        }
        else if(other.name == "Limite")
        {
            tocandoLimite = true;
        }
    }
}
