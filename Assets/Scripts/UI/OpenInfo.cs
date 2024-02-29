using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OpenInfo : MonoBehaviour
{
    public TextMeshProUGUI gateOpen;
    public TextMeshProUGUI run;

    private void Update()
    {
        FindObjectOfType<Player>().onGateOpen += OnGateOpen;
    }

    void OnGateOpen()
    {
        gateOpen.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        run.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
    }
}
