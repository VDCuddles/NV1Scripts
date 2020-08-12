using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

	private Unit unit;
	private Rigidbody rb;
	private ProjectileController pc;
	private Resurrector res;
	private Animator weaponAnim;
	private InputController inputController;
	private float noMovementThreshold = 0.0001f;
	private const int noMovementFrames = 3;
	private bool atDoor = false;
	private bool atLoot = false;
	private WeaponScript weaponScript;
	Vector3[] previousLocations = new Vector3[noMovementFrames];

	[SerializeField] private float walkSpeed = default;
	[SerializeField] private Animator torsoAnim = default;
	[SerializeField] private Animator legsAnim = default;
	[SerializeField] private Animator gownFrontAnim = default;
	[SerializeField] private Animator gownBackAnim = default;
	[SerializeField] private GameObject corpse = default;

	public void SetAtDoor(bool value) { atDoor = value; }
	public void SetAtLoot(bool value) { atLoot = value; }
	public GameObject GetCorpse() { return corpse; }
	public float GetWalkSpeed() { return walkSpeed; }

	void Awake()
	{
		for (int i = 0; i < previousLocations.Length; i++)
		{
			previousLocations[i] = Vector3.zero;
		}
	}

	void Start()
	{
		unit = gameObject.GetComponent<Unit>();
		rb = GetComponent<Rigidbody>();
		pc = GetComponent<ProjectileController>();
		res = GetComponent<Resurrector>();
		inputController = FindObjectOfType<InputController>();

		//ignore short bounds
		Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), GameObject.Find("ShortUpperBounds").GetComponent<Collider>(), true);

	}
	private void FixedUpdate()
	{
		PlayerWalkHandler();
		weaponAnim = GameObject.FindGameObjectWithTag("Weapon").GetComponent<Animator>();
		if (gameObject.GetComponentInChildren<WeaponScript>())
			weaponScript = gameObject.GetComponentInChildren<WeaponScript>();
	}
	private void Update()
	{
		CheckZStopped();
		if (Input.GetKeyDown(inputController.Resurrect))
		{
			res.Resurrect();
		}
		if (Input.GetKeyDown(inputController.Heal))
		{
			res.AbsorbHealth();
		}
		if (Input.GetKeyDown(inputController.SwapWeapon))
		{
			gameObject.GetComponent<WeaponController>().SwapWeapon();
		}
		if (Input.GetKeyDown(inputController.Interact))
		{
			if (atDoor)
			{
				GameObject.FindGameObjectWithTag("FadeWall").GetComponent<SceneChanger>().ChooseLevelAndFade();
				GameObject[] exits = GameObject.FindGameObjectsWithTag("Exit");
				foreach (GameObject exit in exits)
					exit.SetActive(false);
				GameObject.Find("Environment").GetComponent<AudioSource>().PlayOneShot(FindObjectOfType<LevelScript>().GetOpenDoor());
			}
			if (atLoot)
			{
				foreach (GameObject go in GameObject.FindGameObjectsWithTag("Loot"))
					Destroy(go);
				FindObjectOfType<LootManager>().ChooseLootAndDisplay();
			}
		}
		if (weaponAnim)
		{
			//not loaded, yet reloading, and key pressed
			if (!pc.GetIsLoaded() && (Input.GetKey(inputController.ShootLeft) || Input.GetKey(inputController.ShootRight)) && !pc.GetIsReloading())
			{
				SetShootingBools("Right", false);
				SetShootingBools("Left", false);
				pc.SetIsReloading(true);
				if (Input.GetKey(inputController.ShootRight))
				{
					torsoAnim.SetBool("isShootingRight", true);
					weaponAnim.SetBool("isReloadingRight", true);
				}
				else if (Input.GetKey(inputController.ShootLeft))
				{
					torsoAnim.SetBool("isShootingLeft", true);
					weaponAnim.SetBool("isReloadingRight", true);
				}
				StartCoroutine(pc.Reload());
			}
			if (pc.GetIsLoaded() && weaponScript.GetNeedsReload())
			{
				weaponAnim.SetBool("isReloadingRight", false);
				weaponAnim.SetBool("isReloadingLeft", false);
			}
			//loaded and not reloading (ready)
			if (pc.GetIsLoaded() && !pc.GetIsReloading())
			{
				torsoAnim.SetBool("isShootingRight", false);
				torsoAnim.SetBool("isShootingLeft", false);

				if (Input.GetKey(inputController.ShootLeft))
				{
					float origProjXOffset = pc.GetPosOffsetX();
					float origProjYOffset = pc.GetPosOffsetY();
					pc.SetPosOffsetX(weaponScript.GetLeftProjXOffset());
					pc.SetPosOffsetY(weaponScript.GetLeftProjYOffset());
					pc.FireProjectile(isFiringRight: false);
					SetShootingBools("Left", true);
					pc.SetPosOffsetX(origProjXOffset);
					pc.SetPosOffsetY(origProjYOffset);
				}
				if (Input.GetKey(inputController.ShootRight))
				{
					pc.FireProjectile(isFiringRight: true);
					SetShootingBools("Right", true);
				}
			}
			else
			{
				SetShootingBools("Left", false);
				SetShootingBools("Right", false);
			}
			if (!weaponScript.GetNeedsReload() && !(Input.GetKey(inputController.ShootLeft) || Input.GetKey(inputController.ShootRight)))
			{
				SetShootingBools("Left", false);
				SetShootingBools("Right", false);
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("EnemyProjectile"))
		{
			unit.HandleDamage(other);
		}
		else if (other.CompareTag("Exit"))
			atDoor = true;
		else if (other.CompareTag("Loot"))
			atLoot = true;
	}
	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Exit"))
			atDoor = false;
		else if (other.CompareTag("Loot"))
			atLoot = false;
	}

	bool CheckZStopped()
	{
		for (int i = 0; i < previousLocations.Length - 1; i++)
		{
			previousLocations[i] = previousLocations[i + 1];
		}
		previousLocations[previousLocations.Length - 1] = transform.position;

		for (int i = 0; i < previousLocations.Length - 1; i++)
		{
			if (Mathf.Abs(previousLocations[i].z - previousLocations[i + 1].z) <= noMovementThreshold)
			{
				return true;
			}
		}
		return false;
	}

	void PlayerWalkHandler()
	{
		if (!gameObject.GetComponent<Unit>().IsDead())
		{
			float hAxis, vAxis;
			hAxis = Input.GetAxis("Horizontal");
			vAxis = Input.GetAxis("Vertical");
			Vector3 movement = new Vector3(hAxis, vAxis, vAxis) * GetWalkSpeed() * Time.deltaTime;
			if (CheckZStopped())
			{
				movement.y = movement.z;
			}
			rb.velocity = movement;
			WalkAnimHAxis(hAxis, vAxis);
			WalkAnimVAxis(hAxis, vAxis);
			if (!Input.GetKey(inputController.MoveUp) && !Input.GetKey(inputController.MoveDown) && !Input.GetKey(inputController.MoveLeft) && !Input.GetKey(inputController.MoveRight))
				rb.velocity = Vector3.zero;
		}
	}

	private void SetShootingBools(string shootingdir, bool value)
	{
		torsoAnim.SetBool("isShooting" + shootingdir, value);
		weaponAnim.SetBool("isShooting" + shootingdir, value);
	}

	private void SetWalkingBools(string walkingdir, bool value)
	{
		legsAnim.SetBool("isWalking" + walkingdir, value);
		gownFrontAnim.SetBool("isWalking" + walkingdir, value);
		gownBackAnim.SetBool("isWalking" + walkingdir, value);
	}

	public void WalkAnimVAxis(float hAxis, float vAxis)
	{
		if (vAxis != 0)
		{
			SetWalkingBools("Left", true);
		}
		//todo: do some up and down walking animations
		if (!Input.GetKey(inputController.MoveUp) && !Input.GetKey(inputController.MoveDown) && !Input.GetKey(inputController.MoveLeft) && !Input.GetKey(inputController.MoveRight))
		{
			SetWalkingBools("Right", false);
			SetWalkingBools("Left", false);

		}
	}

	public void WalkAnimHAxis(float hAxis, float vAxis)
	{
		if (hAxis < 0)
		{
			SetWalkingBools("Left", true);
			torsoAnim.SetBool("isShootingLeft", true);
		}
		else if (hAxis > 0)
		{
			SetWalkingBools("Right", true);
			torsoAnim.SetBool("isShootingRight", true);
		}
	}
}