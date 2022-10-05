using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fireboy
{
    public class FanCtrl : SwitchEvent
    {
        private Animator _myAnim;

        // Start is called before the first frame update
        void Start()
        {
            _myAnim = this.GetComponent<Animator>();
        }

        public override void SwitchOn()
        {
            _myAnim.SetBool("open", true);
        }

        public override void SwitchOff()
        {
            _myAnim.SetBool("open", false);
        }
    }
}