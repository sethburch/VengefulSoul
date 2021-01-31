using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : Bullet
{
    public Explosion explosionPrefab;

    bool isQuitting = false;

    void OnLevelWasLoaded(int level) { isQuitting = true; }

    private void OnDestroy()
    {
        if (isQuitting) return;
        Explosion explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        explosion.owner = owner;
    }
}
