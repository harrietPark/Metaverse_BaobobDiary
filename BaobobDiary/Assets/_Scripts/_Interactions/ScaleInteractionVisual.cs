using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleInteractionVisual : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] GameObject leftHandSphere;
    [SerializeField] GameObject rightHandSphere;

    private bool isGestureActive = false;

    [SerializeField] float minScale;
    [SerializeField] float maxScale;
    [SerializeField] GameObject scaledObject;

    public void ShowLineRenderer()
    {
        lineRenderer.enabled = true;
        isGestureActive = true;
    }

    public void DisableLineRenderer()
    {
        lineRenderer.enabled = false;
        isGestureActive = false;
    }

    private void Update()
    {
        if (isGestureActive)
        {
            lineRenderer.SetPosition(0, leftHandSphere.transform.position);
            lineRenderer.SetPosition(1, rightHandSphere.transform.position);

            float distance = Vector3.Distance(leftHandSphere.transform.position, rightHandSphere.transform.position);
            float scaleFactor = Mathf.Clamp(distance, minScale, maxScale);

            scaledObject.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
        }
    }
}

