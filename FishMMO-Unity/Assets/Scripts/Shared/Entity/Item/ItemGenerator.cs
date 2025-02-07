﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;

namespace FishMMO.Shared
{
	public class ItemGenerator : BaseRNGenerator
	{
		private Dictionary<string, ItemAttribute> attributes = new Dictionary<string, ItemAttribute>();

		private Item item;
		public event Action<ItemAttribute, int, int> OnSetAttribute;

		public void Initialize(Item item, int seed)
		{
			this.item = item;
			Seed = seed;

			if (item.Equippable != null)
			{
				item.Equippable.OnEquip += ItemEquippable_OnEquip;
				item.Equippable.OnUnequip += ItemEquippable_OnUnequip;
			}
		}

		public void Destroy()
		{
			if (item.Equippable != null)
			{
				item.Equippable.OnEquip -= ItemEquippable_OnEquip;
				item.Equippable.OnUnequip -= ItemEquippable_OnUnequip;
			}
			item = null;
		}

		public string Tooltip()
		{
			return Tooltip(new StringBuilder());
		}

		public string Tooltip(StringBuilder sb)
		{
			sb.Append("<color=#a66ef5>Seed: ");
			sb.Append(Seed);
			sb.Append("</color>");
			foreach (ItemAttribute attribute in attributes.Values)
			{
				sb.Append("<size=110%>");
				sb.Append(attribute.Template.Name);
				sb.Append(": <color=#32a879>");
				sb.Append(attribute.value);
				sb.Append("</color></size>");
				sb.AppendLine();
			}
			return sb.ToString();
		}

		public override void Generate(int seed)
		{
			this.seed = seed;

			System.Random random = new System.Random(seed);
			if (random != null)
			{
				if (attributes != null)
				{
					attributes.Clear();
					attributes = new Dictionary<string, ItemAttribute>();

					EquippableItemTemplate Equippable = item.Template as EquippableItemTemplate;
					if (Equippable == null)
						return;

					WeaponTemplate weapon = item.Template as WeaponTemplate;
					if (weapon != null)
					{
						attributes.Add(weapon.AttackPower.Name, new ItemAttribute(weapon.AttackPower.ID, random.Next(weapon.AttackPower.MinValue, weapon.AttackPower.MaxValue)));
						attributes.Add(weapon.AttackSpeed.Name, new ItemAttribute(weapon.AttackSpeed.ID, random.Next(weapon.AttackSpeed.MinValue, weapon.AttackSpeed.MaxValue)));
					}
					else
					{
						ArmorTemplate armor = item.Template as ArmorTemplate;
						if (armor != null)
						{
							attributes.Add(armor.ArmorBonus.Name, new ItemAttribute(armor.ArmorBonus.ID, random.Next(armor.ArmorBonus.MinValue, armor.ArmorBonus.MaxValue)));
						}
					}

					if (Equippable.AttributeDatabases != null && Equippable.AttributeDatabases.Length > 0)
					{
						int attributeCount = random.Next(0, Equippable.MaxItemAttributes);
						for (int i = 0, rng; i < attributeCount; ++i)
						{
							rng = random.Next(0, Equippable.AttributeDatabases.Length);
							ItemAttributeTemplateDatabase db = Equippable.AttributeDatabases[rng];
							rng = random.Next(0, db.Attributes.Count);
							ItemAttributeTemplate attributeTemplate = Enumerable.ToList(db.Attributes.Values)[rng];
							attributes.Add(attributeTemplate.Name, new ItemAttribute(attributeTemplate.ID, random.Next(attributeTemplate.MinValue, attributeTemplate.MaxValue)));
						}
					}
				}
			}
		}

		public ItemAttribute GetAttribute(string name)
		{
			attributes.TryGetValue(name, out ItemAttribute attribute);
			return attribute;
		}

		public void SetAttribute(string name, int newValue)
		{
			if (attributes.TryGetValue(name, out ItemAttribute attribute))
			{
				if (attribute.value == newValue) return;

				int oldValue = attribute.value;
				attribute.value = newValue;

				OnSetAttribute?.Invoke(attribute, oldValue, newValue);
			}
		}

		public void ItemEquippable_OnEquip(Character character)
		{
			foreach (KeyValuePair<string, ItemAttribute> pair in attributes)
			{
				if (character.AttributeController.TryGetAttribute(pair.Value.Template.CharacterAttribute.ID, out CharacterAttribute characterAttribute))
				{
					characterAttribute.AddValue(pair.Value.value);
				}
			}
		}

		public void ItemEquippable_OnUnequip(Character character)
		{
			foreach (KeyValuePair<string, ItemAttribute> pair in attributes)
			{
				if (character.AttributeController.TryGetAttribute(pair.Value.Template.CharacterAttribute.ID, out CharacterAttribute characterAttribute))
				{
					characterAttribute.AddValue(-pair.Value.value);
				}
			}
		}
	}
}