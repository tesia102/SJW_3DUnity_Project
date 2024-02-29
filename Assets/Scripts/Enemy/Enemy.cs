using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Enemy : MonoBehaviour
{
    public Transform target;

    NavMeshAgent nav;
    CapsuleCollider capsuleCollider;
    GameController gameController;
    Player player1;

    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        gameController = FindObjectOfType<GameController>();
        player1 = FindObjectOfType<Player>();
    }

    private void Update()
    {
        nav.SetDestination(target.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            
            Player player = other.GetComponent<Player>();

            player.hitted++;
            gameController.SetText();
            //Debug.Log($"���� Ƚ�� : {player.hitted}");
            capsuleCollider.isTrigger = false;
            nav.speed = 0.0f;
            StartCoroutine(HitAndStop());

            if (player.hitted > 2) 
            { 
                player.Die();
                Debug.Log("�÷��̾� �׾���");
                gameObject.SetActive(false);
            }
        }
    }

    IEnumerator HitAndStop()
    {
        yield return new WaitForSeconds(10.0f);
        capsuleCollider.isTrigger = true;
        nav.speed = 2.5f + (0.3f * player1.paperCount);
    }
}
