using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Enemy : MonoBehaviour
{
    public Transform target;

    NavMeshAgent nav;
    CapsuleCollider capsuleCollider;

    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        capsuleCollider = GetComponent<CapsuleCollider>();

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
            Debug.Log($"���� Ƚ�� : {player.hitted}");
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
        nav.speed = 2.5f;
    }
}
