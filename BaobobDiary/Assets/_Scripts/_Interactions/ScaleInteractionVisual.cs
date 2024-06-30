using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScaleInteractionVisual : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] GameObject leftHandSphere;
    [SerializeField] GameObject rightHandSphere;

    [SerializeField] Texture _defaultIcon;
    [SerializeField] Texture _changedIcon;

    private bool isGestureActive = false;

    [SerializeField] float minScale;
    [SerializeField] float maxScale;
    [SerializeField] GameObject scaledObject;

    [SerializeField] float scaleMultiplier = 2.0f; // Multiplier to make it easier to reach max scale
    [SerializeField] float maxDistance = .35f; // Maximum distance threshold to show the line renderer

    private float maxScaleTimer = 0f;
    private const float maxScaleDuration = 1f;

    [HideInInspector] public UnityEvent maxScaledForThreeSecs;

    private void Start()
    {
        // Ensure the event is not null
        if (maxScaledForThreeSecs == null)
        {
            maxScaledForThreeSecs = new UnityEvent();
        }
    }

    public void ShowRectile()
    {
        leftHandSphere.SetActive(true);
        rightHandSphere.SetActive(true);
    }

    public void DisableRectile()
    {
        leftHandSphere.SetActive(false);
        rightHandSphere.SetActive(false);
    }

    public void ShowLineRenderer()
    {
        lineRenderer.enabled = true;
        isGestureActive = true;

        leftHandSphere.GetComponent<MeshRenderer>().material.mainTexture = _changedIcon;
        rightHandSphere.GetComponent<MeshRenderer>().material.mainTexture = _changedIcon;
    }

    public void DisableLineRenderer()
    {
        lineRenderer.enabled = false;
        isGestureActive = false;

        leftHandSphere.GetComponent<MeshRenderer>().material.mainTexture = _defaultIcon;
        rightHandSphere.GetComponent<MeshRenderer>().material.mainTexture = _defaultIcon;
    }

    private void Update()
    {
        if (isGestureActive)
        {
            float distance = Vector3.Distance(leftHandSphere.transform.position, rightHandSphere.transform.position);

            if (distance > maxDistance)
            {
                DisableLineRenderer();
                return;
            }

            Debug.Log(distance);

            lineRenderer.SetPosition(0, leftHandSphere.transform.position);
            lineRenderer.SetPosition(1, rightHandSphere.transform.position);

            float scaleFactor = Mathf.Clamp(distance * scaleMultiplier, minScale, maxScale); // Apply multiplier

            scaledObject.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

            if (Mathf.Approximately(scaleFactor, maxScale))
            {
                maxScaleTimer += Time.deltaTime;
                if (maxScaleTimer >= maxScaleDuration)
                {
                    maxScaledForThreeSecs.Invoke();
                    maxScaleTimer = 0f;
                }
            }
            else
            {
                maxScaleTimer = 0f;
            }
        }
    }
}
