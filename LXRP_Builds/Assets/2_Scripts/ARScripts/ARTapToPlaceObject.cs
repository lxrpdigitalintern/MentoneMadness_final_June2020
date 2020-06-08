using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System;
using UnityEngine.UI;

public class ARTapToPlaceObject : MonoBehaviour
{
    [SerializeField] GameObject objectToPlace = null;
    [SerializeField] GameObject placementIndicator = null;

    private ARSessionOrigin arOrigin;
    private ARRaycastManager arRaycast;
    private ARSession arSession;
    private Camera mainCam;

    private Pose placementPose;
    private bool placementPoseIsValid = false;
    private bool isObjectPlaced = false;
    private EARState CURRSTATE = EARState.BLANK;

    [Space]
    /// The time to delay, after ARCore loses tracking of any planes, showing the plane
    /// discovery guide.
    [Tooltip("The time to delay, after ARCore loses tracking of any planes, showing the plane " +
                "discovery guide.")]
    public float DisplayGuideDelay = 3.0f;

    /// The time to delay, after displaying the plane discovery guide, offering more detailed
    /// instructions on how to find a plane.
    [Tooltip("The time to delay, after displaying the plane discovery guide, offering more detailed " +
                "instructions on how to find a plane.")]
    public float OfferDetailedInstructionsDelay = 8.0f;

    /// The time to delay, after Unity Start, showing the plane discovery guide.
    private const float k_OnStartDelay = 1f;

    /// The time to delay, after a at least one plane is tracked by ARCore, hiding the plane discovery guide.
    private const float k_HideGuideDelay = 0.75f;

    /// The duration of the hand animation fades.
    private const float k_AnimationFadeDuration = 0.15f;

    /// The duration of the hand animation fades.
    private const float k_AnimationHideDelay = 3.15f;

    [Space]
    /// The RawImage that provides rotating hand animation.
    [Tooltip("The RawImage that provides rotating hand animation.")]
    [SerializeField] private RawImage m_HandAnimation = null;

    /// The snackbar Game Object.
    [Tooltip("The snackbar Game Object.")]
    [SerializeField] private GameObject m_SnackBar = null;

    /// The snackbar text.
    [Tooltip("The snackbar text.")]
    [SerializeField] private Text m_SnackBarText = null;

    /// The Game Object that contains the button to open the help window.
    [Tooltip("The Game Object that contains the button to open the help window.")]
    [SerializeField] private GameObject m_OpenButton = null;

    /// The Game Object that contains the window with more instructions on how to find a plane.
    [Tooltip(
        "The Game Object that contains the window with more instructions on how to find " +
        "a plane.")]
    [SerializeField] private GameObject m_MoreHelpWindow = null;

    /// The Game Object that contains the button to close the help window.
    [Tooltip("The Game Object that contains the button to close the help window.")]
    [SerializeField] private Button m_GotItButton = null;

    [Space]
    [Space]
    /// The elapsed time ARCore has been detecting at least one plane.
    [SerializeField] private float m_DetectedPlaneElapsed = 0;

    /// The elapsed time ARCore has been tracking but not detected any planes.
    [SerializeField] private float m_NotDetectedPlaneElapsed = 0.0f;

    private void Awake()
    {
        mainCam = GameObject.FindObjectOfType<Camera>();

        arRaycast = GetComponent<ARRaycastManager>();
        arOrigin = GetComponent<ARSessionOrigin>();
        arSession = FindObjectOfType<ARSession>();

        m_OpenButton.GetComponent<Button>().onClick.AddListener(_OnOpenButtonClicked);
        m_GotItButton.onClick.AddListener(_OnGotItButtonClicked);
    }
    void Start()
    {
        //arOrigin = FindObjectOfType<ARSessionOrigin>();
        //ARraycast = FindObjectOfType<ARRaycastManager>();
        objectToPlace.SetActive(false);
    }

    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            PlaceObject();
        }
    }

    private void PlaceObject()
    {
        Instantiate(objectToPlace, placementPose.position, placementPose.rotation);
        placementIndicator.SetActive(false);
        SetState(EARState.PLACED);
    }

    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void UpdatePlacementPose()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        arRaycast.Raycast(screenCenter, hits, TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;

            var cameraForward = Camera.current.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }
    public void SetState(EARState newState)
    {
        if (CURRSTATE == newState)
        {
            return;
        }

        Debug.Log("ARSTATE change from:  " + CURRSTATE + "  to:  " + newState);
        CURRSTATE = newState;
        HandleStateChangedEvent(CURRSTATE);
    }

    void HandleStateChangedEvent(EARState state)
    {
        if (CURRSTATE != state)
            return;

        // Show tutorial
        if (state == EARState.TUTORIAL)
        {
            placementIndicator.SetActive(false);
            return;
        }

        // Starting state of the game
        if (state == EARState.PLACEMENT)
        {
            return;
        }

        if (state == EARState.PLACED)
        {
            isObjectPlaced = true;

            //m_HandAnimation.enabled = false;
            //m_SnackBar.SetActive(false);
            //m_OpenButton.SetActive(false);

            MainManager.Instance.SetState(EGameState.PLACED);
            return;
        }
    }

    private void _OnGotItButtonClicked()
    {
        m_MoreHelpWindow.SetActive(false);
        enabled = true;
    }
    private void _OnOpenButtonClicked()
    {
        m_MoreHelpWindow.SetActive(true);

        enabled = false;
        //-------------------------------------------------------------------------
        //m_FeaturePoints.SetActive(false);
        m_HandAnimation.enabled = false;
        m_SnackBar.SetActive(false);
    }
}