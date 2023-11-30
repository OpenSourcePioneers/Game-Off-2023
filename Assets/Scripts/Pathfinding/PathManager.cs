using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    TwoD_Pathfinding pathFinder;
    Queue<PathRequest> requests = new Queue<PathRequest>();
    PathRequest curRequest;

    static PathManager inst;
    bool isPathfinding = false;

    void Awake()
    {
        pathFinder = GetComponent<TwoD_Pathfinding>();
        inst = this;
    }

    public static void RequestPath(Vector3 initial, Vector3 target, Action<Vector3[], bool> callback)
    {
        PathRequest nRequest = new PathRequest(initial, target, callback);
        inst.requests.Enqueue(nRequest);
        inst.TryNextRequest();
    }

    private void TryNextRequest()
    {
        if(!isPathfinding && requests.Count > 0)
        {
            curRequest = requests.Dequeue();
            isPathfinding = true;
            pathFinder.StartPathfinding(curRequest.initial, curRequest.target);
        }
    }

    public void FinishedFinding(Vector3[] wayPoints, bool success)
    {
        curRequest.callback(wayPoints, success);
        isPathfinding = false;
        TryNextRequest();
    }
    
    struct PathRequest
    {
        public Vector3 initial;
        public Vector3 target;
        public Action<Vector3[], bool> callback;

        public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback)
        {
            initial = _start;
            target = _end;
            callback = _callback;
        }
    }
}
