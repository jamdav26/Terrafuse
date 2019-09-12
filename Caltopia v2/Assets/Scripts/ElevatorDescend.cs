using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorDescend : MonoBehaviour
{
    public float movementSpeed = 10;
    public GameObject endPoint;

    // Update is called once per frame
    void Update()
    {
    	Vector3 targetPosition = endPoint.transform.position;

	    Vector3 currentPosition = this.transform.position;
	    //first, check to see if we're close enough to the target
	    if(Vector3.Distance(currentPosition, targetPosition) > .1f) { 
	        Vector3 directionOfTravel = targetPosition - currentPosition;
	        //now normalize the direction, since we only want the direction information
	        directionOfTravel.Normalize();
	        //scale the movement on each axis by the directionOfTravel vector components

	        this.transform.Translate(
	            (directionOfTravel.x * movementSpeed * Time.deltaTime),
	            (directionOfTravel.y * movementSpeed * Time.deltaTime),
	            (directionOfTravel.z * movementSpeed * Time.deltaTime),
	            Space.World);
	    }
    }
}