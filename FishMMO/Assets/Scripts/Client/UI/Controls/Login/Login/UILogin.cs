﻿using FishNet.Transporting;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.UI;

namespace FishMMO.Client
{
	public class UILogin : UIControl
	{
		public TMP_InputField username;
		public TMP_InputField password;
		public Button signInButton;
		public TMP_Text handshakeMSG;

		public override void OnStarting()
		{
			Client.NetworkManager.ClientManager.OnClientConnectionState += ClientManager_OnClientConnectionState;
			Client.LoginAuthenticator.OnClientAuthenticationResult += Authenticator_OnClientAuthenticationResult;
			Client.OnReconnectFailed += ClientManager_OnReconnectFailed;
		}


		public override void OnDestroying()
		{
			Client.NetworkManager.ClientManager.OnClientConnectionState -= ClientManager_OnClientConnectionState;

			Client.LoginAuthenticator.OnClientAuthenticationResult -= Authenticator_OnClientAuthenticationResult;

			Client.OnReconnectFailed -= ClientManager_OnReconnectFailed;
		}

		private void ClientManager_OnClientConnectionState(ClientConnectionStateArgs obj)
		{
			//handshakeMSG.text = obj.ConnectionState.ToString();
		}

		private void ClientManager_OnReconnectFailed()
		{
			visible = true;
			SetSignInLocked(false);
		}

		private void Authenticator_OnClientAuthenticationResult(ClientAuthenticationResult result)
		{
			switch (result)
			{
				case ClientAuthenticationResult.InvalidUsernameOrPassword:
					// update the handshake message
					handshakeMSG.text = "Invalid Username or Password.";
					Client.ForceDisconnect();
					SetSignInLocked(false);
					break;
				case ClientAuthenticationResult.AlreadyOnline:
					handshakeMSG.text = "Account is already online.";
					Client.ForceDisconnect();
					SetSignInLocked(false);
					break;
				case ClientAuthenticationResult.Banned:
					// update the handshake message
					handshakeMSG.text = "Account is banned. Please contact the system administrator.";
					Client.ForceDisconnect();
					SetSignInLocked(false);
					break;
				case ClientAuthenticationResult.LoginSuccess:
					// reset handshake message and hide the panel
					handshakeMSG.text = "";
					visible = false;

					// request the character list
					CharacterRequestListBroadcast requestCharacterList = new CharacterRequestListBroadcast();
					Client.NetworkManager.ClientManager.Broadcast(requestCharacterList);
					break;
				case ClientAuthenticationResult.WorldLoginSuccess:
					break;
				case ClientAuthenticationResult.ServerFull:
					break;
				default:
					break;
			}
			SetSignInLocked(false);
		}

		public void OnClick_Login()
		{
			if (Client.IsConnectionReady(LocalConnectionState.Stopped) &&
				Client.LoginAuthenticator.IsAllowedUsername(username.text) &&
				Client.LoginAuthenticator.IsAllowedPassword(password.text))
			{
				// set username and password in the authenticator
				Client.LoginAuthenticator.SetLoginCredentials(username.text, password.text);

				handshakeMSG.text = "";

				if (Client.TryGetRandomLoginServerAddress(out ServerAddress serverAddress))
				{
					Client.ConnectToServer(serverAddress.address, serverAddress.port);

					SetSignInLocked(true);
				}
				else
				{
					handshakeMSG.text = "Failed to get a login server!";
				}
			}
		}

		public void OnClick_Quit()
		{
#if UNITY_EDITOR
			EditorApplication.ExitPlaymode();
#else
			Application.Quit();
#endif
		}

		/// <summary>
		/// Sets locked state for signing in.
		/// </summary>
		public void SetSignInLocked(bool locked)
		{
			signInButton.interactable = !locked;
			username.enabled = !locked;
			password.enabled = !locked;
		}
	}
}