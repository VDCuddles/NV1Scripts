using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootScript : MonoBehaviour
{
	public void AnimateLoot()
	{
		Animator a = gameObject.GetComponent<Animator>();
		a.SetTrigger("fadedIn");
	}
}
