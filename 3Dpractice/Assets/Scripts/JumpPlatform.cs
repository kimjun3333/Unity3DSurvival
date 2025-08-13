using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPlatform : MonoBehaviour
{
    public int jumpForce;

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rigid = other.GetComponent<Rigidbody>();

        if(rigid != null)
        {
            rigid.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
