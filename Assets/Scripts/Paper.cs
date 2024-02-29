using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paper : MonoBehaviour
{
    GameController gameController;
    Enemy enemy;

    private void Awake()
    {
        gameController = FindObjectOfType<GameController>();
        enemy = FindObjectOfType<Enemy>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();

            player.paperCount++;
            enemy.nav.speed = 2.5f + (0.3f * player.paperCount);
            gameController.SetText();
            gameObject.SetActive(false);
        }
    }
}
