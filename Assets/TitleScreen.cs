using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    public GameObject titleText;

    // Start is called before the first frame update
    void Start()
    {
        LeanTween.move(titleText, titleText.transform.position + new Vector3(0.0f, .5f, 0.0f), .5f).setLoopPingPong().setEaseInOutSine();
    }

    // Update is called once per frame
    void Update()
    {
        if (!Application.CanStreamedLevelBeLoaded(SceneManager.GetActiveScene().buildIndex + 1)) return;

        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            Manager.Instance.StartMusic();
        }
    }
}
