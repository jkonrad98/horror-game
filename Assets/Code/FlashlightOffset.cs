using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightOffset : MonoBehaviour
{
    private Vector3 offsetVector;
    private Transform targetToFollow;
    [SerializeField] private float offsetSpeed = 4f;
    // Start is called before the first frame update
    void Start()
    {
        targetToFollow = Camera.main.transform;
        offsetVector = transform.position - targetToFollow.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = targetToFollow.position + offsetVector;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetToFollow.rotation, offsetSpeed * Time.deltaTime);

    }
}
