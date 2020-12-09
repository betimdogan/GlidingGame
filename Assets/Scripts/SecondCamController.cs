using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondCamController : MonoBehaviour
{
    [SerializeField]
    Transform rocketmanPosition;
    Vector3 distance;
    float speed = 20.0f;
/*
    void Start()
    {
        rocketmanPosition = GameObject.Find("Rocketman_Animated").transform;  
    }
*/

    // Update is called once per frame
    void LateUpdate()
    {
        distance = new Vector3(rocketmanPosition.position.x, rocketmanPosition.position.y + 0.99f, rocketmanPosition.position.z - 1.2f);
        transform.position = Vector3.Lerp(transform.position, distance, speed*Time.deltaTime);

        
    }
}
