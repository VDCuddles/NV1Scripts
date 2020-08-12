using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resurrector : MonoBehaviour
{
	[SerializeField] private GameObject FriendlyBElem = default;
	[SerializeField] private GameObject FriendlyBA = default;
	[SerializeField] private GameObject resAnim = default;
	[SerializeField] private GameObject healAnim = default;
	[SerializeField] private float resurrectionCooldown = default;
	[SerializeField] private int resurrectionLimit = default;
	[SerializeField] private int healthGainFactor = default;
	[SerializeField] private AudioClip resSound = default;
	[SerializeField] private AudioClip healSound = default;
	[SerializeField] private AudioClip readySound = default;

	private Transform target;
    private float resurrectionTimeStamp = 0f;
    private int friendCount;
    private bool readyToRes = true;
	private bool readyPlayed = false;
	private Unit unit;

    public bool GetReadyToRes() { return readyToRes; }
	public float GetResurrectionCooldown() { return resurrectionCooldown; }
	public float GetResurrectionTimeStamp() { return resurrectionTimeStamp; }

	void Start()
    {
		unit = gameObject.GetComponent<Unit>();
    }

    void Update()
    {
        CheckReady();
		CountFriends();
	}

	public void AbsorbHealth() 
	{
		if (Time.time > (resurrectionTimeStamp + resurrectionCooldown))
		{
			readyToRes = true;

			target = unit.GetClosestEnemy(GameObject.FindGameObjectWithTag("Player"));
			if (target && target.GetComponent<Unit>().IsDead() && target.GetComponent<Unit>().CheckResProximity())
			{
				Destroy(target.gameObject);
				if (unit.GetHealth() < 30)
				{
					unit.SetHealth(unit.GetHealth() + healthGainFactor * 15);
				}
				else if (unit.GetHealth() >= 30 && unit.GetHealth() < 50)
				{
					unit.SetHealth(unit.GetHealth() + healthGainFactor * 8);
				}
				else if(unit.GetHealth() >= 50 && unit.GetHealth() < 75)
				{
					unit.SetHealth(unit.GetHealth() + healthGainFactor * 5);
				}
				else
				{
					unit.SetHealth(unit.GetHealth() + healthGainFactor * 3);
				}
				AudioSource aso = gameObject.GetComponent<AudioSource>();
				aso.PlayOneShot(healSound);

				resurrectionTimeStamp = Time.time;
				GameObject hAnim = Instantiate(healAnim, target.position, Quaternion.identity, GameObject.Find("Friendlies").transform);
				hAnim.name = healAnim.name;
				Destroy(hAnim, 0.5f);
				readyPlayed = false;
			}
		}
		else
		{
			readyToRes = false;
		}
	}

    public void Resurrect()
    {
        if ((Time.time > (resurrectionTimeStamp + resurrectionCooldown)) && (friendCount < resurrectionLimit))
		{
			readyToRes = true;
			target = unit.GetClosestEnemy(GameObject.FindGameObjectWithTag("Player"));
			if (target && target.GetComponent<Unit>().IsDead() && target.GetComponent<Unit>().CheckResProximity())
			{
				Destroy(target.gameObject);
				GameObject spawn = Instantiate(checkSpawnType(), target.position, Quaternion.identity, GameObject.Find("Friendlies").transform);
				spawn.name = checkSpawnType().name;
				resurrectionTimeStamp = Time.time;
				AudioSource aso = gameObject.GetComponent<AudioSource>();
				aso.PlayOneShot(resSound);
				GameObject rAnim = Instantiate(resAnim, target.position, Quaternion.identity, GameObject.Find("Friendlies").transform);
				rAnim.name = resAnim.name;
				Destroy(rAnim, 0.5f);
				readyPlayed = false;
			}
		}
		else
        {
            readyToRes = false;
        }

    }

	private void CountFriends()
	{
		var allies = new List<GameObject>();
		foreach (GameObject ally in GameObject.FindGameObjectsWithTag("Friendlies"))
		{
				if (ally && !ally.GetComponent<Unit>().IsDead())
					allies.Add(ally);
		}
		friendCount = allies.Count;
	}

	private void CheckReady()
    {
        if ((Time.time > (resurrectionTimeStamp+resurrectionCooldown)) && (friendCount < resurrectionLimit))
		{
			readyToRes = true;
			if (!readyPlayed)
			{
				AudioSource aso = gameObject.GetComponent<AudioSource>();
				aso.PlayOneShot(readySound);
				readyPlayed = true;
			}
		}
    }
    private GameObject checkSpawnType()
    {
        switch (target.name)
        {
            case "Blood Apparition":
                return FriendlyBA;
            case "Blood Elemental":
                return FriendlyBElem;
        }
        return FriendlyBElem;
    }
}
