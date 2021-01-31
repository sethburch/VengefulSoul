using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    public float timeToBeatLevel = 10.0f;
    public RectTransform progressBar;
    public Player playerPrefab;

    public bool godMode = false;

    public GameObject levelCompleteText;

    private void LoadNextLevel()
    {
        if (!Application.CanStreamedLevelBeLoaded(SceneManager.GetActiveScene().buildIndex + 1))
        {
            Debug.LogError("CANT LOAD LEVEL");
            return;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void Start()
    {
        Manager.Instance.ResetLevelTime();
        if (!levelCompleteText) return;
        levelCompleteText.SetActive(false);
        LeanTween.scale(levelCompleteText, new Vector3(.6f, .6f), .2f).setEaseInOutSine().setLoopPingPong();

        // if (!godMode)
        //     LeanTween.scale(progressBar.gameObject, new Vector3(0.0f, transform.localScale.y), timeToBeatLevel).setOnComplete(FailedLevel);
    }

    private void FailedLevel()
    {
        RestartLevel();
    }

    private void Update()
    {
        if (GameObject.FindObjectsOfType<Enemy>().Length <= 0)
        {
            if (levelCompleteText)
                levelCompleteText.SetActive(true);
            if (progressBar)
                LeanTween.pause(progressBar.gameObject);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                LoadNextLevel();
            }
        }
        else
        {
            Manager.Instance.CountTime();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartLevel();
        }
    }

    public void RestartLevel()
    {
        foreach (Grenade grenade in GameObject.FindObjectsOfType<Grenade>())
        {
            Destroy(grenade.gameObject);
        }
        foreach (GameObject explosion in GameObject.FindGameObjectsWithTag("Explosion"))
        {
            Destroy(explosion);
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
