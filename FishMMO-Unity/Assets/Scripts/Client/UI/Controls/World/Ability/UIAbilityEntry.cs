﻿using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using FishMMO.Shared;

namespace FishMMO.Client
{
	public class UIAbilityEntry : Button
	{
		[SerializeField]
		public Image Icon;

		public Action<int> OnLeftClick;
		public Action<int> OnRightClick;

		public int Index { get; private set; }
		public ITooltip Tooltip { get; private set; }
		public Character Character { get; private set; }

		protected override void OnDestroy()
		{
			base.OnDestroy();

			Clear();
		}

		public void Initialize(Character character, int index, ITooltip tooltip)
		{
			Character = character;
			Index = index;
			Tooltip = tooltip;
		}

		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);

			if (Character != null)
			{
				if (Tooltip != null &&
					UIManager.TryGet("UITooltip", out UITooltip tooltip))
				{
					tooltip.SetText(Tooltip.Tooltip(), true);
				}
			}
		}

		public override void OnPointerExit(PointerEventData eventData)
		{
			base.OnPointerExit(eventData);

			if (UIManager.TryGet("UITooltip", out UITooltip tooltip))
			{
				tooltip.OnHide();
			}
		}

		public override void OnPointerClick(PointerEventData eventData)
		{
			base.OnPointerClick(eventData);

			if (eventData.button == PointerEventData.InputButton.Left)
			{
				OnLeftClick?.Invoke(Index);
			}
			else if (eventData.button == PointerEventData.InputButton.Right)
			{
				OnRightClick?.Invoke(Index);
				Clear();
			}
		}

		public virtual void Clear()
		{
			Character = null;
			Index = -1;
			Tooltip = null;
			if (Icon != null) Icon.sprite = null;
		}
	}
}
