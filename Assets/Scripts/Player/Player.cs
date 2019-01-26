﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoreGame.SystemControls;
using UnityEngine.UI;

#pragma warning disable 0649

public class Player : MonoBehaviour
{
    //Player
    Rigidbody2D playerRigidBody;
    SpriteRenderer playerSpriteRenderer;
    BoxCollider2D footCollider;
    Animator playerAnimator;

    //Move
    Vector2 leftJoystick;
    [SerializeField]
    float moveSpeed;

    //Jump
    bool isJumping;
    [SerializeField]
    float jumpSpeed, jumpAmplitude;
    float jumpTime;
    float jumpInitialY;

    //Agachar
    bool isDucking;
    bool tocandoBasura;
    bool cubierto;

    //StressBar
    float stressValue;
    [SerializeField]
    float maxStressValue;
    [SerializeField]
    GameObject stressBar;
    Image stressBarValue;

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
        playerAnimator = GetComponent<Animator>();
        //estres
        stressBarValue = stressBar.transform.GetChild(1).GetComponent<Image>();
        stressValue = 0;
        RefreshStress(0f);
        //Camara
        camara = Camera.main;
        camY = camara.transform.position.y;
        //Jump
        isJumping = false;
        footCollider = GetComponent<BoxCollider2D>();
        jumpTime = 0;
        //Agachar
        isDucking = false;
        cubierto = false;
        tocandoBasura = false;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Jump();
        Attack();
        Duck();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.name == "Limite")
        {
            tocandoLimite = true;
        }
        else if (other.name.StartsWith("Basura"))
        {
            tocandoBasura = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.name.StartsWith("Basura"))
        {
            tocandoBasura = false;
        }
    }

    private void Attack()
    {
        //Ataque B
        if (Controllers.GetFire(2, 1))
        {
            playerSpriteRenderer.color = Color.red;
        }
        if (Controllers.GetFire(2, 2))
        {
            playerSpriteRenderer.color = Color.white;
            RefreshStress(2f);
        }
    }

    private void Jump()
    {
        //Saltar A 
        if (!isJumping)
        {
            if (Controllers.GetFire(1, 2))
            {
                isJumping = true;
                footCollider.enabled = false;
                jumpTime = 0;
                jumpInitialY = transform.position.y;
                playerSpriteRenderer.color = Color.cyan;
                StartCoroutine(Jumping());
            }
        }
        else
        {
            jumpTime += Time.deltaTime;
            transform.position = new Vector3(
                transform.position.x, 
                transform.position.y + Mathf.Sin(jumpSpeed * jumpTime) * jumpAmplitude, 
                transform.position.z);
        }
    }

    private void Duck()
    {
        //Agachar X 
        if (Controllers.GetFire(3, 1))
        {
            playerSpriteRenderer.color = Color.blue;
            isDucking = true;
            if (tocandoBasura)
            {
                cubierto = true;
            }
        }
        if (Controllers.GetFire(3, 2))
        {
            playerSpriteRenderer.color = Color.white;
            isDucking = false;
            cubierto = false;
        }
    }

    private void Move()
    {
        //Si no esta saltando, moverse
        if (!isJumping && !isDucking)
        {
            leftJoystick = Controllers.GetJoystick(1, 1);
            playerAnimator.SetFloat("Velocity", Mathf.Abs(leftJoystick.x) + Mathf.Abs(leftJoystick.y));
            
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
                    camara.transform.position.x, camY, camara.transform.position.z);
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
            //Mover la z
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
        }
        //Si esta saltando, no moverse
        else
        {
            //que no se mueva la camara si saltas y estas pegado
            if(transform.position.x > triggerCamara.transform.position.x)
            {
                camara.transform.parent = null;
            }
            playerRigidBody.velocity = Vector2.zero;
        }
    }
    
    IEnumerator Jumping()
    {
        yield return new WaitForSeconds(.4f);
        if (isJumping)
        {
            isJumping = false;
            footCollider.enabled = true;
            transform.position = new Vector3(transform.position.x, jumpInitialY, transform.position.z);
            playerSpriteRenderer.color = Color.white;
        }
    }

    public void RefreshStress(float stressChange)
    {
        stressValue += stressChange;
        
        if (stressValue >= maxStressValue)
        {
            print("Perdiste");
        }
        else
        {
            stressBarValue.fillAmount = stressValue / maxStressValue;
        }
    }

}
