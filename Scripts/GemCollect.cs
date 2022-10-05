using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fireboy
{
    public class GemCollect : MonoBehaviour
    {
        public Player GemType;
        public ParticleSystem FXCollect;

        public void OnCollect()
        {
            this.GetComponent<Collider2D>().enabled = false;
            this.transform.GetChild(0).gameObject.SetActive(false);
            FXCollect.Play();
            Invoke("OnHide", 1.5f);
        }

        private void OnHide()
        {
            this.gameObject.SetActive(false);
        }
    }
}