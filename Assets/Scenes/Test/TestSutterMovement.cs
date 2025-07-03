using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSutterMovement : MonoBehaviour{

    [SerializeField] Transform head;
    [SerializeField] Transform hand;
    [SerializeField] Rigidbody handModel;
    [SerializeField] CharacterController character;

    [Space]

    [SerializeField] float mouseSpeed = 10f;
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float mouseAngleX = 70f;
    [SerializeField] float handDistance = 0.4f;
    [SerializeField] bool useStaticMouseMove = false;
    [SerializeField] bool onBeforeRender = true;
    [SerializeField] bool updateHandOnBeforeRender = true;

    Vector3 headRotation;
    Vector3 headPositionDirection;
    Vector3 handDirection;

    void Start() {
        headRotation = head.rotation.eulerAngles;
        Application.onBeforeRender += OnBeforeRender;
    }

    void Update() {
        Inputs();
        MovementHead(Time.deltaTime);
        MovementHand();
        // MovementHandModel();
    }

    void FixedUpdate() {
        //MovementHead(Time.fixedDeltaTime);
        //
        MovementCharacter(Time.fixedDeltaTime);
        MovementHandModel(Time.fixedDeltaTime);
    }

    void LateUpdate() {
        //MovementHead(Time.deltaTime);
        //MovementHand();
    }

    void OnBeforeRender() {
        if (!onBeforeRender) return;
        Inputs();
        MovementHead(Time.deltaTime);
        if(updateHandOnBeforeRender) MovementHand();
    }

    void Inputs() {
        headRotation.x -= Input.GetAxis("Mouse Y") * mouseSpeed * Time.deltaTime;
        headRotation.y += (Input.GetAxis("Mouse X") + (useStaticMouseMove ? 5f : 0f)) * mouseSpeed * Time.deltaTime;
        headRotation.x = Mathf.Clamp(headRotation.x, -mouseAngleX, mouseAngleX);
        headRotation.z = head.rotation.eulerAngles.z;
        headPositionDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        
    }

    void MovementHead(float deltaTime) {
        head.rotation = Quaternion.Euler(headRotation);
        Vector3 dir = head.rotation * headPositionDirection;
        dir.y = 0f;
        head.position += dir * deltaTime * moveSpeed * (onBeforeRender ? 0.5f : 1f);
    }

    void MovementHand() {
        Vector3 dir = head.rotation * Vector3.forward;
        dir = dir.normalized * handDistance;
        hand.position = head.position + dir;
        handDirection = (hand.position - handModel.position);
    }

    void MovementCharacter(float deltaTime) {
        Vector3 dir = head.position - character.transform.position;
        dir.y = 0;
        character.Move(dir * 1f/deltaTime);
    }

    void MovementHandModel(float deltaTime) {
        //handModel.transform.position = handModel.position = hand.position;
        // handModel.velocity = (hand.position - handModel.position) * (1f / deltaTime);
        //handModel.velocity = handDirection * (1f / deltaTime);
        Vector3 targetVelocity = (hand.position - handModel.position) * (1f / deltaTime);
        targetVelocity -= handModel.linearVelocity;
        handModel.AddForce(targetVelocity * handModel.mass, ForceMode.Impulse);
    }


}
