using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampLight : MonoBehaviour
{
    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    void Update()
    {
		if(player)
			gameObject.GetComponent<Light>().range = (Mathf.Abs(player.transform.position.z) + 16.0f);
    }
}
