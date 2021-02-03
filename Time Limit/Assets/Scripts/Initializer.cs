using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initializer : MonoBehaviour
{
    private void Start()
    {
        GameManager.instance.reset();
        print("init");
    }
}
