using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour
{
    public Button fadeButton;

    public void Click()
    {
        StartCoroutine(Fade(Color.black, Color.clear, 1.0f));
    }

    IEnumerator Fade(Color from, Color to, float time)
    {
        float speed = 0.2f / time;
        float percent = 0.0f;

        while (percent < 1.0f)
        {
            percent += Time.deltaTime * speed;
            fadeButton.image.color = Color.Lerp(from, to, percent);
            yield return null;
        }
        StartGame();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Main");
    }
}
