using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class Player : MonoBehaviour
{
    [SerializeField] TMP_Text stepText;
    [SerializeField] ParticleSystem dieParticles;
    [SerializeField, Range(0.01f, 1f)] float moveDuration = 0.2f;
    [SerializeField, Range(0.01f, 1f)] float jumpHeight = 0.5f;
    private int minZPos;
    private int extent;
    private float backBoundary;
    private float leftBoundary;
    private float rightBoundary;
    
    private int maxTravel;
    public int MaxTravel { get => maxTravel; }
    private int currentTravel;
    public int CurrentTravel { get => currentTravel; }

    public bool IsDie { get => this.enabled == false;}

    public void SetUp(int minZPos, int extent)
    {
        backBoundary = minZPos - 1;
        leftBoundary = -(extent + 1);
        rightBoundary = extent + 1;
    }

    private void Update() 
    {
        var moveDir = Vector3.zero;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            moveDir += new Vector3(0, 0, 1);
        }

        else if (Input.GetKey(KeyCode.DownArrow))
        {
            moveDir += new Vector3(0, 0, -1);
        }    

        else if (Input.GetKey(KeyCode.RightArrow))
        {
            moveDir += new Vector3(1, 0, 0);
        }

        else if (Input.GetKey(KeyCode.LeftArrow))
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
        Vector3 TargetPosition = transform.position + targetDirection;
        transform.LookAt(TargetPosition);

        var moveSeq = DOTween.Sequence(transform);
        moveSeq.Append(transform.DOMoveY(jumpHeight, moveDuration/2));
        moveSeq.Append(transform.DOMoveY(0, moveDuration/2));

        // bugged, jadi ga bisa jalan
        if  (TargetPosition.z <= backBoundary || TargetPosition.x <= leftBoundary || TargetPosition.x >= rightBoundary)
            return;

        if(Tree.AllPositions.Contains(TargetPosition))
            return;

        transform.DOMoveX(TargetPosition.x, moveDuration);
        transform.DOMoveZ(TargetPosition.z, moveDuration).OnComplete(UpdateTravel);
    }

    private void UpdateTravel()
    {
        currentTravel = (int) this.transform.position.z;
        if(currentTravel > maxTravel)
        maxTravel = currentTravel;

        stepText.text = "STEP: " + maxTravel.ToString();
    }

    public bool IsJumping()
    {
        return DOTween.IsTweening(transform);
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(this.enabled==false)
            return;
            
        var car = other.GetComponent<Car>();
        if (car != null)
        {
            AnimateCrash();
        }

        // if(other.tag == "Car")
        // {
        //     //AnimateCrash();
        // }
    }

    private void AnimateCrash()
    {
        transform.DOScaleY(0.01f, 0.2f);
        transform.DOScaleX(3f, 0.2f);
        transform.DOScaleZ(2f, 0.2f);
        this.enabled = false;
        dieParticles.Play();
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
