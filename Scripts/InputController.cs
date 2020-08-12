using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
	[SerializeField] public KeyCode MoveUp { get; set; } = KeyCode.W;
	[SerializeField] public KeyCode MoveDown { get; set; } = KeyCode.S;
	[SerializeField] public KeyCode MoveLeft { get; set; } = KeyCode.A;
	[SerializeField] public KeyCode MoveRight { get; set; } = KeyCode.D;
	[SerializeField] public KeyCode ShootLeft { get; set; } = KeyCode.LeftArrow;
	[SerializeField] public KeyCode ShootRight { get; set; } = KeyCode.RightArrow;
	[SerializeField] public KeyCode SwapWeapon { get; set; } = KeyCode.UpArrow;
	[SerializeField] public KeyCode Resurrect { get; set; } = KeyCode.R;
	[SerializeField] public KeyCode Heal { get; set; } = KeyCode.F;
	[SerializeField] public KeyCode Interact { get; set; } = KeyCode.Space;
	[SerializeField] public KeyCode ToggleMusic { get; set; } = KeyCode.M;

	private List<KeyCode> keyList;

	void Start()
    {
		keyList = new List<KeyCode>();
		keyList.AddRange(new KeyCode[] { MoveUp, MoveDown, MoveLeft, MoveRight, ShootLeft, ShootRight, SwapWeapon, Resurrect, Heal, Interact});
	}

	// Update is called once per frame
	void Update()
    {
        
    }
}
