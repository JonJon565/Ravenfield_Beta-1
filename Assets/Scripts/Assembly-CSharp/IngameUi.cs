using UnityEngine;
using UnityEngine.UI;

public class IngameUi : MonoBehaviour
{
	public static IngameUi instance;

	private Canvas canvas;

	public Text currentAmmo;

	public Text spareAmmo;

	public Text health;

	public Image weapon;

	private void Awake()
	{
		instance = this;
		canvas = GetComponent<Canvas>();
		Hide();
	}

	public void SetAmmoText(int current, int spare)
	{
		currentAmmo.text = ((current == -1) ? string.Empty : current.ToString());
		if (spare >= 0)
		{
			spareAmmo.text = "/" + spare;
			return;
		}
		switch (spare)
		{
		case -1:
			spareAmmo.text = string.Empty;
			break;
		case -2:
			spareAmmo.text = "/∞";
			break;
		}
	}

	public void SetWeapon(Weapon weapon)
	{
		this.weapon.sprite = weapon.uiSprite;
	}

	public void SetHealth(float health)
	{
		this.health.text = Mathf.CeilToInt(health).ToString();
	}

	public void Hide()
	{
		canvas.enabled = false;
	}

	public void Show()
	{
		canvas.enabled = true;
	}
}
