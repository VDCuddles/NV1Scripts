using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{

    [SerializeField] private float shootRate = default;
    [SerializeField] private GameObject projectilePrefab = default;
    [SerializeField] private float posOffsetX = default;
	[SerializeField] private float posOffsetY = default;
	[SerializeField] private float castAnimLength = default;
	[SerializeField] private GameObject castAnim = default;

	private Rigidbody charRB;
	private Animator animator;
	private float shootRateTimeStamp = 0f;
	private GameObject projectileInstance;
    private Rigidbody mRbody;
	private Unit unit;
	private WeaponScript weaponscript;
	private bool isLoaded = true;
	private bool isReloading = false;
	private float recoilTime = 0f;
	private AudioSource source;

	public bool GetIsLoaded() { return isLoaded; }
	public bool GetIsReloading() { return isReloading; }
	public void SetIsReloading(bool value) { isReloading = value; }
	public void SetCastAnimLength(float value) { castAnimLength = value; }
	public void SetProjectilePrefab(GameObject value) { projectilePrefab = value; }
	public void SetCastAnim(GameObject value) { castAnim = value; }
	public float GetRecoilTime() { return recoilTime; }
	public float GetPosOffsetX() { return posOffsetX; }
	public float GetPosOffsetY() { return posOffsetY; }
	public void SetPosOffsetX(float value) { posOffsetX = value; }
	public void SetPosOffsetY(float value) { posOffsetY = value; }

	void Start()
    {
        charRB = gameObject.GetComponent<Rigidbody>();
        mRbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        unit = gameObject.GetComponent<Unit>();
		source = gameObject.GetComponent<AudioSource>();
	}
    void Update()
    {
		if (gameObject.GetComponentInChildren<WeaponScript>())
		{
			weaponscript = gameObject.GetComponentInChildren<WeaponScript>();
			shootRate = weaponscript.GetShootRate();
			posOffsetX = weaponscript.GetProjXOffset();
			posOffsetY = weaponscript.GetProjYOffset();
		}
    }

	public void FireProjectile( bool isFiringRight )
	{
		if (Time.time > shootRateTimeStamp && (!unit.IsDead()) && isLoaded)
		{
			//projectile instatiation
			Vector3 projPosition = new Vector3(transform.position.x + (isFiringRight ? posOffsetX : -posOffsetX), transform.position.y + posOffsetY, transform.position.z);
			GameObject projectileInstance = Instantiate(projectilePrefab, projPosition, Quaternion.identity, GameObject.Find("Projectiles").transform);
			projectileInstance.name = projectilePrefab.name;
			projectileInstance.GetComponent<ProjectileScript>().SetParent(gameObject);
			ProjectileScript projectileMovement = projectileInstance.GetComponent<ProjectileScript>();
			projectileMovement.FaceAndExpire(isFiringRight);
			shootRateTimeStamp = Time.time + shootRate;

			//cast animation
			if(castAnim)
			{
				GameObject casts = Instantiate(castAnim, projPosition, Quaternion.identity);
				casts.name = castAnim.name;
				casts.GetComponentInChildren<CastMovement>().DefineParent(gameObject);
				SpriteRenderer castsR = casts.GetComponentInChildren<SpriteRenderer>();
				castsR.flipX = !isFiringRight ? castsR.flipX = true : false;
				Destroy(casts, castAnimLength);
				if (weaponscript)
					if (weaponscript.GetNeedsReload())
						isLoaded = false;
			}
		}
	}

	public IEnumerator Reload()
	{
		yield return new WaitForSeconds(weaponscript.GetReloadTime());
		source.clip = weaponscript.GetBeginReload();
		source.Play();
		yield return new WaitForSeconds(0.3f);
		source.clip = weaponscript.GetEndReload();
		source.Play();
		isLoaded = true;
		yield return new WaitForSeconds(0.3f);
		isReloading = false;
	}

}
