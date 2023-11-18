using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DetectionSystem : MonoBehaviour
{
    [Header("Game Object to Detect")]
    [SerializeField] private GameObject _objectToDetect;
    [SerializeField][Tooltip("The object is not detected by default on game start.")]
    public bool objectDetected = false;

    [Header("Line of Sight")]
    [SerializeField][Tooltip("Check if line of sight is required for detection.")]
    bool _lineOfSightRequired;
    [SerializeField][Tooltip("Line of Sight point of origin.")]
    GameObject _raycastOrigin;

    [Header("Method of Detection")]
    [SerializeField] private DetectionMethod _detectionMethod;
    [SerializeField] string _tagToDetect;
    [SerializeField] LayerMask _layerToDetect;
    private enum DetectionMethod
    {
        Tag,
        Layer,
        GameObject
    }

    [Header("Time Until Detector Forgets")]
    [SerializeField][Tooltip("The amount of time in seconds it takes for the detector to forget the object after it leaves the detection range.")] 
    public float DetectLeaveDuration = 2.0f;
    private IEnumerator coroutine;

    [Header("Events to be Run")]
    public UnityEvent DetectedTagWithoutLineOfSight;
    public UnityEvent DetectedTagWithLineOfSight;
    public UnityEvent DetectedLayerWithoutLineOfSight;
    public UnityEvent DetectedLayerWithLineOfSight;
    public UnityEvent DetectedGameObjectWithoutLineOfSight;
    public UnityEvent DetectedGameObjectWithLineOfSight;

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (_detectionMethod)
        {

            case DetectionMethod.Tag:
                if (other.tag == _tagToDetect)
                {
                    if (_lineOfSightRequired == false)
                    {
                        Debug.Log("Detected Player Tag, LoS not needed.");
                        objectDetected = true;

                        DetectedTagWithoutLineOfSight.Invoke();

                        ObjectLeavingRange();
                        
                    }

                    if (_lineOfSightRequired == true)
                    {
                        Debug.Log("Tag, LoS needed.");
                        //LineOfSight();
                        objectDetected = true;

                        DetectedTagWithLineOfSight.Invoke();

                        ObjectLeavingRange();
                    }
                }
                break;

            case DetectionMethod.Layer:
                if (other.gameObject.layer == _layerToDetect)
                {
                    if (_lineOfSightRequired == false)
                    {
                        Debug.Log("Detected Player Layer, LoS not needed.");
                        objectDetected = true;
                        ObjectLeavingRange();
                    }

                    if (_lineOfSightRequired == true)
                    {
                        Debug.Log("Layer, LoS needed.");
                        objectDetected = true;
                        ObjectLeavingRange();
                    }
                }
                break;

            case DetectionMethod.GameObject:
                if (other.gameObject == _objectToDetect)
                {
                    if (_lineOfSightRequired == false)
                    {
                        Debug.Log("Detected Player GameObject, LoS not needed.");
                        objectDetected = true;
                        ObjectLeavingRange();
                    }

                    if (_lineOfSightRequired == true)
                    {
                        Debug.Log("GameObject, LoS needed.");
                        objectDetected = true;
                        ObjectLeavingRange();
                    }
                }
                break;

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            //Debug.Log("Player Exited Collider.");
            StartCoroutine(coroutine);
        }
    }

    private void ObjectLeavingRange()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = leftRange();
    }

    IEnumerator leftRange()
    {
        yield return new WaitForSeconds(DetectLeaveDuration);
        objectDetected = false;
        Debug.Log("Player Not Detected");
    }

    public void LineOfSight()
    {

        Debug.Log("LineOfSight invoked.");
        
        Vector3 rayStartPos = _raycastOrigin.transform.position;
        //Vector3 rayDirection = gameObject.transform.forward;

        if (objectDetected)
        {
            Debug.Log("LineOfSight objectDetected.");

            Vector3 rayDirection = _objectToDetect.transform.position - _raycastOrigin.transform.position;
            Ray ray = new Ray(rayStartPos, rayDirection);
            Debug.DrawRay(ray.origin, ray.direction, Color.red);
            RaycastHit hitData;
            Physics.Raycast(ray, out hitData);

            if (hitData.transform != null)
            {
                Debug.Log("Detected Object: " + hitData.transform.gameObject.name);


                if (hitData.transform.gameObject.tag == "Player")
                {
                    //Debug.Log("Detected Object: " + hitData.transform.gameObject.name);
                    gameObject.transform.LookAt(_objectToDetect.transform.position);
                }
            }
        }
    }
}