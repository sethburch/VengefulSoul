using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    private static Manager _instance;
    public static Manager Instance { get { return _instance; } }

    public float totalTime = 0.0f;
    public float thisLevelTime = 0.0f;

    public AudioClip musicIntro;
    public AudioClip musicLoop;

    public AudioSource audi;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void CountTime()
    {
        totalTime += Time.deltaTime;
        thisLevelTime += Time.deltaTime;
    }

    public void ResetLevelTime()
    {
        thisLevelTime = 0.0f;
    }

    public void StartMusic()
    {
        GameObject holder = new GameObject();
        holder.transform.parent = transform;
        audi = holder.AddComponent(typeof(AudioSource)) as AudioSource;

        audi.clip = musicIntro;
        audi.volume = 2.0f;
        audi.Play();
    }

    private void Update()
    {
        if (!audi) return;

        if (!audi.isPlaying)
        {
            audi.clip = musicLoop;
            audi.Play();
        }
    }
}
