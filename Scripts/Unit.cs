using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
	private float damageTimeStamp = 0f;
	private bool hasDedSoundPlayed = false;
	private bool isResurrectable = false;
	private Material originalMat;
	private bool materialChanged = false;
	private int health;
	private static float fadeoutTime = 0.0005f;

	[SerializeField] private AudioClip dead = default;
	[SerializeField] private int maxHealth = default;
	[SerializeField] private AudioClip hurt = default;
	[SerializeField] private float damageIFrameRate = default;
	[SerializeField] private float interactableDistance = default;
	[SerializeField] private Material highlightMat = default;
	[SerializeField] private GameObject hurtEffect = default;
	[SerializeField] private GameObject bloodStain= default;
	[SerializeField] private float hurtAnimDuration = default;

	public AudioClip GetDeadClip() { return dead; }
	public int GetHealth() { return health; }
	public int GetMaxHealth() { return maxHealth; }
	public void SetHealth(int value) { health = value; }

	void Start()
	{
		originalMat = gameObject.GetComponentInChildren<SpriteRenderer>().material;
		if (gameObject.CompareTag("Friendlies"))
			DontDestroyOnLoad(transform.root.gameObject);
		health = maxHealth;
	}

	void FixedUpdate()
	{
		if (isResurrectable)
		{
			CheckResProximity();
		}
		ZSort();
		if (GameObject.FindGameObjectWithTag("Player") && !gameObject.CompareTag("SpawnPoint"))
			Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), GameObject.FindGameObjectWithTag("Player").GetComponent<Collider>(), true);
	}

	private void ZSort()
	{
			Vector3 pos = transform.position;
			pos.z = pos.y;
			gameObject.GetComponent<Rigidbody>().MovePosition(pos);
		//calculate force to move to like the player class is using?
		//may be worth blocking movement on the Z axis so any movement does its own movement z-sorting rather than this
	}

	public void HandleDamage(Collider co)
	{

		if (!IsDead())
		{
			if (Time.time > damageTimeStamp)
			{
				ProjectileScript projGo = co.GetComponentInParent<ProjectileScript>();
				SetHealth(GetHealth() - projGo.GetDamage());
				damageTimeStamp = Time.time + damageIFrameRate;
				if (GetComponent<AudioSource>().clip != hurt)
				{
					GetComponent<AudioSource>().clip = hurt;
				}
				GetComponent<AudioSource>().PlayOneShot(hurt);

				//blood spatter
				Vector3 position = new Vector3(transform.position.x, transform.position.y + gameObject.GetComponent<BoxCollider>().bounds.extents.y*1.5f);
				GameObject blood = Instantiate(hurtEffect, position, Quaternion.identity, GameObject.Find("Environment").transform);
				blood.name = hurtEffect.name;
				if (co && co.GetComponentInParent<ProjectileScript>().GetParent().transform.position.x > transform.position.x)
					blood.GetComponent<SpriteRenderer>().flipX = true;
				if (co && co.transform.parent.gameObject.name == "blankmelee" || co && co.transform.parent.gameObject.name == "Playerblankmelee")
					blood.GetComponent<SpriteRenderer>().flipX = !blood.GetComponent<SpriteRenderer>().flipX;

				Destroy(blood, hurtAnimDuration);
				StartCoroutine(LeaveBloodstain(co, blood.GetComponent<SpriteRenderer>().flipX));
				HandleDeath();
			}
		}

	}

	IEnumerator LeaveBloodstain(Collider co, bool isFlipped)
	{
		Vector3 position = gameObject.transform.position;
		GameObject blood = Instantiate(bloodStain, position, Quaternion.identity, GameObject.Find("Environment").transform);
		blood.name = bloodStain.name;
		blood.GetComponentInChildren<SpriteRenderer>().flipX = isFlipped;
		for (float t = 1.0f; t > 0.0f; t -= fadeoutTime)
		{
			if(blood)
			{
				Color newcolour = blood.GetComponentInChildren<SpriteRenderer>().color;
				newcolour.a = t;
				blood.GetComponentInChildren<SpriteRenderer>().color = newcolour;
				yield return null;
			}

		}
			Destroy(blood);
	}

	private void HandleDeath()
	{
		if (IsDead())
		{
			Collider co = gameObject.GetComponent<BoxCollider>();
			Rigidbody rb = gameObject.GetComponent<Rigidbody>();
			if (!gameObject.CompareTag("SpawnPoint"))
			{
				co.isTrigger = true;
				rb.isKinematic = true;
			}
			//set layer to corpses layer
			gameObject.layer = 10;
			foreach (Transform child in gameObject.GetComponentsInChildren<Transform>())
			{
				child.gameObject.layer = 10;
			}

			if (!gameObject.CompareTag("Player"))
			{
				gameObject.GetComponentInChildren<Canvas>().enabled = false;
				Animator animator = gameObject.GetComponentInChildren<Animator>();
				animator.SetBool("isDead", true);
			}
			
			if (!hasDedSoundPlayed)
			{
				GetComponent<AudioSource>().clip = dead;
				GetComponent<AudioSource>().PlayOneShot(dead);
				hasDedSoundPlayed = true;
			}
			if (gameObject.CompareTag("Enemies"))
			{
				isResurrectable = true;
			}
			if (gameObject.CompareTag("Friendlies"))
			{
				Destroy(gameObject, 6f);
			}
			if (gameObject.CompareTag("Player"))
			{
				GameObject corpse = Instantiate(gameObject.GetComponent<PlayerController>().GetCorpse(), transform.position, Quaternion.identity);
				corpse.name = gameObject.GetComponent<PlayerController>().GetCorpse().name;
				corpse.GetComponent<AudioSource>().PlayOneShot(dead);
				Destroy(gameObject);
			}
		}
	}
	public bool IsDead()
	{
		if (GetHealth() <= 0)
			return true;
		return false;
	}
	public bool CheckResProximity()
	{
		Transform player = default;
		if (GameObject.FindGameObjectWithTag("Player"))
		 player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

		if (GameObject.FindGameObjectWithTag("Player"))
		{
			if ((player.GetComponent<Resurrector>().GetReadyToRes()) && Vector3.Distance(transform.position, player.position) <= interactableDistance)
			{
				if (!materialChanged)
				{
					gameObject.GetComponentInChildren<SpriteRenderer>().material = highlightMat;
					materialChanged = true;
				}
				return true;
			}
			else
			{
				gameObject.GetComponentInChildren<SpriteRenderer>().material = originalMat;
				materialChanged = false;
			}
			return false;
		}
		return false;
	}

	public Transform GetClosestEnemy(GameObject thisUnit)
	{
		var enemylist = new List<GameObject>();
		GameObject[] enemyArray = null;
		Transform tMin = null;
		float minDist = Mathf.Infinity;
		Vector3 currentPos = transform.position;

		enemyArray = FillEnemyArray(thisUnit, enemylist, enemyArray);

		Transform[] enemTransArray = new Transform[enemyArray.Length];
		for (int i = 0; i < enemyArray.Length; i++)
		{
			enemTransArray[i] = enemyArray[i].transform;
		}

		foreach (Transform e in enemTransArray)
		{
			bool isEDead = false;
			if (e)
				isEDead = e.GetComponent<Unit>().IsDead();

			float dist = Vector3.Distance(e.position, currentPos);
			bool isPlayer = gameObject.CompareTag("Player");

			if ((dist < minDist) && e
			&& ((isPlayer && isEDead) || (!isPlayer && !isEDead)))
			{
				tMin = e;
				minDist = dist;
			}
		}
		return tMin;
	}

	public GameObject[] GetAllEnemies(GameObject thisUnit)
	{
		var enemylist = new List<GameObject>();
		GameObject[] enemyArray = null;
		enemyArray = FillEnemyArray(thisUnit, enemylist, enemyArray);
		return enemyArray;
	}

	private static GameObject[] FillEnemyArray(GameObject thisUnit, List<GameObject> enemylist, GameObject[] enemyArray)
	{
		if (thisUnit.CompareTag("Player"))
		{
			enemylist.AddRange(GameObject.FindGameObjectsWithTag("Enemies"));
			enemyArray = enemylist.ToArray();
		}
		else if (thisUnit.CompareTag("Enemies") && thisUnit.GetComponent<RangedEnemyScript>() == null)
		{
			enemylist.AddRange(GameObject.FindGameObjectsWithTag("Friendlies"));
			if (GameObject.FindGameObjectWithTag("Player"))
				enemylist.AddRange(GameObject.FindGameObjectsWithTag("Player"));
			enemyArray = enemylist.ToArray();
		}
		else if (thisUnit.CompareTag("Friendlies"))
		{
			enemylist.AddRange(GameObject.FindGameObjectsWithTag("Enemies"));
			enemyArray = enemylist.ToArray();
		}
		else if (thisUnit.CompareTag("Enemies") && thisUnit.GetComponent<RangedEnemyScript>())
		{
			if (GameObject.FindGameObjectWithTag("Player"))
				enemylist.AddRange(GameObject.FindGameObjectsWithTag("Player"));
			enemyArray = enemylist.ToArray();
		}

		return enemyArray;
	}


}




//interface IHaveHealth
//{
//    int CurrentHealth { get; }
//    void TakeDamage(int amount, string damageType);
//}

//class RegularHealth : IHaveHealth
//{
//    int health;
//    public int CurrentHealth
//    { get { return health; } }

//    public void TakeDamage(int amount, string damageType)
//    {
//        health -= amount;
//    }
//}


//class GhostHealth : IHaveHealth
//{
//    int health;
//    public int CurrentHealth
//        => health;

//    public void TakeDamage(int amount, string damageType)
//    {
//        if( damageType != "regular" )
//            health -= amount;
//    }
//}

//class Explosion
//{
//    void ExplodeOntoSomething( IHaveHealth theThingshealth)
//        => theThingshealth.TakeDamage(50, "regular");
//}

