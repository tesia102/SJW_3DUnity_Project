using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    Animator animator;

    readonly int Open_Hash = Animator.StringToHash("Open");

    Player player;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        Player player = GetComponent<Player>();
    }

    private void Update()
    {
        player.onGateOpen = Open;
    }

    private void Open()
    {
        animator.SetTrigger(Open_Hash);
    }
}
