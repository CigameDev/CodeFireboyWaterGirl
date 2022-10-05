using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fireboy
{
    public class FanPush : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Rigidbody2D rigid = collision.GetComponent<Rigidbody2D>();
            if (rigid)
            {
                rigid.AddForce(Vector2.right * 1000f);
            }
        }
    }
}
