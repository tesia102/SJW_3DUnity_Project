using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OpenInfo : MonoBehaviour
{
    public TextMeshProUGUI gateOpen;
    public TextMeshProUGUI run;
    Player player;
    private void Awake()
    {
        player = FindObjectOfType<Player>();
        gateOpen = GetComponent<TextMeshProUGUI>();
        run = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        player.onGateOpen += OnGateOpen;
    }

    void OnGateOpen()
    {
        gateOpen.gameObject.SetActive(true);
        run.gameObject.SetActive(true);
    }
}
