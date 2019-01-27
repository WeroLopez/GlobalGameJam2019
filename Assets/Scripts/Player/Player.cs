using System.Collections;
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
    public bool isJumping;
    [SerializeField]
    float jumpSpeed, jumpAmplitude;
    float jumpTime;
    public float jumpInitialY;

    //Agachar
    bool isDucking;
    bool tocandoBasura;
    bool cubierto;
    Vector3 basuraPos;

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
    bool immovilized;
    [SerializeField]
    Transform triggerCamara;

    [SerializeField]
    bool isAttacking = false;

    //Hitbox for hits
    [SerializeField]
    Vector2 hitbox1 = new Vector2(1.0f, 1.5f);
    [SerializeField]
    Vector2 hitbox2 = new Vector2(1.0f, -.25f);


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
        immovilized = false;
        basuraPos = Vector3.zero;
        jumpInitialY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Jump();
        Attack();
        Duck();
        //Cambia posicion de hitbox de ataque
        if (playerSpriteRenderer.flipX)
        {
            transform.GetChild(0).transform.position = new Vector2(transform.position.x - hitbox1.x, transform.position.y - hitbox1.y);
            transform.GetChild(1).transform.position = new Vector2(transform.position.x - hitbox2.x, transform.position.y - hitbox2.y);
        }
        else
        {
            transform.GetChild(0).transform.position = new Vector2(transform.position.x + hitbox1.x, transform.position.y - hitbox1.y);
            transform.GetChild(1).transform.position = new Vector2(transform.position.x + hitbox2.x, transform.position.y - hitbox2.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.name == "Limite")
        {
            tocandoLimite = true;
        }
        else if (other.name.StartsWith("Basura"))
        {
            basuraPos = other.gameObject.transform.position;
            tocandoBasura = true;
        }
        
        else if (other.name.StartsWith("CasaPerro"))
        {
            var numeroAleatorio = Random.Range(0f, 100.0f);
            if (numeroAleatorio < 70)
            {
                StartCoroutine(AfraidOfDog());
                other.GetComponent<Collider2D>().enabled = false;
                other.GetComponent<SpriteRenderer>().color = Color.red;
                print("PERRO");
            }
            else
            {
                other.GetComponent<Collider2D>().enabled = false;
                other.GetComponent<SpriteRenderer>().color = Color.cyan;
            }
        }
        else if (other.name.StartsWith("Oxxo"))
        {
            StartCoroutine(Pistear());
            other.GetComponent<Collider2D>().enabled = false;
            other.GetComponent<SpriteRenderer>().color = Color.green;
        }else if (other.name.StartsWith("Cholo"))
        {
            other.gameObject.GetComponent<Enemy>().hitpoints -= 1;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.name.StartsWith("Basura"))
        {
            tocandoBasura = false;
        }
        else if (other.name.StartsWith("Chicle"))
        {
            StartCoroutine(StuckOnGum());
            Destroy(other.gameObject);
            print("ATORADO EN EL CHICLE");
        }
    }

    private void Attack()
    {
        //Ataque B
        if (Controllers.GetFire(2, 1))
        {
            isAttacking = true;
            playerSpriteRenderer.color = Color.red;
            if (isDucking)
            {
                transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                transform.GetChild(0).gameObject.SetActive(true);
            }
        }
        if (Controllers.GetFire(2, 2))
        {
            isAttacking = false;
            playerSpriteRenderer.color = Color.white;
            RefreshStress(2f);
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    private void Jump()
    {
        //Saltar A 
        if (!isJumping)
        {
            if (Controllers.GetFire(1, 2) && !immovilized)
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
            if (!immovilized)
            {
                playerSpriteRenderer.color = Color.blue;
                isDucking = true;
                if (tocandoBasura)
                {
                    cubierto = true;
                }
            }
        }
        if (Controllers.GetFire(3, 2))
        {
            if (!immovilized)
            {
                playerSpriteRenderer.color = Color.white;
                isDucking = false;
                cubierto = false;
            }
        }
    }

    private void Move()
    {
        //Si no esta saltando o atorado en el chicle, moverse
        if (!isJumping && !isDucking && !immovilized)
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

    IEnumerator Pistear()
    {
        // El personaje se toma una cerveza del Oxxo para bajar el estrés.
        immovilized = true;
        yield return new WaitForSeconds(1f);
        immovilized = false;
        RefreshStress(-30);
    }

    IEnumerator StuckOnGum()
    {
        // El personaje se atora en el chicle 1s.
        immovilized = true;
        yield return new WaitForSeconds(1f);
        immovilized = false;
        RefreshStress(10);
    }

    IEnumerator AfraidOfDog()
    {
        // Sale el perro de la casa inmovilizando al personaje 2s.
        immovilized = true;
        yield return new WaitForSeconds(2f);
        immovilized = false;
        RefreshStress(10);
    }

    public void Plomazo(bool disparoHaciaArriba)
    {
        if (!cubierto)
        {
            RefreshStress(10);
        }
        else
        {
            print("basuraPos: " + basuraPos);
            print("playerPos: " + transform.position);
            if (!((disparoHaciaArriba && (transform.position.z > basuraPos.z)) || (!disparoHaciaArriba && (transform.position.z < basuraPos.z))))
            {
                RefreshStress(10);
            }
        }
    }

    public void RefreshStress(float stressChange)
    {
        stressValue += stressChange;
        if (stressValue < 0)
        {
            stressValue = 0;
        }
        
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
