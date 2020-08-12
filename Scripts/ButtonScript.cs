using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour
{
	[SerializeField] private SceneChanger sceneChanger = default;
	[SerializeField] private AudioClip audioClip = default;
	[SerializeField] private SpriteRenderer sr = default;
	private InputController inputController;
	private AudioSource audioSource = default;

	private void Start()
	{
		audioSource = gameObject.GetComponent<AudioSource>();
		StartCoroutine(WaitForFade());
		inputController = gameObject.AddComponent<InputController>();
	}
	private void Update()
	{
		if (Input.GetKeyDown(inputController.Interact))
		{
			TriggerAnimator("Level1");
		}
	}

	public void TriggerAnimator(string targetLevel)
	{
		if (sr.color.a == 0)
		{
			PlayConfirm();
			sceneChanger.FadeToLevel(targetLevel);
		}
	}

	private void PlayConfirm()
	{
		audioSource.PlayOneShot(audioClip);
	}

	IEnumerator WaitForFade()
	{
		yield return new WaitForSeconds(4);
		gameObject.GetComponent<Animator>().SetTrigger("readyToUse");
		gameObject.GetComponent<Button>().interactable = true;
	}
}
