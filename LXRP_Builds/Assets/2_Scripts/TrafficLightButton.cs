using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightButton : MonoBehaviour, IClickable
{
    TrafficLightScript mainScript;

    private void Awake()
    {
        mainScript = GetComponentInParent<TrafficLightScript>();
    }

    public void OnClick()
    {
        mainScript.OnClick();
    }

    public void SendMessageToManager()
    {
        //throw new System.NotImplementedException();
    }

    void Update()
    {
        transform.LookAt(GameObject.FindGameObjectWithTag("MainCamera").transform);
        Mathf.Clamp(transform.rotation.x, 0, 0);
        Mathf.Clamp(transform.rotation.z, 0, 0);
    }
}
