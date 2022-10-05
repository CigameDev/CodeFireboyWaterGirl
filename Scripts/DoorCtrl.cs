using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fireboy
{
    public class DoorCtrl : MonoBehaviour
    {
        public Player DoorType;

        private Animator _myAnim;

        // Start is called before the first frame update
        void Start()
        {
            _myAnim = this.GetComponent<Animator>();
        }

        public void OnOpenDoor(bool open)
        {
            _myAnim.SetBool("open", open);
        }
    }
}