using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
	private bool isFacingRight;
	private GameObject parent;
	private Vector3 velocity;

	[SerializeField] private float projSpeed = default;
	[SerializeField] private float fallfactor = default;
	[SerializeField] private float projDecay = default;
	[SerializeField] private int damage = default;
	[SerializeField] private bool falls = default;

	public int GetDamage() { return damage; }
	public GameObject GetParent() { return parent; }
	public void SetParent(GameObject value) { parent = value; }


	public void FaceAndExpire(bool isFacingRight)
    {
        this.isFacingRight = isFacingRight;
        Destroy(this.gameObject, projDecay);
    }

    void Update()
    {
		velocity = transform.GetComponent<Rigidbody>().velocity;
        if (isFacingRight)
        {
			velocity = transform.right * projSpeed * 20;
			if (falls)
				velocity.y = -1 * projSpeed * fallfactor;
			transform.GetComponent<Rigidbody>().velocity = velocity;
		}
        else
        {
			velocity = -1.0f * transform.right * projSpeed * 20;
			if (falls)
				velocity.y = -1 * projSpeed * fallfactor;
			transform.GetComponent<Rigidbody>().velocity = velocity;
		}
    }

}
