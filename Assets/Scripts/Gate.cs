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
        if(player != null && player.paperCount > 5)
        {
            Open();
        }
    }

    public void Open()
    {
        animator.SetTrigger(Open_Hash);
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
