using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCorpseScript : MonoBehaviour
{
	[SerializeField] private GameObject player = default;
	[SerializeField] private AudioClip selfResSound = default;
	[SerializeField] private GameObject resAnim = default;
	private InputController inputController;
	AudioSource aso;

	private void Start()
	{
		inputController = FindObjectOfType<InputController>();
		aso = GameObject.Find("Environment").GetComponent<AudioSource>();
	}
	private void Update()
	{
			StartResAnim();		
	}

	public void StartResAnim()
	{
		if (Input.GetKeyDown(inputController.Resurrect))
		{
			//this calls Resurrect() from the animator
			gameObject.GetComponentInChildren<Animator>().SetTrigger("Resurrect");
			aso.clip = selfResSound;
			aso.Play();

			GameObject rAnim = Instantiate(resAnim, transform.position, Quaternion.identity, GameObject.Find("Friendlies").transform);
			rAnim.name = resAnim.name;
			Destroy(rAnim, 0.5f);
		}
	}
	public void Resurrect()
	{
		if (!GameObject.FindGameObjectWithTag("Player"))
		{
			GameObject player = Instantiate(this.player, gameObject.GetComponentInParent<Transform>().position, Quaternion.identity);
			player.name = this.player.name;
			GameObject.Find("PlayerHealthBar").GetComponent<HealthBar>().setUnit(player.GetComponent<Unit>());
		}
		Destroy(gameObject);
		
	}

}
