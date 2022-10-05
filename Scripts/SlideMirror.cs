using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fireboy
{
    public class SlideMirror : MonoBehaviour
    {
        [SerializeField] private Transform _mirror;
        [SerializeField] private Transform _dot;
        [SerializeField] private float _limitDot;
        

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            float pos = Mathf.Clamp( _dot.localPosition.x,-_limitDot,_limitDot);
            _dot.localPosition = Vector3.right * pos;
            float angle = pos * 60f;
            _mirror.eulerAngles = Vector3.forward *( 90f - angle);
        }
    }
}