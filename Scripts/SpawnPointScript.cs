using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointScript : MonoBehaviour
{
	private Unit unit;
    // Start is called before the first frame update
    void Start()
    {
		unit = gameObject.GetComponent<Unit>();
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("PlayerProjectile"))
		{
			unit.HandleDamage(other);
		}
	}
}
