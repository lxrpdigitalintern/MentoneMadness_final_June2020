using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Singleton class
public class TargetLocation : MonoBehaviour
{
    private static TargetLocation _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public static TargetLocation Instance { get { return _instance; } }

    public void SetLocation(Vector3 pos)
    {
        transform.position = pos;
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

}
