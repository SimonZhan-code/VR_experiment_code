using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Hand : MonoBehaviour
{
    public SteamVR_Action_Boolean m_Grabaction = null;



    private SteamVR_Behaviour_Pose m_Pose = null;
    private FixedJoint m_Joint = null;

    private Interactable m_CurrentInteractable = null;
    public List<Interactable> m_ContactInterables = new List<Interactable>();

    private void Awake()
    {
        m_Pose = GetComponent<SteamVR_Behaviour_Pose>();
        m_Joint = GetComponent<FixedJoint>();
    }

    // Update is called once per frame
    private void Update()
    {
        //Down
        if (m_Grabaction.GetStateDown(m_Pose.inputSource))
        {
            if (m_ContactInterables.Count != 0)
                print(m_Pose.inputSource + " Trigger Down");
                Pickup();
        }
        //Up
        if (m_Grabaction.GetStateUp(m_Pose.inputSource))
        {
            if (m_ContactInterables.Count == 0)
                print(m_Pose.inputSource + " Trigger Up");
                Drop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Interactable"))
            m_ContactInterables.Add(other.gameObject.GetComponent<Interactable>());

       
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Interactable"))
            m_ContactInterables.Remove(other.gameObject.GetComponent<Interactable>());

        
    }

    public void Pickup()
    {

    }

    public void Drop()
    {

    }

    private Interactable GetNearstInteractable()
    {
        return null;
        
    }
}
