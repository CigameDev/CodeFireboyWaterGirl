using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fireboy
{
    public class MarbleCtrl : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            this.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if(collision.gameObject.tag == TagDefine.FIRE_BOY || collision.gameObject.tag == TagDefine.WATER_GIRL)
            {
                this.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            }
        }
    }
}
