using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
	[SerializeField] private Unit unit = default;
    [SerializeField] private Image image = default;

    private int maxHealth;
	public void setUnit(Unit value) { unit = value; }

    void Update()
    {
		if (gameObject.CompareTag("PlayerHealthBar") && GameObject.FindGameObjectWithTag("Player"))
				unit = GameObject.FindGameObjectWithTag("Player").GetComponent<Unit>();
		maxHealth = unit.GetMaxHealth();
		float healthToSet = (float)unit.GetHealth() / maxHealth;
        image.fillAmount = healthToSet;
    }
}
