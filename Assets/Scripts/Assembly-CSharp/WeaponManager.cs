using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
	public enum WeaponSlot
	{
		Primary = 0,
		Secondary = 1,
		Gear = 2,
		LargeGear = 3
	}

	[Serializable]
	public class WeaponEntry
	{
		public string name = "Weapon";

		public Sprite image;

		public GameObject prefab;

		public WeaponSlot slot;
	}

	public class LoadoutSet
	{
		public WeaponEntry primary;

		public WeaponEntry secondary;

		public WeaponEntry gear1;

		public WeaponEntry gear2;

		public WeaponEntry gear3;

		public LoadoutSet()
		{
			primary = null;
			secondary = null;
			gear1 = null;
			gear2 = null;
			gear3 = null;
		}
	}

	public static WeaponManager instance;

	public List<WeaponEntry> weapons;

	private void Awake()
	{
		instance = this;
	}

	public static List<WeaponEntry> GetWeaponEntriesOfSlot(WeaponSlot slot)
	{
		List<WeaponEntry> list = new List<WeaponEntry>();
		foreach (WeaponEntry weapon in instance.weapons)
		{
			if (weapon.slot == slot)
			{
				list.Add(weapon);
			}
		}
		return list;
	}

	public static WeaponEntry EntryNamed(string name)
	{
		return instance.weapons.Find((WeaponEntry obj) => obj.name == name);
	}
}
