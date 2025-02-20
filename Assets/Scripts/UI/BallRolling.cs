using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallRolling : MonoBehaviour
{
    [SerializeField] Rigidbody2D rigidbodyToFollow;

    void Update()
    {
        Vector3 axis = Vector3.Cross(rigidbodyToFollow.velocity.normalized, Vector3.forward);
        Debug.DrawRay(transform.position, axis);
        Debug.DrawRay(transform.position, transform.forward, Color.blue);
        float angle = rigidbodyToFollow.velocity.magnitude * Mathf.Rad2Deg * Time.deltaTime;
        transform.Rotate(axis, angle, Space.World);
    }
}
