using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.SceneManagement;

public class TargetsCreating : MonoBehaviour {

    public GameObject TargetPrefab;

    private Vector3 roomLower = new Vector3(-3.3f, 0, 3);
    private Vector3 roomUpper = new Vector3(3.3f, 4, 7);


    float roomEdge = 0.2f;
    

    public static List<GameObject> alltargets = new List<GameObject>();
    public static int targetsNum = 150;
    //private float InitialVelocity = 6f;
    private int targetID = -1;

    private Color TARGET_COLOR = Color.red;
    private Color DEFAULT_COLOR = Color.white;

    public static bool isStartingTriggered = false;
    public static bool isSelectionTriggered = false;
    public static bool isTargetsCreating = false;

    public static bool isCreatingNewTrail = true;
    public static List<int> TrialPool = new List<int>();
    public static int[] density = { 10, 100};
    public static float[] velocity = { 2, 4, 6 };

    public static int current_trial;
    public static int current_density_id;
    public static int current_velocity_id;

    public int current_density;
    public float current_velocity;

    public static int needClickNum = 2;


    public static int pilotSelectionNum = 10;
    public static int[] pilot_density = { 10, 50, 100 };
    public static int[] pilot_velocity = { 4 };

    // Use this for initialization
    void Start () {
        if(isCreatingNewTrail == true)
        {
            InitTrail();
            isCreatingNewTrail = false;
        }
        
	}
    
 	
	// Update is called once per frame
	void Update () {
        if (isStartingTriggered)
        {
            DestroyTargets();
            //SelectionInput.perMissNumber = 0;
            //SelectionInput.perWrongNumber = 0;
            CreatTargets();
            isTargetsCreating = true;
            isStartingTriggered = false;

        }
        if (isSelectionTriggered) // 是否选中
        {
            DestroyTargets();
            isTargetsCreating = false;
            isSelectionTriggered = false;
            StartControl.isStartBtnCreating = false;
            //startBtn.SendMessage("CreatStartBtn");
            //SteamVR_LoadLevel.Begin("StartMenu");
        }
        if (isTargetsCreating) // 是否创建了目标
        {
            for (int i = 0; i < alltargets.Count; i++)
            {
                RegionDetection(alltargets[i]);                  
            }            
        }
	}

    private void Bounce(GameObject targetObject, Vector3 collisionNormal)
    {
        //print("collisionNormal = " + collisionNormal);

        Vector3 lastFrameVelocity = targetObject.GetComponent<Rigidbody>().velocity;
        //Debug.Log("position = " + targetObject.transform.position);
        //Debug.Log("lastFrameVelocity = " + lastFrameVelocity);

        var speed = lastFrameVelocity.magnitude;
        var direction = Vector3.Reflect(lastFrameVelocity.normalized, collisionNormal);

        //Debug.Log("Out Direction: " + direction);
        targetObject.GetComponent<Rigidbody>().velocity = direction * Mathf.Max(speed, current_velocity);
        
        //Debug.Log("newFrameVelocity = " + targetObject.GetComponent<Rigidbody>().velocity);
    }

    public void ChangeColor(GameObject g, Color color)
    {
        g.GetComponent<Renderer>().material.SetColor("_OutlineColor", color);
        if (color!= TARGET_COLOR && TrajectoriesAnalysis.tech == TrajectoriesAnalysis.Techniche.PilotStudy)
        {
            color.a = 0;
            //Debug.Log("pilot color = " + color);
        }
        else
        {
            color.a = 1;
        }
        g.GetComponent<Renderer>().material.color = color;
    }


