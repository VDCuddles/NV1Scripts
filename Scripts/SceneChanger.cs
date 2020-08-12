using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
	[SerializeField] private Animator animator = default;
	private int totalLevelsInStage;
	private SpriteRenderer spriteRenderer;
	private string levelToLoad;

	private void Start()
	{
		Color color = gameObject.GetComponent<SpriteRenderer>().color;
		color.a = 0;
		if (gameObject.CompareTag("FadeWall"))
			gameObject.GetComponent<SpriteRenderer>().color = color;
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		if(FindObjectOfType<LevelScript>())
			totalLevelsInStage = FindObjectOfType<LevelScript>().GetTotalLevelsInStage();
	}

	private void Update()
	{
		if (SceneManager.GetActiveScene().name == "VoidSplash")
		{
			if (Input.anyKey)
			{
				FadeToLevel("MainMenu");
			}
			StartCoroutine(FadeAnyway());
		}
	}

	public void ChooseLevelAndFade()
	{
		string level = SceneManager.GetActiveScene().name;
		string numericlevel = new string(level.Where(char.IsDigit).ToArray());
		int nlevel = int.Parse(numericlevel);
		FadeToLevel(DecideNumber(nlevel));

	}

	private string DecideNumber(int currentlevel)
	{
		int randomNr = currentlevel;
		while (randomNr == currentlevel)
		{
			randomNr = UnityEngine.Random.Range(1, totalLevelsInStage + 1);
		}
		return "Level" + randomNr.ToString();
	}

	public void FadeToLevel(string level)
	{
		if (spriteRenderer.color.a == 0)
		{
			levelToLoad = level;
			animator.SetTrigger("FadeOut");
			MusicScript musicScript;
			if (FindObjectOfType<MusicScript>())
			{
				musicScript = FindObjectOfType<MusicScript>();
				musicScript.isFadingOut = true;
			}
		}
	}

	public void OnFadeComplete()
	{
		SceneManager.LoadScene(levelToLoad);
	}

	IEnumerator FadeAnyway()
	{
		yield return new WaitForSeconds(5);
		FadeToLevel("MainMenu");
	}
}
