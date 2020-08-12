using System;
using UnityEngine;

public class RangedEnemyScript : MonoBehaviour
{
    private Unit unit;
    private Transform target;
    private Transform player;
    private ProjectileController pjCont;
    private RaycastHit hit;
    private Rigidbody rb;

	[SerializeField] private Animator animator = default;
    [SerializeField] private float speed = default;
	[SerializeField] private float safeDistance = default;

	void Start()
    {
		if (GameObject.FindGameObjectWithTag("Player"))
		{
			player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
		}
		pjCont = gameObject.GetComponent<ProjectileController>();
        rb = gameObject.GetComponent<Rigidbody>();
        unit = gameObject.GetComponent<Unit>();
        animator.SetBool("isXFlipped", false);
        target = gameObject.GetComponent<Unit>().GetClosestEnemy(gameObject);

	}

    void FixedUpdate()
    {
		//use layer mask to ignore collisions with anything that isn't its own target
        ConfirmRangedTarget();
		if ((checkLineOfSight() == "left" ) || (checkLineOfSight() == "right"))
        {
            if (!hit.transform.GetComponent<Unit>().IsDead())
            {
				bool fireToRight = checkLineOfSight() == "right" ? true : false;
                pjCont.FireProjectile(fireToRight);
                animator.SetBool("isInLineOfSight", true);
            }
        }
        else
        {
            animator.SetBool("isInLineOfSight", false);
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerProjectile") && gameObject.CompareTag("Enemies"))
		{
            unit.HandleDamage(other);
        }

        else if (other.CompareTag("EnemyProjectile") && gameObject.CompareTag("Friendlies"))
        {
            unit.HandleDamage(other);
        }
    }

    public void ConfirmRangedTarget()
    {
        if (target)
        {
            if (target.GetComponent<Unit>().IsDead())
            {
                target = gameObject.GetComponent<Unit>().GetClosestEnemy(gameObject);
            }
            else if (!unit.IsDead())
            {
                DistanceTarget();
            }

        }
        else
        {
            target = gameObject.GetComponent<Unit>().GetClosestEnemy(gameObject);
        }
    }

    public void DistanceTarget()
	{

		float safeBufferMin = safeDistance - 0.1f;
		float safeBufferMax = safeDistance + 0.1f;
		Vector3 targetCenter = target.GetComponentInChildren<Collider>().bounds.center;
		Vector3 goCenter = gameObject.GetComponentInChildren<Collider>().bounds.center;
		float distance = Vector3.Distance(transform.position, target.position);

		if (target)
		{

			//if (target)
			//	Debug.Log(gameObject.name + " is targeting " + target.name);
			//else
			//	Debug.Log(gameObject.name + " has no target");

			//if an enemy, and within safe distance, attempt to distance self
			if (distance <= safeBufferMin)
			{
				Vector3 dir = -1 * (target.position - transform.position);
				Vector3 movement = dir.normalized * speed * 30.0f * Time.deltaTime;
				rb.velocity = movement;
				animator.SetBool("isRunningAway", true);
			}

			//if at safe distance, move along y
			else if ((distance > safeBufferMin) && (distance < safeBufferMax) && target && !target.GetComponent<Unit>().IsDead())
			{
				Vector3 dir = (target.position - transform.position);
				dir.x = 0;
				if (Math.Abs(target.position.y - transform.position.y) <= 0.002f)
				{
					dir.y = 0;
					dir.z = 0;
				}
					
				Vector3 movement = dir.normalized * speed * 30.0f * Time.deltaTime;
				rb.velocity = movement;
				animator.SetBool("isRunningAway", true);
			}

			//if beyond safe distance or no target/dead target, move closer (in the case of friendlies, to player)
			else if (Vector3.Distance(transform.position, target.position) >= safeBufferMax || !target || target.GetComponent<Unit>().IsDead())
			{
				GameObject confirmedTarget = gameObject.CompareTag("Friendlies") ? target.gameObject : GameObject.FindGameObjectWithTag("Player");
				if (!target || target.GetComponent<Unit>().IsDead())
				{
					confirmedTarget = GameObject.FindGameObjectWithTag("Player");
				}
				Vector3 dir = (confirmedTarget.transform.position - transform.position);
				Vector3 movement = dir.normalized * speed * 30.0f * Time.deltaTime;
				rb.velocity = movement;
			}

			else
			{
				rb.velocity = Vector3.zero;
			}

			//animation
			if (transform.position.x > target.position.x)
            {
                animator.SetBool("isToLeftOfTarget", true);
                animator.SetBool("isXFlipped", true);
            }
            else
            {
                animator.SetBool("isToLeftOfTarget", false);
                animator.SetBool("isXFlipped", false);
            }
        }



    }
    private string checkLineOfSight()
    {
        Ray left = new Ray(gameObject.GetComponentInChildren<Collider>().bounds.center, Vector3.left);
        Ray right = new Ray(gameObject.GetComponentInChildren<Collider>().bounds.center, Vector3.right);
		LayerMask layer;
		if (gameObject.CompareTag("Enemies"))
			layer = LayerMask.GetMask("Friendlies");
		else
			layer = LayerMask.GetMask("Enemies");

		foreach (GameObject go in unit.GetAllEnemies(gameObject))
		{
			if (Physics.Raycast(left, out hit, Mathf.Infinity, layer) && (hit.transform == go.transform))
				return "left";
			else if (Physics.Raycast(right, out hit, Mathf.Infinity, layer) && (hit.transform == go.transform))
				return "right";
		}
        return "none";
    }
}
