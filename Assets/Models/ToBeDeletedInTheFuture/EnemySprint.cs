using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySprint : MonoBehaviour
{
    [SerializeField] private Animator anim;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            anim.SetTrigger("Run");
        }
    }
}
