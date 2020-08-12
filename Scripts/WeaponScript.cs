using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
	[SerializeField] private bool needsReload = default;
	[SerializeField] private float reloadTime = default;
	[SerializeField] private AudioClip beginReload = default;
	[SerializeField] private AudioClip endReload = default;
	[SerializeField] private GameObject castAnim = default;
	[SerializeField] private GameObject projectile = default;
	[SerializeField] private float shootRate = default;
	[SerializeField] private float leftProjXOffset = default;
	[SerializeField] private float leftProjYOffset = default;
	[SerializeField] private float projXOffset = default;
	[SerializeField] private float projYOffset = default;

	public GameObject GetCastAnim() { return castAnim; }
	public GameObject GetProjectile() { return projectile; }
	public bool GetNeedsReload() { return needsReload; }
	public float GetReloadTime() { return reloadTime; }
	public float GetShootRate() { return shootRate; }
	public AudioClip GetBeginReload() { return beginReload; }
	public AudioClip GetEndReload() { return endReload; }
	public float GetLeftProjXOffset() { return leftProjXOffset; }
	public float GetLeftProjYOffset() { return leftProjYOffset; }
	public float GetProjXOffset() { return projXOffset; }
	public float GetProjYOffset() { return projYOffset; }

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
