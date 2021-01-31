using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartHint : MonoBehaviour
{
    public GameObject restart;

    private void Start()
    {
        LeanTween.scale(restart.gameObject, new Vector3(.6f, .6f), .2f).setEaseInOutSine().setLoopPingPong();
        restart.gameObject.SetActive(false);
    }

    void Update()
    {
        Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();
        if (enemies.Length <= 0)
        {
            restart.gameObject.SetActive(false);
            return;
        }
        foreach (var enemy in GameObject.FindObjectsOfType<Enemy>())
        {
            if (enemies.Length == 1)
            {
                if (!enemy.canKillSelf)
                {
                    restart.gameObject.SetActive(true);
                }
            }
        }
    }
}
