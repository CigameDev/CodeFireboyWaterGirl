using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class SetSkin : MonoBehaviour
{
    public SkeletonAnimation Boy;
    public SkeletonAnimation Girl;

    // Start is called before the first frame update
    void Start()
    {
        int skin = Random.Range(1, 29);
        Boy.skeleton.SetSkin($"Char/B{skin}");
        Girl.skeleton.SetSkin($"Char/G{skin}");
    }


}
