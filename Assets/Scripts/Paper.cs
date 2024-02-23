using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paper : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();

            player.paperCount++;
            Debug.Log($"{player.paperCount}");
            gameObject.SetActive(false);
        }
    }
}
