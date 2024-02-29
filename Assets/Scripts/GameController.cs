using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public TextMeshProUGUI text;
    Player player;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    private void Start()
    {
        SetText();
    }

    public void SetText()
    {
        text.text = ($"Paper {player.paperCount} / 5\nLife : {3 - player.hitted}");
    }
}
