// Manager class handling ray casting actions
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    // Singleton Members
    private static InputManager _instance;
    public static InputManager Instance { get { return _instance; } }

    public GameObject station;

    private Camera MainCam;
    private Ray ray;
    private RaycastHit hit;

    [SerializeField] float shpereCastRadius = 0.05f;

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

        // Setup Raycast
        MainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        Input.multiTouchEnabled = false;
    }

    #region Casting Methods

    void Update()
    {
        // Listen for input on the editor or non-mobile standalone build
#if UNITY_ANDROID
       if (Application.isEditor)
       {
           if (Input.GetMouseButtonDown(0))
           {
               if (!EventSystem.current.IsPointerOverGameObject())
                   InitiateCast();
           }
           return;
       }
       else
       {
           if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
          {
               if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                   InitiateCast();
           }
       } 
#else
        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
                InitiateCast();
        }
#endif


    }

    private void InitiateCast()
    {
        Vector3 rayOrigin = MainCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
    
        if (Physics.SphereCast(rayOrigin, shpereCastRadius, MainCam.transform.forward, out hit, 100f))
        {
            //Debug.DrawRay(rayOrigin, MainCam.transform.forward, Color.green, 1f);

            if (hit.collider.gameObject.GetComponent<IClickable>() != null)
            {
                Debug.Log("Player Object Hit");
                hit.collider.gameObject.GetComponent<IClickable>().OnClick();
                return;
            }
            else
            {
                if (Physics.Raycast(rayOrigin, MainCam.transform.forward, out hit, 200))
                {
                    Debug.Log("Walkable Object Hit");
                    HandleWalkableHit(hit.point);
                }
            }
        }
    }

    #endregion

    #region Cast Hit Handler Methods

    private void HandleWalkableHit(Vector3 pos)
    {
        TargetLocation.Instance.gameObject.SetActive(false);

        MainManager.Instance.CurrSelectedPlayer.MoveToDestination(pos);
        TargetLocation.Instance.SetLocation(pos);

        TargetLocation.Instance.gameObject.SetActive(true);
    }

    #endregion
}
