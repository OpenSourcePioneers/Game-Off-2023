using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovement : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private TwoD_Grid grid;
    [SerializeField] private float speed;

    Rigidbody rb;
    Vector3[] path = new Vector3[0];
    Vector3 vec;
    TwoD_Node targetNode;
    int pathInd;
    bool gettingPath = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        PushPathRequest();
    }

    // Update is called once per frame
    void Update()
    {
        if(player.grounded && TwoD_Grid.NodeFromWorldPosition(player.transform.position) != targetNode)
        {
            PushPathRequest();
        }

        if(!gettingPath)
        {
            CheckForWayPoints();
        }
    }
    void FixedUpdate() 
    {
        Move();
    }

    void CheckPath(Vector3[] _path, bool pathFound)
    {
        if(pathFound)
        {
            pathInd = 0;
            path = _path;
            gettingPath = false;
            CheckForWayPoints();
        }
    }

    private void Move()
    {
        //Movement
        if(rb.velocity.sqrMagnitude < speed * speed && !gettingPath)
            rb.AddForce(vec.normalized * speed * 5000f * Time.fixedDeltaTime, ForceMode.Force);
    }

    private void CheckForWayPoints()
    {
         if(pathInd > path.Length || path == null || gettingPath)
            return;
        vec = path[pathInd] - transform.position;
        transform.forward = vec;
        if(vec.sqrMagnitude < 1)
        pathInd++;
    }

    private void PushPathRequest()
    {
        
        targetNode = TwoD_Grid.NodeFromWorldPosition(player.transform.position);
        gettingPath = true;
        PathManager.RequestPath(transform.position, player.transform.position, CheckPath);
    }


    void OnDrawGizmos() 
    {
        if(path == null || !Application.isPlaying)
            return;
        for(int i = pathInd; i < path.Length; i++)
        {
            Gizmos.DrawCube(path[i], Vector3.one * grid.nodeRadius);
            if(i+1 < path.Length)
                Gizmos.DrawLine(path[i], path[i+1]);
        }
        //if(pathInd < path.Length)
            //Gizmos.DrawLine(transform.position, path[pathInd]);
    }
}
