using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundary : MonoBehaviour
{

	private Vector3 lowerBounds;
	private Vector3 upperBounds;
	private Vector3 leftBounds;
	private Vector3 rightBounds;

	public Vector3 getLowerBounds() { return lowerBounds; }
	public Vector3 getUpperBounds() { return upperBounds; }
	public Vector3 getLeftBounds() { return leftBounds; }
	public Vector3 getRightBounds() { return rightBounds; }

	void Start()
	{
		lowerBounds = GameObject.Find("LowerBounds").transform.position;
		upperBounds = GameObject.Find("UpperBounds").transform.position;
		leftBounds = GameObject.Find("LeftBounds").transform.position;
		rightBounds = GameObject.Find("RightBounds").transform.position;

	}

	public void ConstrainBoundaries(Transform unit)
	{
		float unitWidth = unit.GetComponent<BoxCollider>().bounds.extents.x;
		Vector3 newPos = unit.position;
		newPos.x = Mathf.Clamp(newPos.x, leftBounds.x + unitWidth, rightBounds.x - unitWidth);
		newPos.y = Mathf.Clamp(newPos.y, lowerBounds.y, upperBounds.y);
		unit.position = newPos;
		ConstrainObstacles(unit);
	}

	private void ConstrainObstacles(Transform unit)
	{
		float unitWidth = unit.GetComponent<BoxCollider>().bounds.extents.x;
		float unitDepth = unit.GetComponent<BoxCollider>().bounds.extents.y;
		Bounds unitBounds = unit.GetComponent<BoxCollider>().bounds;

		foreach (GameObject obstacle in GameObject.FindGameObjectsWithTag("Obstacle"))
		{
			float obstacleWidth = obstacle.GetComponent<BoxCollider>().bounds.extents.x;
			Bounds obstacleBounds = obstacle.GetComponent<BoxCollider>().bounds;
			Vector3 newPos = unit.position;
			if (unitBounds.Intersects(obstacleBounds))
			{
				switch (CheckDirection(unitBounds, obstacleBounds))
				{
					case "left":
						newPos.x = Mathf.Clamp(newPos.x, leftBounds.x + unitWidth, obstacleBounds.min.x - unitWidth);
						break;
					case "right":
						newPos.x = Mathf.Clamp(newPos.x, obstacleBounds.max.x + unitWidth/2, rightBounds.x - unitWidth);
						break;
					case "down":
						newPos.x = unit.position.x;
						newPos.y = Mathf.Clamp(newPos.y, lowerBounds.y, obstacleBounds.min.y);
						break;
					case "up":
						newPos.x = unit.position.x;
						newPos.z = Mathf.Clamp(newPos.z, obstacleBounds.max.z, upperBounds.z);
						break;
				}
				newPos.y = newPos.z;
				unit.position = newPos;
			}
		}
	}

	private string CheckDirection(Bounds unitBounds, Bounds obstacleBounds)
	{

		float xAbs = Mathf.Abs(unitBounds.center.x - obstacleBounds.center.x);
		float zAbs = Mathf.Abs(unitBounds.center.z - obstacleBounds.center.z);
		float xRelation = unitBounds.center.x - obstacleBounds.center.x;
		float yRelation = unitBounds.center.z - obstacleBounds.center.z;

		Debug.Log("xabs = " + xAbs);
		Debug.Log("zabs = " + zAbs);

		if (xAbs > zAbs)
			if (xRelation < 0.0f)
				return "left";
			else
				return "right";
		else
			if (yRelation > 0.0f)
				return "up";
			else
			return "down";

		//Vector3 direction = (unitBounds.center - obstacleBounds.center).normalized;
		//float xAbs = Mathf.Abs(direction.x);
		//float zAbs = Mathf.Abs(direction.z);

		//if (direction.x < 0.0f)
		//	return "left";
		//else if (direction.x > 0.0f)
		//	return "right";
		//if (direction.y > 0.0f)
		//	return "up";
		//else if (direction.y < 0.0f)
		//	return "down";
		//else
		//	return "none";

		//if (Input.GetAxis("Horizontal") != 0.0f)
		//	if (Input.GetAxis("Horizontal") > 0.0f)
		//		return "left";
		//	else
		//		return "right";
		//if (Input.GetAxis("Vertical") != 0.0f)
		//	if (Input.GetAxis("Vertical") < 0.0f)
		//		return "up";
		//	else
		//		return "down";
		//return "none";
	}
}
