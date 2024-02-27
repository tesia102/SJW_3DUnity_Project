using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameClear : MonoBehaviour
{
    public Image fadePlane;
    public GameObject gameClearUI;

    private void Start()
    {
        StopAllCoroutines();
        FindObjectOfType<Gate>().onClear += OnGameClear;
    }

    void OnGameClear()
    {
        StartCoroutine(Fade(Color.clear, Color.black, 1.0f));
        gameClearUI.SetActive(true);
    }

    IEnumerator Fade(Color from, Color to, float time)
    {
        float speed = 1.0f / time;
        float percent = 0.0f;

        while(percent < 1.0f)
        {
            percent += Time.deltaTime * speed;
            fadePlane.color = Color.Lerp(from, to, percent);
            yield return null;
        }
    }

    //public void StartNewGame()
    //{
    //    SceneManager.LoadScene("02_Test_Map");
    //}
}
