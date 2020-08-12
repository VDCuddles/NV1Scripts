using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootManager : MonoBehaviour
{
	[SerializeField] private GameObject lootDrop = default;


	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void DropLoot()
	{
		GameObject loot = Instantiate(lootDrop, Vector3.zero, Quaternion.identity, GameObject.Find("Environment").transform);
	}

	public void ChooseLootAndDisplay()
	{

	}
}
