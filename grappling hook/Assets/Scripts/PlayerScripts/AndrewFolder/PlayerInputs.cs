using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//thieved shamelessly from https://www.youtube.com/watch?v=MbWK8bCAU2w&list=PLFt_AvWsXl0f0hqURlhyIoAabKPgRsqjz&index=1

[RequireComponent(typeof( MoveController))]
public class PlayerInputs : MonoBehaviour
{
    [SerializeField] private float gravity = -20f;
    Vector3 velocity;

    MoveController moveController;

    // Start is called before the first frame update
    void Start()
    {
        moveController = gameObject.GetComponent<MoveController>();
    }

    // Update is called once per frame
    void Update()
    {
        velocity.y += gravity * Time.deltaTime;
        moveController.Move(velocity * Time.deltaTime);
    }
}
