using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicScript : MonoBehaviour
{
	private InputController inputController;
	private AudioSource audioSource;
	private GameObject fadeWall;
	public bool isFadingOut { get; set; }

    // Start is called before the first frame update
    void Start()
    {
		fadeWall = GameObject.FindGameObjectWithTag("FadeWall");
		audioSource = gameObject.GetComponent<AudioSource>();
		if (!FindObjectOfType<InputController>())
			inputController = gameObject.AddComponent<InputController>();
		else
			inputController = FindObjectOfType<InputController>();

		if (SceneManager.GetActiveScene().name != "MainMenu")
			DontDestroyOnLoad(transform.root.gameObject);
	}

	// Update is called once per frame
	void Update()
	{
		if (FindObjectOfType<MusicScript>())
		{
			if (Input.GetKeyDown(inputController.ToggleMusic))
			{
				audioSource.mute = !audioSource.mute;
			}

		}
		if (isFadingOut && SceneManager.GetActiveScene().name == "MainMenu")
			audioSource.volume = 1 - fadeWall.GetComponent<SpriteRenderer>().color.a;
	}

}
