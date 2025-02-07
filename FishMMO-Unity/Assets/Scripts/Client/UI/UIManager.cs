﻿using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace FishMMO.Client
{
	/// <summary>
	/// Helper class for our UI
	/// </summary>
	public static class UIManager
	{
		private static Dictionary<string, UIControl> controls = new Dictionary<string, UIControl>();
		private static Client _client;

		/// <summary>
		/// Dependency injection for the Client.
		/// </summary>
		internal static void SetClient(Client client)
		{
			_client = client;
		}

		internal static void Register(UIControl control)
		{
			if (control == null)
			{
				return;
			}
			if (controls.ContainsKey(control.Name))
			{
				return;
			}

			control.SetClient(_client);

			UnityEngine.Debug.Log("UIManager Registered[" + control.Name + "]");
			controls.Add(control.Name, control);
		}

		internal static void Unregister(UIControl control)
		{
			if (control == null)
			{
				return;
			}
			else
			{
				//Debug.Log("UIManager: Unregistered " + control.Name);
				controls.Remove(control.Name);
			}
		}

		public static bool TryGet<T>(string name, out T control) where T : UIControl
		{
			if (controls.TryGetValue(name, out UIControl result))
			{
				if ((control = result as T) != null)
				{
					return true;
				}
			}
			control = null;
			return false;
		}

		public static bool Exists(string name)
		{
			if (controls.ContainsKey(name))
			{
				return true;
			}
			return false;
		}

		public static void Show(string name)
		{
			if (controls.TryGetValue(name, out UIControl result))
			{
				result.OnShow();
			}
		}

		public static void Hide(string name)
		{
			if (controls.TryGetValue(name, out UIControl result) && result.Visible)
			{
				result.OnHide();
			}
		}

		public static void HideAll()
		{
			foreach (KeyValuePair<string, UIControl> p in controls)
			{
				p.Value.OnHide();
			}
		}

		public static void ShowAll()
		{
			foreach (KeyValuePair<string, UIControl> p in controls)
			{
				p.Value.OnShow();
			}
		}

		public static bool ControlHasFocus()
		{
			if (EventSystem.current.currentSelectedGameObject != null)
			{
				return true;
			}
			foreach (UIControl control in controls.Values)
			{
				if (control.Visible && control.HasFocus)
				{
					return true;
				}
			}
			return false;
		}

		public static bool InputControlHasFocus()
		{
			foreach (UIControl control in controls.Values)
			{
				if (control.Visible && control.InputField != null && control.InputField.isFocused)
				{
					return true;
				}
			}
			return false;
		}
	}
}