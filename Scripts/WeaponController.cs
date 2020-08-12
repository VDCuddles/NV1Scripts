using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
	private GameObject currentWeapon;
	[SerializeField] private GameObject shotgun = default;
	[SerializeField] private GameObject zombieHand = default;
	[SerializeField] private GameObject necroGauntlet = default;

	private List<GameObject> weaponList = new List<GameObject>();

	void Start()
    {
		weaponList.AddRange(new GameObject[] { shotgun, zombieHand , necroGauntlet });
	}

    // Update is called once per frame
    void Update()
    {
		currentWeapon = GameObject.FindGameObjectWithTag("Weapon");
	}


	public void SwapWeapon()
	{
		//needs work
		int index = 0;
		for (int i = 0; i < weaponList.Count; i++)
		{
			if (currentWeapon.name == weaponList[i].name)
			{
				int container = i;
				if (i + 2 > weaponList.Count)
					container = 0;
				else
					container = i + 1;
				index = container;
			}
		}
		Destroy(currentWeapon);
		GameObject newWeapon = Instantiate(weaponList[index], (GameObject.Find("PlayerBody").transform.position + weaponList[index].transform.position), Quaternion.identity, GameObject.Find("PlayerBody").transform);
		newWeapon.name = weaponList[index].name;
		gameObject.GetComponent<ProjectileController>().SetCastAnim(newWeapon.GetComponent<WeaponScript>().GetCastAnim());
		gameObject.GetComponent<ProjectileController>().SetProjectilePrefab(newWeapon.GetComponent<WeaponScript>().GetProjectile());

	}
}
