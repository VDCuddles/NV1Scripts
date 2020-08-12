using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastMovement : MonoBehaviour
{

	private GameObject parent;

	void FixedUpdate()
	{
		if (parent && parent == GameObject.FindGameObjectWithTag("Player"))
			moveCastAnim(GetComponentInChildren<SpriteRenderer>().flipX);
	}

	private void moveCastAnim(bool isFlipped)
	{

		GameObject player = GameObject.FindGameObjectWithTag("Player");
		Vector3 newPos = player.transform.position;
		newPos.x += (isFlipped ? -1.0f*player.GetComponentInChildren<WeaponScript>().GetLeftProjXOffset() : player.GetComponent<ProjectileController>().GetPosOffsetX());
		newPos.y += (isFlipped ? player.GetComponentInChildren<WeaponScript>().GetLeftProjYOffset() : player.GetComponent<ProjectileController>().GetPosOffsetY());
		newPos.z += -1.0f;
		transform.position = newPos;
	}

	public void DefineParent(GameObject parent)
	{
		this.parent = parent;
	}
}
