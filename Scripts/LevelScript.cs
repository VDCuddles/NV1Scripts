using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelScript : MonoBehaviour
{
	[SerializeField] private string criteria = default;
	[SerializeField] private int totalLevelsInStage = default;
	[SerializeField] private GameObject player = default;
	[SerializeField] private AudioClip openDoor = default;
	[SerializeField] private GameObject musicPlayer = default;

	GameObject[] exits;
	private bool lootDropped = false;
	private Vector3 spawnPosition;
	public int GetTotalLevelsInStage() { return totalLevelsInStage; }
	public AudioClip GetOpenDoor() { return openDoor;  }

	private void Start()
	{
		spawnPosition = new Vector3(-5.47f, -1.52f, -1.52f);
		exits = GameObject.FindGameObjectsWithTag("Exit");
		foreach (GameObject exit in exits)
		{
			exit.SetActive(false);
		}
		if (!GameObject.FindGameObjectWithTag("Player"))
			SpawnPlayer();
		if (!GameObject.FindGameObjectWithTag("MusicPlayer"))
		{
			GameObject mp = Instantiate(musicPlayer, spawnPosition, Quaternion.identity);
			mp.name = musicPlayer.name;
		}
		else
		{
			GameObject.FindGameObjectWithTag("Player").transform.position = spawnPosition;
			GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().SetAtDoor(false);
			GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().SetAtLoot(false);
		}
	}

	public void SpawnPlayer()
	{
		GameObject player = Instantiate(this.player, spawnPosition, Quaternion.identity);
		player.name = this.player.name;
		GameObject.Find("PlayerHealthBar").GetComponent<HealthBar>().setUnit(player.GetComponent<Unit>());
	}

	void Update()
	{
		StartCoroutine(CheckProgression());
	}

	IEnumerator CheckProgression()
	{
		yield return new WaitForSeconds(0.1f);
		if (CriteriaMet())
		{
			foreach (GameObject exit in exits)
				exit.SetActive(true);
			if(!lootDropped)
			{
				DropLoot();
			}

		}
	}

	private bool CriteriaMet()
	{
		string condition = criteria;
		switch (condition)
		{
			case "SpawnsDoneEnemiesDead":
				if (SpawnsDoneEnemiesDead())
					return true;
				else
					return false;
			case "SpawnsDeadEnemiesDead":
				if (SpawnsDeadEnemiesDead())
					return true;
				else
					return false;
		}
		return false;
	}

	private void DropLoot()
	{
		gameObject.GetComponent<LootManager>().DropLoot();
		lootDropped = true;
	}

	private bool SpawnsDoneEnemiesDead()
	{
		//currently registers this true for the instant that the last enemy is about to spawn
		if (AllEnemiesDead() && AllSpawnsEmpty())
			return true;
		return false;
	}

	private bool SpawnsDeadEnemiesDead()
	{
		//currently registers this true for the instant that the last enemy is about to spawn
		if (AllEnemiesDead() && AllSpawnsDead())
			return true;
		return false;
	}

	public bool AllEnemiesDead()
	{
		GameObject[] uArray = GameObject.FindGameObjectsWithTag("Enemies");
		foreach (GameObject g in uArray)
			if (!g.GetComponent<Unit>().IsDead())
				return false;
		return true;
	}

	public bool AllSpawnsEmpty()
	{
		List<SpawnController> spawnControllers = new List<SpawnController>();
		GameObject[] sArray = GameObject.FindGameObjectsWithTag("SpawnPoint");
		foreach (GameObject g in sArray)
			spawnControllers.Add(g.GetComponent<SpawnController>());
		foreach (SpawnController s in spawnControllers)
		{
			if (!s.NoSpawnsLeft())
				return false;
		}
		return true;
	}

	public bool AllSpawnsDead()
	{
		GameObject[] sArray = GameObject.FindGameObjectsWithTag("SpawnPoint");
		foreach (GameObject s in sArray)
		{
			if (!s.GetComponent<Unit>().IsDead())
				return false;
		}
		return true;
	}
}
