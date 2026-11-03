using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class ARPlacementManager : MonoBehaviour
{
    [Header("AR Components")]
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private GameObject placementIndicator;
    [Header("Cell Model")]
    [SerializeField] private GameObject cellPrefab;
    [Header("Cell Manipulation")]
    [SerializeField] private float rotationSpeed = 0.5f;
    [SerializeField] private float scaleSpeed = 0.005f;
    [SerializeField] private float minScale = 0.05f;
    [SerializeField] private float maxScale = 2f;

    private float initialDistance;
    private Vector3 initialScale;

    private GameObject placedCell;

    private List<ARRaycastHit> hits =
        new List<ARRaycastHit>();

    private Pose placementPose;
    private bool placementPoseIsValid = false;

    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();
        PlaceCell();

        ScaleCell();
        RotateCell();
    }

    void UpdatePlacementPose()
    {
        Vector2 screenCenter =
            new Vector2(Screen.width / 2f, Screen.height / 2f);

        placementPoseIsValid = raycastManager.Raycast(
            screenCenter,
            hits,
            TrackableType.PlaneWithinPolygon
        );

        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;
        }
    }

    void UpdatePlacementIndicator()
    {
        if (placementIndicator == null)
            return;

        placementIndicator.SetActive(placementPoseIsValid);

        if (placementPoseIsValid)
        {
            placementIndicator.transform.SetPositionAndRotation(
                placementPose.position,
                placementPose.rotation
            );
        }
    }

    void PlaceCell()
    {
        if (cellPrefab == null)
            return;
        
        if (!placementPoseIsValid)
            return;

        if (Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase != TouchPhase.Began)
            return;

        if (placedCell == null)
        {
            placedCell = Instantiate(
                cellPrefab,
                placementPose.position,
                placementPose.rotation
            );
        }
        else
        {
            placedCell.transform.position = placementPose.position;
        }
    }

    void ScaleCell()
    {
        if (placedCell == null || Input.touchCount != 2)
            return;

        Touch touch0 = Input.GetTouch(0);
        Touch touch1 = Input.GetTouch(1);

        if (touch0.phase == TouchPhase.Began ||
            touch1.phase == TouchPhase.Began)
        {
            initialDistance = Vector2.Distance(
                touch0.position,
                touch1.position
            );

            initialScale = placedCell.transform.localScale;
        }

        float currentDistance = Vector2.Distance(
            touch0.position,
            touch1.position
        );

        if (initialDistance <= 0)
            return;

        float scaleFactor = currentDistance / initialDistance;

        Vector3 newScale = initialScale * scaleFactor;

        float clampedScale = Mathf.Clamp(
            newScale.x,
            minScale,
            maxScale
        );

        placedCell.transform.localScale = Vector3.one * clampedScale;
    }

    void RotateCell()
    {
        if (placedCell == null || Input.touchCount != 2)
            return;

        Touch touch0 = Input.GetTouch(0);
        Touch touch1 = Input.GetTouch(1);

        if (touch0.phase == TouchPhase.Moved ||
            touch1.phase == TouchPhase.Moved)
        {
            float rotation = touch0.deltaPosition.x * rotationSpeed;

            placedCell.transform.Rotate(
                Vector3.up,
                -rotation,
                Space.World
            );
        }
    }
}
