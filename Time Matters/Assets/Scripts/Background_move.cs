using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background_move : MonoBehaviour
{

    public float _speed = 5.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(_speed * Time.delataTime, 0, 0);

        if(transform.localposition.x <-2500.0f)
        {
            transform.localPosition = new Vector3(-1280.0f, 0, 0);
        }
    }
}
