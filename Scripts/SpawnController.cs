using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
	[SerializeField] private GameObject spawn = default;
	[SerializeField] private GameObject spawnAnim = default;
	[SerializeField] private float spawnRate = default;
	[SerializeField] private int spawnLimit = default;
	[SerializeField] private float spawnDelay = default;
	[SerializeField] private AudioClip spawnSound = default;

	private float spawnTimeStamp = 0f;
	private int spawnCount;
	public bool NoSpawnsLeft()	{ return (spawnCount >= spawnLimit);}

	void Start()
	{
		spawnDelay += Time.time;
	}

	void Update()
	{
		//TODO : only spawn if the spawn point has an alive unit component or does not have a unit component
		if(!gameObject.GetComponentInParent<Unit>().IsDead() || !gameObject.GetComponentInParent<Unit>())
			SpawnUnit(spawn);
	}

	void SpawnUnit(GameObject unit)
	{
		if (Time.time > spawnTimeStamp && Time.time > spawnDelay && spawnCount < spawnLimit)
		{
			//spawn unit
			GameObject newUnit = Instantiate(unit, transform.position, Quaternion.Euler(new Vector3(0, 0, 0)), GameObject.Find("Enemies").transform);
			newUnit.name = unit.name;
			AudioSource aso = gameObject.GetComponentInParent<AudioSource>();
			aso.clip = spawnSound;
			aso.Play();

			//spawn animation
			GameObject spawnA = Instantiate(spawnAnim, transform.position, Quaternion.Euler(new Vector3(0, 0, 0)), GameObject.Find("Enemies").transform);
			spawnA.name = spawnAnim.name;
			spawnTimeStamp = Time.time + spawnRate;
			spawnCount++;
			Destroy(spawnA, 0.5f);

		}
	}
}
