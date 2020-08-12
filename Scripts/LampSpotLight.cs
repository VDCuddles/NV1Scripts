using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampSpotLight : MonoBehaviour
{
    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    void Update()
    {
        float zpercent = ((Mathf.Abs(player.transform.position.z) / 11.95352f));
		gameObject.GetComponent<Light>().spotAngle = 20f - (15f * zpercent);
		gameObject.GetComponent<Light>().intensity = 1.39f + (1.79f * zpercent);
    }
}