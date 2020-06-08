using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARManager : MonoBehaviour
{
    private static ARManager _instance;
    public static ARManager Instance { get { return _instance; } }

    [SerializeField] ARSession arSession;
    [SerializeField] ARSessionOrigin arOrigin;
    [SerializeField] ARCameraManager aRCamera;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            //DontDestroyOnLoad(this);
        }
    }

    //private void OnLevelWasLoaded(int level)
    //{
        //arSession.Reset();
        //arSession.subsystem.Reset();
        //aRCamera.subsystem.Start();
        //arOrigin.camera.Reset();
    //}


}