    public void CreatTargets()
    {
        current_trial = getTrial();

        current_density = getDensity(current_trial);
        current_velocity = getVelocity(current_trial);
        targetsNum = current_density;
        targetID = UnityEngine.Random.Range(0, targetsNum);
        //print("Start Creating targets" + targetID);
        for (int i = 0; i < targetsNum; i++)
        {
           
            else // 左下角-->右上角
                {
                    positionX = UnityEngine.Random.Range((roomUpper.x - roomLower.x) / 4, (roomUpper.x - roomLower.x) / 2 - roomEdge);
                    positionY = UnityEngine.Random.Range((roomUpper.y - roomLower.y) * 3 / 4, roomUpper.y - roomEdge);
                    //positionX = UnityEngine.Random.Range(0, (roomUpper.x - roomLower.x) / 4);
                    //positionY = UnityEngine.Random.Range((roomUpper.y - roomLower.y) / 2, (roomUpper.y - roomLower.y) * 3 / 4);
                    positionZ = UnityEngine.Random.Range(roomLower.z + roomEdge, roomUpper.z - roomEdge);
                }
                targetClone = Instantiate(TargetPrefab, new Vector3(positionX, positionY, positionZ), Quaternion.identity);
                
            }
            else // Creat Non_Targets
            {
                do
                {
                    positionX = UnityEngine.Random.Range(roomLower.x + roomEdge, roomUpper.x - roomEdge);
                    positionY = UnityEngine.Random.Range(roomLower.y + roomEdge, roomUpper.y - roomEdge);
                    positionZ = UnityEngine.Random.Range(roomLower.z + roomEdge, roomUpper.z - roomEdge);
                } while (!InRegion(new Vector3(positionX, positionY, positionZ)));
                targetClone = Instantiate(TargetPrefab, new Vector3(positionX, positionY, positionZ), Quaternion.identity);
            }

            // Geralize initial velocity randomly
            //float velocity = InitialVelocity;
            float velocity = current_velocity;
            float dir_theta = UnityEngine.Random.Range(0, Mathf.PI);
            float dir_alpha = UnityEngine.Random.Range(0, 2 * Mathf.PI);

            targetClone.GetComponent<Rigidbody>().velocity = new Vector3(velocity * Mathf.Sin(dir_theta) * Mathf.Cos(dir_alpha), velocity * Mathf.Cos(dir_theta), velocity * Mathf.Sin(dir_theta) * Mathf.Sin(dir_alpha));
            targetClone.GetComponent<ElasticCollision>().SetVelocity(targetClone.GetComponent<Rigidbody>().velocity);

            if(i == targetID)
            {
                
                ChangeColor(targetClone, TARGET_COLOR);
                targetClone.tag = "target";

                if(TrajectoriesAnalysis.tech == TrajectoriesAnalysis.Techniche.Ray_Casting)
                {
                    targetClone.AddComponent<SelectionEventHandler>();
                }
                //Debug.Log("target color = " + GetComponent<Renderer>().material.color);
                //Debug.Log("target position = " + targetClone.transform.position);
                //Debug.Log("target speed = " + targetClone.GetComponent<Rigidbody>().velocity);
            }
            else
            {
                targetClone.tag = "non_target";
                ChangeColor(targetClone, DEFAULT_COLOR);
            }
            
            alltargets.Add(targetClone);
        }
        isTargetsCreating = true;
        //Debug.Log("Finish Creating");
    }



    public void DestroyTargets()
    {
        //Debug.Log("Destroy target count" + alltargets.Count);
        for (int i = 0; i < alltargets.Count; i++)
        {
            Destroy(alltargets[i]);
        }
        alltargets.Clear();
        isTargetsCreating = false;
    }

    int getTrial()
    {
        int Index = UnityEngine.Random.Range(0, TrialPool.Count);
        int trialId = TrialPool[Index];
        TrialPool.RemoveAt(Index);
        //Debug.Log("trialid = " + trialId);
        return trialId;
    }

    int getDensity(int trialID)
    {
        current_density_id = trialID % 10;
        if (TrajectoriesAnalysis.tech == TrajectoriesAnalysis.Techniche.PilotStudy)
        {
            return pilot_density[current_density_id];
        }
        else
        {
            return density[current_density_id];
        }
    }


    float getVelocity(int trilaID)
    {
        current_velocity_id = trilaID / 10;
        if (TrajectoriesAnalysis.tech == TrajectoriesAnalysis.Techniche.PilotStudy)
        {
            return pilot_velocity[current_velocity_id];
        }
        else
        {
            return velocity[current_velocity_id];
        }
    }

    void InitTrail()
    {
        TrialPool.Clear();
        if(TrajectoriesAnalysis.tech == TrajectoriesAnalysis.Techniche.PilotStudy)
        {
            for(int i = 0; i < pilot_density.Length; i++)
            {
                for(int j = 0; j < pilot_velocity.Length; j++)
                {
                    for(int t = 0; t < pilotSelectionNum; t++)
                    {
                        TrialPool.Add(i + j * 10);
                    }
                }
            }
            return;
        }

        for(int i = 0; i < density.Length; i++)
        {
            for(int j = 0; j < velocity.Length; j++)
            {
                for(int t = 0; t < needClickNum; t++)
                {
                    TrialPool.Add(i + j * 10);
                }
            }
        }
        //Debug.Log("TrialPoos length = " + TrialPool.Count);
    }


    public bool InRegion(Vector3 pos)
    {
        if (pos.x != 0 && Mathf.Abs(pos.z) / Mathf.Abs(pos.x) < Mathf.Tan(35 * Mathf.PI / 180))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void RegionDetection(GameObject targetObject)
    {
        bool isInRegion = true;
        if (targetObject.transform.position.x < roomLower.x + roomEdge)
        {
            targetObject.GetComponent<Transform>().position = new Vector3(roomLower.x + roomEdge, targetObject.transform.position.y, targetObject.transform.position.z);
            //Debug.Log("new position = " + targetObject.transform.position + " new x = " + (roomLower.x + roomEdge));
            Bounce(targetObject, new Vector3(1, 0, 0));
            isInRegion = false;            
        }
        if (targetObject.transform.position.x > roomUpper.x - roomEdge)
        {
            targetObject.GetComponent<Transform>().position = new Vector3(roomUpper.x - roomEdge, targetObject.transform.position.y, targetObject.transform.position.z);
            //Debug.Log("new position = " + targetObject.transform.position + " new x = " + (roomUpper.x - roomEdge));
            Bounce(targetObject, new Vector3(-1, 0, 0));
            isInRegion = false;
        }

        if (targetObject.transform.position.y < roomLower.y + roomEdge)
        {
            targetObject.GetComponent<Transform>().position = new Vector3(targetObject.transform.position.x, roomLower.y + roomEdge, targetObject.transform.position.z);
            //Debug.Log("new position = " + targetObject.transform.position + " new y = " + (roomUpper.y + roomEdge));
            Bounce(targetObject, new Vector3(0, 1, 0));
            isInRegion = false;
        }
        if (targetObject.transform.position.y > roomUpper.y - roomEdge)
        {
            targetObject.GetComponent<Transform>().position = new Vector3(targetObject.transform.position.x, roomUpper.y - roomEdge, targetObject.transform.position.z);
            //Debug.Log("new position = " + targetObject.transform.position + " new y = " + (roomUpper.y - roomEdge));
            Bounce(targetObject, new Vector3(0, -1, 0));
            isInRegion = false;
        }

        if (targetObject.transform.position.z < roomLower.z + roomEdge)
        {
            targetObject.GetComponent<Transform>().position = new Vector3(targetObject.transform.position.x, targetObject.transform.position.y, roomLower.z + roomEdge);
            //Debug.Log("new position = " + targetObject.transform.position + " new z = " + (roomLower.z + roomEdge));
            Bounce(targetObject, new Vector3(0, 0, 1));
            isInRegion = false;
        }
        if (targetObject.transform.position.z > roomUpper.z - roomEdge)
        {
            targetObject.GetComponent<Transform>().position = new Vector3(targetObject.transform.position.x, targetObject.transform.position.y, roomUpper.z - roomEdge);
            //Debug.Log("new position = " + targetObject.transform.position + " new z = " + (roomUpper.z - roomEdge));
            Bounce(targetObject, new Vector3(0, 0, -1));
            isInRegion = false;
        }

        if (isInRegion)
        {
           // UpdateTargetsDirection(targetObject);
        }
    }

    public void UpdateTargetsDirection(GameObject targetObject)
    {
        float velocity;
        float dir_theta;
        float dir_alpha;
        Vector3 targetVelocity = targetObject.GetComponent<Rigidbody>().velocity;
        if(targetVelocity.magnitude == 0)
        {
            velocity = current_velocity;
            dir_theta = UnityEngine.Random.Range(0, Mathf.PI);
            dir_alpha = UnityEngine.Random.Range(0, 2 * Mathf.PI);
        }
        else
        {
            //Debug.Log("target velocity = " + targetVelocity);
            float x = targetVelocity.x;
            float y = targetVelocity.y;
            float z = targetVelocity.z;
            //float velocity = Mathf.Sqrt(x * x + y * y + z * z);
            velocity = current_velocity;
            dir_theta = Mathf.Acos(y / Mathf.Sqrt(x * x + y * y + z * z));
            if (z > 0)
                dir_alpha = Mathf.Acos(x / Mathf.Sqrt(x * x + z * z));
            else
                dir_alpha = -Mathf.Acos(x / Mathf.Sqrt(x * x + z * z));

            // Geralize new direction of the target randomly varies within a cone of ten degree vertical angle. 
            float delt_theta = UnityEngine.Random.Range(-Mathf.PI / 36, Mathf.PI / 36);
            dir_theta += delt_theta;
        }

        //new_theta = Mathf.Max(0, new_theta);
        //new_theta = Mathf.Min(Mathf.PI, new_theta);
        //Debug.Log("theta = " + new_theta + " alpha = " + dir_alpha);
        targetObject.GetComponent<Rigidbody>().velocity = new Vector3(velocity * Mathf.Sin(dir_theta) * Mathf.Cos(dir_alpha), velocity * Mathf.Cos(dir_theta), velocity * Mathf.Sin(dir_theta) * Mathf.Sin(dir_alpha));
        targetObject.GetComponent<ElasticCollision>().SetVelocity(targetObject.GetComponent<Rigidbody>().velocity);

    }
}
