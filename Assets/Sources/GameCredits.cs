using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

public class GameCredits : MonoBehaviour
{
	public static GameCredits inst;

	public Transform contentPanel;
	public AudioSource ASrc_Theme;

	public bool IsShowing { get {
			return contentPanel.gameObject.activeInHierarchy;
		} }

	private void Awake() {
		inst = this;
	}

	private void Update() {
		if (Input.GetKeyDown( KeyCode.Escape )) {
			Close();
		}
	}

	public void Open() {
		contentPanel.gameObject.SetActive( true );
		ASrc_Theme.Play();
	}

	public void Close() {
		UnityEngine.SceneManagement.SceneManager.LoadScene( 0 );
	}

	public void GoToDiscord() {
		Application.OpenURL( "http://discord.gg/dvCHVZA" );
	}
}
