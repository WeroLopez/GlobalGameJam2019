using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649

namespace CoreGame
{
    namespace Instanciador
    {
        public class Instanciador : MonoBehaviour
        {
            [SerializeField]
            GameObject @object;
            SpriteRenderer sr;

            // Start is called before the first frame update
            void Start()
            {   
                sr = GetComponent<SpriteRenderer>();
            }

            // Update is called once per frame
            void Update()
            {

            }

            private void OnTriggerEnter2D(Collider2D other)
            {
                print(other.name);
                print(other.tag);
                if (other.name.StartsWith("Instanciador"))
                {
                    @object.SetActive(true);
                    sr.sprite = null;
                }
            }
        }
    }
}
