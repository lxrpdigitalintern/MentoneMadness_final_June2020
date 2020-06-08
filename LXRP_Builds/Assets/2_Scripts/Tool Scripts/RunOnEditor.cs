using UnityEngine;
using System.Collections;

public class RunOnEditor : MonoBehaviour
{
    [SerializeField] Camera ARCamera = null;

    private void Start()
    {

    }
    private void Awake()
    {
#if UNITY_ANDROID
        // Destroy AR camera object if running in editor or as non-mobile stand alone build
        if (Application.isEditor || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            Destroy(ARCamera.gameObject);
        }
        else // Destroy "regular" camera if running a mobile device build
        {
            Destroy(this.gameObject);
        }
#else
        Destroy(ARCamera.gameObject);
#endif
    }
}
