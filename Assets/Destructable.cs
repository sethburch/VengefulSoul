using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour
{
    public AudioClip destroySound;
    
    public void Destruct()
    {
        GameObject holder = new GameObject();
        AudioSource audi = holder.AddComponent(typeof(AudioSource)) as AudioSource;

        audi.clip = destroySound;
        audi.pitch = UnityEngine.Random.Range(.90f, 1.1f);
        audi.volume = .5f;
        audi.Play();
        Destroy(holder, 2.0f);

        Destroy(gameObject);
    }
}
