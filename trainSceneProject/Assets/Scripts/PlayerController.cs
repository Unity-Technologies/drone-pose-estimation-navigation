using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public Camera cam;

    public NavMeshAgent agent;

    private GameObject targetObject;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ////Debug.Log("Pressed primary button.");
            //Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            //RaycastHit hit;

            //if (Physics.Raycast(ray, out hit))
            //{
            //    // move the agent
            //    Debug.Log(hit.point);

            //    agent.SetDestination(hit.point);
            //}


            targetObject = GameObject.Find("TargetCube_modified");
            var targetPosition = targetObject.transform.position;

            agent.SetDestination(targetPosition);
        }

    }
}
