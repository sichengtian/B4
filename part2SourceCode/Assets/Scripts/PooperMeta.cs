using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooperMeta : MonoBehaviour {
    public GameObject stallSpot;
    public GameObject agent;
    public GameObject meetPoint;
    public GameObject toilet;
    public GameObject poopPoint;
    public GameObject clogSignal;
    public bool StillActive;

	// Use this for initialization
	void Start () {
        StillActive = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public bool IsActiveForTree()
    {
        return StillActive;
    }
}
