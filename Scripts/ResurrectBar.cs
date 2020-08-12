using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResurrectBar : MonoBehaviour
{
	[SerializeField] Image image = default;

	private float resurrectionCooldown;

	void Start()
	{
	}

	void Update()
	{
		if(GameObject.FindGameObjectWithTag("Player"))
		{
			resurrectionCooldown = GameObject.FindGameObjectWithTag("Player").GetComponent<Resurrector>().GetResurrectionCooldown();
			float valuetoset = Mathf.Abs(GameObject.FindGameObjectWithTag("Player").GetComponent<Resurrector>().GetResurrectionTimeStamp() - Time.time) / resurrectionCooldown;
			image.fillAmount = valuetoset;
		}
	}
}
