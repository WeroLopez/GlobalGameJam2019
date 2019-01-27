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

    bool muerto;

    [SerializeField]
    Sprite spritePerro;

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
    float limiteInferior;
    float limiteSuperior;
    bool tocandoLimite;
    bool immovilized;
    [SerializeField]
    Transform triggerCamara;

    [SerializeField]
    bool isAttacking = false;
    [SerializeField]
    float attackTime = 1f;
    [SerializeField]
    float hurtTime = 1f;

    [SerializeField]
    public bool isHurt = false;

    //Hitbox for hits
    [SerializeField]
    Vector2 hitbox1 = new Vector2(1.0f, 1.5f);
    [SerializeField]
    Vector2 hitbox2 = new Vector2(1.0f, -.25f);
    //Knockback and InitialPosition for knockback
    [SerializeField]
    float knockbackTotal = 3f;
    [SerializeField]
    float knockback = .1f;
    [SerializeField]
    Vector2 initialPosition;
    bool invulnerable = false;

    //Direccion del knockbak a donde se movera el enemigo, true es izquierda y false derecha
    public bool knockbackdirection = true;

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
        limiteInferior = -4.0f;
        limiteSuperior = 2.4f;
        //jumpInitialY = transform.position.y;
        muerto = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isHurt)
        {
            Move();
            Jump();
            Attack();
            Duck();
        }
        else
        {
            // Knockback(knockback,knockbackTotal);
            //  stressValue = initialStress;
            StartCoroutine(waitHurt());
        }
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
                other.GetComponent<SpriteRenderer>().sprite = spritePerro;
                print("PERRO");
            }
            else
            {
                other.GetComponent<Collider2D>().enabled = false;
            }
        }
        else if (other.name.StartsWith("Oxxo"))
        {
            StartCoroutine(Pistear());
            other.GetComponent<Collider2D>().enabled = false;
        }else if (other.name.StartsWith("Cholo"))
        {
            other.gameObject.GetComponent<Enemy>().hitpoints -= 1;
        }
        else if (other.name.StartsWith("Chicle"))
        {
            StartCoroutine(StuckOnGum());
            Destroy(other.gameObject);
            print("ATORADO EN EL CHICLE");
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
            playerAnimator.SetTrigger("Punch");
            isAttacking = true;
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
                playerAnimator.SetTrigger("Jump");
                isJumping = true;
                footCollider.enabled = false;
                jumpTime = 0;
                jumpInitialY = transform.position.y;
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
                playerAnimator.SetBool("Duck", true);
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
                playerAnimator.SetBool("Duck", false);
                isDucking = false;
                cubierto = false;
            }
        }
    }

    private void Move()
    {
        //Si no esta saltando o atorado en el chicle, moverse
        if (!isJumping && !isDucking && !immovilized && !muerto)
        {
            leftJoystick = Controllers.GetJoystick(1, 1);
            playerAnimator.SetFloat("Velocity", Mathf.Abs(leftJoystick.x) + Mathf.Abs(leftJoystick.y));
            
            //No moverse hacia atras cuando toca el limite
            if (tocandoLimite && leftJoystick.x < 0)
            {
                if ((transform.position.y > limiteInferior || leftJoystick.y > 0) && (transform.position.y < limiteSuperior || leftJoystick.y < 0))
                {
                    playerRigidBody.velocity = new Vector2(0, leftJoystick.y) * moveSpeed;
                }
                else
                {
                    playerRigidBody.velocity = new Vector2(0, 0);
                }
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
                if ((transform.position.y > limiteInferior || leftJoystick.y > 0) && (transform.position.y < limiteSuperior || leftJoystick.y < 0))
                {
                    playerRigidBody.velocity = new Vector2(leftJoystick.x, leftJoystick.y) * moveSpeed;
                }
                else
                {
                    playerRigidBody.velocity = new Vector2(leftJoystick.x, 0) * moveSpeed;
                }
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
        playerAnimator.SetTrigger("Chicle");
        immovilized = true;
        yield return new WaitForSeconds(1f);
        immovilized = false;
        RefreshStress(10);
    }

    IEnumerator AfraidOfDog()
    {
        // Sale el perro de la casa inmovilizando al personaje 2s.
        playerAnimator.SetTrigger("Scared");
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
            playerAnimator.SetTrigger("Hit");
        }
        else
        {
            print("basuraPos: " + basuraPos);
            print("playerPos: " + transform.position);
            if (!((disparoHaciaArriba && (transform.position.z > basuraPos.z)) || (!disparoHaciaArriba && (transform.position.z < basuraPos.z))))
            {
                RefreshStress(10);
                playerAnimator.SetTrigger("Hit");
            }
        }
    }

    public void RefreshStress(float stressChange)
    {
        if (!invulnerable) { 
            stressValue += stressChange;
            if (stressValue < 0)
            {
                stressValue = 0;
            }
        
            if (stressValue >= maxStressValue)
            {
                print("Perdiste");
                muerto = true;
                playerAnimator.SetTrigger("Death");
            }
            else
            {
                stressBarValue.fillAmount = stressValue / maxStressValue;
            }
        }
    }
    IEnumerator waitAttack()
    {
        isAttacking = true;
        yield return new WaitForSeconds(attackTime);
        isAttacking = false;
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);

    }
    //Activa los hitboxs, el index es el numero de hijo a activar/desactivar, el activate es si se activa o desactiva y el all es si es a todos
    void activateHitbox(int index, bool activate, bool all)
    {
        if (all)
        {
            transform.GetChild(0).gameObject.SetActive(activate);
            transform.GetChild(1).gameObject.SetActive(activate);
        }
        else
        {
            transform.GetChild(index).gameObject.SetActive(activate);
        }
    }
    public void Knockback(float knockback, float knockbackTotal)
    {
        float newknockback;
        if (knockbackdirection)
        {
            newknockback = -knockbackTotal;
        }
        else
        {
            newknockback = knockbackTotal;
        }
        Vector2 newPos = new Vector2(initialPosition.x + newknockback, initialPosition.y);
        //transform.position = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), newPos, moveSpeed * Time.deltaTime);
        playerRigidBody.velocity = new Vector3(transform.position.x + knockback, 0, 0) * moveSpeed * Time.deltaTime;
        float dist = Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(newPos.x, newPos.y));
        if (dist < 1)
        {
            isHurt = false;
            playerRigidBody.velocity = Vector3.zero;
        }
    }
    public void setInitialKnockback()
    {
        initialPosition = new Vector2(transform.position.x, transform.position.y);
    }

    IEnumerator waitHurt()
    {
        footCollider.enabled = false;
        invulnerable = true;
        playerAnimator.SetTrigger("Hit");
        yield return new WaitForSeconds(hurtTime);
        footCollider.enabled = true;
        invulnerable = false;
        isHurt = false;
    }
}


