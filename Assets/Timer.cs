using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    void Update()
    {
        GetComponent<Text>().text = "Time: " + Manager.Instance.thisLevelTime.ToString("f1");
    }
}
