using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowAndHideManual : MonoBehaviour
{
    public TextMeshProUGUI manual;

    private void Awake()
    {
        manual = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        StartCoroutine(HideManual());
    }

    IEnumerator HideManual()
    {
        yield return new WaitForSeconds(6.0f);
        this.gameObject.SetActive(false);
    }
}
