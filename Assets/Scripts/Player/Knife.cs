using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoreGame.SystemControls;

public class Knife : MonoBehaviour
{
    [SerializeField]
    bool throwing = false;
    [SerializeField]
    float movespeed = 0;
    [SerializeField]
    float angle = 0;
    [SerializeField]
    float radius = 0;
    Collider2D knifecollider;
    [SerializeField]
    bool dropped = false;
    [SerializeField]
    float tolerance = -4.5f;
    Rigidbody2D kniferigidbody;
    [SerializeField]
    bool nearplayer = false;
    private void Start()
    {
        kniferigidbody = GetComponent<Rigidbody2D>();
        knifecollider = GetComponent<Collider2D>();
    }
    // Update is called once per frame
    void Update()
    {
        Movement();
    }
    void Movement()
    {
        if (throwing  && !dropped)
        {
            float x = Mathf.Sin(angle) * radius;
            float y = Mathf.Cos(angle) * radius;
            Debug.Log("X: " + x + " Y: " + y);
            angle -= movespeed * Time.deltaTime;
            kniferigidbody.velocity = new Vector2(x,y );
            if (y < tolerance)
            {
                dropped = true;
                kniferigidbody.velocity = new Vector2(0,0);
            }
        }
    }
    void picked()
    {
        if (Controllers.GetFire(3, 1) && nearplayer)
        {
            Destroy(this.gameObject);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        nearplayer = true;
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        nearplayer = false;
    }



}
