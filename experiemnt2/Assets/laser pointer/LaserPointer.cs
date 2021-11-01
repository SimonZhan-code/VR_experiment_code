using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class LaserPointer : MonoBehaviour
{
    public SteamVR_Input_Sources handType;
    public SteamVR_Behaviour_Pose controllerPose;
    public SteamVR_Action_Boolean teleportAction;
    public SteamVR_Action_Boolean grabAction;
    public GameObject targetsphere;

    public GameObject laserPrefab;
    private GameObject laser;
    private Transform LaserTransform;
    private Vector3 hitPoint;

    void Start()
    {
        laser = Instantiate(laserPrefab);

        LaserTransform = laser.transform;
    }

    
    void Update()
    {
        if (teleportAction.GetState(handType))
        {
            RaycastHit hit;

            if (Physics.Raycast(controllerPose.transform.position, transform.forward, out hit, 100))
            {
                hitPoint = hit.point;
                ShowLaser(hit);
                if (GetGrab())
                {
                    Debug.Log(hit.collider.gameObject);
                }
            }
        }
        else
        {
            laser.SetActive(false);
        }
        
    }
    private void ShowLaser(RaycastHit hit)
    {
        laser.SetActive(true);
        LaserTransform.position = Vector3.Lerp(controllerPose.transform.position, hitPoint, .5f);
        LaserTransform.LookAt(hitPoint);
        
        LaserTransform.localScale = new Vector3(LaserTransform.localScale.x, LaserTransform.localScale.y, hit.distance);

    }
    public bool GetGrab()
    {
        return grabAction.GetState(handType);
    }
}
