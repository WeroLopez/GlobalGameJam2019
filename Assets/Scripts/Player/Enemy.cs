using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoreGame.SystemControls;


#pragma warning disable 0649
public class Enemy : MonoBehaviour
{
    //Enemy
    Rigidbody2D enemyRigidBody;
    SpriteRenderer enemySpriteRenderer;
    //Si esta activo el enemigo
    [SerializeField]
    bool active = false;
    [SerializeField]
    float moveSpeed = 4;
    //Distancia a la cual se acerca, default es 1.4
    [SerializeField]
    float distance = 1.4f;
    //Daño de stress que hace el enemigo 
    [SerializeField]
    float damage = 5;
    //Tiempo que dura el ataque
    [SerializeField]
    float attackTime = 1f;

    //Variables que se usan para saber en que estado esta el enemigo
    [SerializeField]
    bool close = false;
    [SerializeField]
    bool attacking = false;
    
    bool isJumping=false;


    Player player;
    GameObject player1;
    SpriteRenderer enemyrenderer;

    [SerializeField]
    public int hitpoints = 3;
    Vector3 OGrot;

    // Update is called once per frame
    void Start()
    {
        player1 = GameObject.Find("Player");
        player = player1.GetComponent<Player>();
        enemyRigidBody = GetComponent<Rigidbody2D>();
        enemyrenderer = GetComponent<SpriteRenderer>();
        OGrot = transform.rotation.eulerAngles;

    }
    void Update()
    {
        //Con esto rota el mono junto con sus hitbox de golpexx
       if((player.transform.position.x - transform.position.x) > 0)
        {
            //enemyrenderer.flipX = false;
            Vector3 rot = transform.rotation.eulerAngles;
            rot = new Vector3(rot.x, OGrot.y +180, rot.z);
            transform.rotation = Quaternion.Euler(rot);
        }
        else
        {
           // enemyrenderer.flipX = true;
            Vector3 rot = transform.rotation.eulerAngles;
            rot = new Vector3(rot.x, OGrot.y, rot.z);
            transform.rotation = Quaternion.Euler(rot);
        }
        Movement();
        Attack();
        Dead();
        
    }

    void Movement()
    {
        //El enemigo solo se mueve si activo == true y no esta atacando
        if (active && !attacking)
        {
            float dist1 = Vector2.Distance(new Vector2(player.transform.position.x + distance, player.transform.position.y), new Vector2(this.transform.position.x, this.transform.position.y));
            float dist2 = Vector2.Distance(new Vector2(player.transform.position.x - distance, player.transform.position.y), new Vector2(this.transform.position.x, this.transform.position.y));

            //Debug.Log("Distancia1: "+dist1+" Distancia2: "+dist2);
            Vector2 pos1 = new Vector2(player.transform.position.x + distance, player.transform.position.y);
            Vector2 pos2 = new Vector2(player.transform.position.x - distance, player.transform.position.y);

            Vector2 pos3 = new Vector2(player.transform.position.x + distance, player.jumpInitialY);
            Vector2 pos4 = new Vector2(player.transform.position.x - distance, player.jumpInitialY);
            
            
            //Si el jugador esta brincando, no sigue su posicion de brinco si no la anterior al brinco
            if (!player.isJumping) {
                if (dist1 < dist2)
                {
                    transform.position = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), pos1, moveSpeed * Time.deltaTime);
                    

                }
                else
                {
                    transform.position = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), pos2, moveSpeed * Time.deltaTime);
                }
            }
            else
            {
                if (dist1 < dist2)
                {
                    transform.position = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), pos3, moveSpeed * Time.deltaTime);

                }
                else
                {
                    transform.position = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), pos4, moveSpeed * Time.deltaTime);
                }
            }
            //Si  las distancias de los 2 puntos laterales son 0, entonces true.
            if ((dist1 == 0 || dist2 == 0) )
            {
                close = true;
            }
            else
            {
                close = false;
            }
        }
    }
    void Attack()
    {
        //Solo ataca si esta cerca
        if (close)
        {
            //No ataca de nuevo si esta actualmente atacando
            if(!attacking) { 
            StartCoroutine(waitAttack());
            }
        }
    }

    IEnumerator waitAttack()
    {
        //Ataque al azar, agarra los objetos hijo del cholo, Alto y Bajo
        attacking = true;
        int r = Random.Range(0, 2);
 
        transform.GetChild(r).gameObject.SetActive(true);
        
        enemyRigidBody.velocity = Vector3.zero;
        yield return new WaitForSeconds(attackTime);
        transform.GetChild(r).gameObject.SetActive(false);  
        attacking = false;
        
    }
    public void SetCollidersStatus(bool active, string Collider)
    {
        Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            if (collider.name == Collider)
            {
                //    Debug.Log("The collider name to disable is: " + collider.name);
                collider.enabled = active;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Si el trigger entra, le hace daño al jugador
        if(collision.name == "Player")
        {
            player.RefreshStress(damage);
        }
    }

    private void Dead()
    {
        if(hitpoints < 0) { 
        this.gameObject.SetActive(false);
        }
    }

}
