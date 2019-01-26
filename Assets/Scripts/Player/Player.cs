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
    [SerializeField]
    Transform triggerCamara;

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

        //No moverse hacia atras cuando toca el limite
        if (tocandoLimite && leftJoystick.x < 0)
        {
            playerRigidBody.velocity = new Vector2(0, leftJoystick.y) * moveSpeed;
        }
        //Moverse hacia enfrente si toca el limite
        else if (tocandoLimite && leftJoystick.x > 0)
        {
            tocandoLimite = false;
            playerRigidBody.velocity = new Vector2(leftJoystick.x, leftJoystick.y) * moveSpeed;
        }
        //Moverse
        else
        {
            playerRigidBody.velocity = new Vector2(leftJoystick.x, leftJoystick.y) * moveSpeed;
        }
        
        if (leftJoystick.x > 0)
        {
            //Flipear el sprite
            playerSpriteRenderer.flipX = false;
            //mover la camara hacia enfrente
            if (transform.position.x > triggerCamara.transform.position.x)
            {
                camara.transform.parent = transform;
                camara.transform.position = new Vector3(
                    camara.transform.position.x,camY, camara.transform.position.z);
            }
        }
        else if (leftJoystick.x < 0)
        {
            //Flipear el sprite
            playerSpriteRenderer.flipX = true;
            //dejar de mover la camara
            camara.transform.parent = null;
        }
        else if (leftJoystick.x == 0)
        {
            //dejar de mover la camara
            camara.transform.parent = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.name == "Limite")
        {
            tocandoLimite = true;
        }
    }
}
