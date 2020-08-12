using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieScript : MonoBehaviour
{
    private Unit unit;
    private Transform target;
    private Transform player;
    private ProjectileController pjCont;

    [SerializeField] private Animator animator = default;
    [SerializeField] private float speed = default;
    [SerializeField] private float stoppingDistance = default;

    void Start()
    {
        pjCont = gameObject.GetComponent<ProjectileController>();
        unit = gameObject.GetComponent<Unit>();
		if (GameObject.FindGameObjectWithTag("Player"))
		{
			player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
		}
        animator.SetBool("isXFlipped", false);

		//ignore short bounds
		Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), GameObject.Find("ShortUpperBounds").GetComponent<Collider>(), true);

	}

    void Update()
    {
        ConfirmMeleeTarget(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        //if( other is IHaveHealth)
        //{
        //    var hh = other as IHaveHealth;

        //}

        if (other.CompareTag("PlayerProjectile") && gameObject.CompareTag("Enemies"))
		{
            unit.HandleDamage(other);
        }
        else if (other.CompareTag("EnemyProjectile") && gameObject.CompareTag("Friendlies"))
		{
            unit.HandleDamage(other);
        }
    }

    public void ConfirmMeleeTarget(GameObject thisUnit)
    {
        if (!target)
			target = gameObject.GetComponent<Unit>().GetClosestEnemy(thisUnit);

		else if (target.GetComponent<Unit>().IsDead())
		{
			target = gameObject.GetComponent<Unit>().GetClosestEnemy(thisUnit);
		}
		else if (!unit.IsDead())
		{
			ChaseAndMeleeTarget();
		}
		else return;
    }

    public void ChaseAndMeleeTarget()
    {
		if (target)
		{
			var targetIsToTheLeft = transform.position.x > target.position.x;

			 if (Vector3.Distance(transform.position, target.position) <= stoppingDistance)
			{
				if (!target.GetComponent<Unit>().IsDead() || target)
				{
					pjCont.FireProjectile(isFiringRight: !targetIsToTheLeft);
					animator.SetBool("isMelee", true);
				}
			}
			else if (Vector3.Distance(transform.position, target.position) > stoppingDistance)
			{
				transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
				animator.SetBool("isMelee", false);
			}

			animator.SetBool("isToLeftOfTarget", targetIsToTheLeft);
			animator.SetBool("isXFlipped", targetIsToTheLeft);
		}
    }



}
