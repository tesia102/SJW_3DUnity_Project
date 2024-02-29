using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Gate : MonoBehaviour
{
    Animator animator;

    Player player;
    Enemy enemy;

    readonly int Open_Hash = Animator.StringToHash("Open");

    public Action onClear;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        player = FindObjectOfType<Player>();
        enemy = FindObjectOfType<Enemy>();
    }

    private void Update()
    {
        Open();
    }

    public void Open()
    {
        if (player != null && player.paperCount > 4)
        {
            animator.SetTrigger(Open_Hash);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            onClear?.Invoke();
            enemy.gameObject.SetActive(false);
        }
    }
}
