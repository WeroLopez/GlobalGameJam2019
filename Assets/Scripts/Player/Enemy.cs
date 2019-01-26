using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoreGame.SystemControls;

public class Enemy : MonoBehaviour
{
    //Enemy
    Rigidbody2D enemyRigidBody;
    SpriteRenderer enemySpriteRenderer;
    [SerializeField]
    bool active = false;
    [SerializeField]
    float moveSpeed;
    [SerializeField]
    float distance = 5;
    [SerializeField]
    float damage = 5;
    [SerializeField]
    float attackTime = .5f;
    [SerializeField]
    bool close = false;
    [SerializeField]
    bool attacking = false;

    [SerializeField]
    Player player;

    #pragma warning disable 0649

    // Update is called once per frame
    void Start()
    {
        Debug.Log("Player position: " + player.transform.position.x + "," + player.transform.position.y);
        Vector2 pos1 = new Vector2(player.transform.position.x + distance, player.transform.position.y);
        Vector2 pos2 = new Vector2(player.transform.position.x - distance, player.transform.position.y);

        float dist1 = Vector2.Distance(new Vector2(player.transform.position.x + distance, player.transform.position.y), new Vector2(this.transform.position.x, this.transform.position.y));
        float dist2 = Vector2.Distance(new Vector2(player.transform.position.x - distance, player.transform.position.y), new Vector2(this.transform.position.x, this.transform.position.y));

        Debug.Log("pos1: " + pos1+ " dist1 "+dist1);
        Debug.Log("pos2: " + pos2+ " dist2 " + dist2);
    }
    void Update()
    {
        Movement();
        Attack();
    }

    void Movement()
    {
        if (active)
        {
            float dist1 = Vector2.Distance(new Vector2(player.transform.position.x + distance, player.transform.position.y), new Vector2(this.transform.position.x, this.transform.position.y));
            float dist2 = Vector2.Distance(new Vector2(player.transform.position.x - distance, player.transform.position.y), new Vector2(this.transform.position.x, this.transform.position.y));

            //Debug.Log("Distancia1: "+dist1+" Distancia2: "+dist2);
            Vector2 pos1 = new Vector2(player.transform.position.x + distance, player.transform.position.y);
            Vector2 pos2 = new Vector2(player.transform.position.x - distance, player.transform.position.y);

         
           
            if (dist1 < dist2)
            {
                transform.position = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), pos1, moveSpeed * Time.deltaTime);
            }
            else
            {
                transform.position = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), pos2, moveSpeed * Time.deltaTime);
            }
            
            if(dist1 == 0 || dist2 == 0)
            {
                close = true;
               //        Debug.Log("Ta cerca");
            }
            else
            {
                close = false;
                //Debug.Log("Ta no    cerca");
                //Debug.Log("Distancia1: " + dist1 + " Distancia2: " + dist2);
            }
        }
    }
    void Attack()
    {
        if (close)
        {
            if(!attacking) { 
            StartCoroutine(waitAttack());
            }
        }
    }

    IEnumerator waitAttack()
    {
        attacking = true;
        int r = Random.Range(0, 2);
        transform.GetChild(r).gameObject.active = true;
        yield return new WaitForSeconds(attackTime);
        transform.GetChild(r).gameObject.active = false;  
        attacking = false;
        
    }
    void attackType()
    {
        
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);
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
}
