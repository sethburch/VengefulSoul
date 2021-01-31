using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FinalScreen : MonoBehaviour
{
    public Text time;
    public GameObject parent;

    void Start()
    {
        LeanTween.move(gameObject, gameObject.transform.position + new Vector3(0.0f, .5f, 0.0f), .5f).setLoopPingPong().setEaseInOutSine();
        time.text = "Final Time: \n" + Manager.Instance.totalTime.ToString("f1");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            foreach (var audio in GameObject.FindObjectsOfType<AudioSource>())
            {
                Destroy(audio);
            }
            SceneManager.LoadScene(0);
        }
    }
}
