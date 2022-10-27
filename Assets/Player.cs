using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    [SerializeField, Range(0.01f, 1f)] float moveDuration = 0.2f;
    [SerializeField, Range(0.01f, 1f)] float jumpHeight = 0.5f;

    private void Update() 
    {
        var moveDir = Vector3.zero;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            moveDir += new Vector3(0, 0, 1);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            moveDir += new Vector3(0, 0, -1);
        }    

        if (Input.GetKey(KeyCode.RightArrow))
        {
            moveDir += new Vector3(1, 0, 0);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveDir += new Vector3(-1, 0, 0);
        }
        
        if (moveDir != Vector3.zero && IsJumping() == false)
        {
            Jump(moveDir);
        }
    }

    private void Jump(Vector3 targetDirection)
    {
        var TargetPosition = transform.position + targetDirection;

        transform.LookAt(TargetPosition);

        var moveSeq = DOTween.Sequence(transform);
        moveSeq.Append(transform.DOMoveY(jumpHeight, moveDuration/2));
        moveSeq.Append(transform.DOMoveY(0, moveDuration/2));

        transform.DOMoveX(TargetPosition.x, moveDuration);
        transform.DOMoveZ(TargetPosition.z, moveDuration);
    }

    private bool IsJumping()
    {
        return DOTween.IsTweening(transform);
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Car")
        {
            AnimateDie();
        }
    }

    private void AnimateDie()
    {
        transform.DOScaleY(0.01f, 0.2f);
        transform.DOScaleX(3f, 0.2f);
        transform.DOScaleZ(2f, 0.2f);
        this.enabled = false;
    }
    
    private void OnTriggerStay(Collider other) {
        // execute setiap frame selama masih nempel
        Debug.Log("Stay");
    }

    private void OnTriggerExit(Collider other) {
        // execute sekali pada frame ketika tidak nempel
        Debug.Log("Exit");
    }
}
