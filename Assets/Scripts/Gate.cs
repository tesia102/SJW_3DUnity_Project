using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    Animator animator;

    readonly int Open_Hash = Animator.StringToHash("Open");

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // https://zheldajdajd.tistory.com/5
}
