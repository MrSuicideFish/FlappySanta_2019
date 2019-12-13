using System;
using System.Text;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public struct HighScoreEntry {
	public string UserID { get; private set; }
	public int Score { get; private set; }

	public HighScoreEntry(string userId, int score) {
		UserID = userId;
		Score = score;
	}

	public override string ToString() {
		return UserID + " : " + Score.ToString();
	}
}

public class GameController : MonoBehaviour {

	private const string URI_INSERT_HIGH_SCORE = "LINK";
	private const string URI_GET_HIGH_SCORES = "LINK";

	// game instance
	public static GameController inst;

	// gameplay state
	public bool gameHasStarted { get; private set; }
	public bool gameHasEnded { get; private set; }
	public int points { get; private set; }

	// player instance
	public Santa playerSanta;

	// obstacles
	public ObstacleScroller obstacleScroller;

	public Transform PreGameUI;
	public Transform EndGameUI;
	public UnityEngine.UI.Button restartGameButton;

	public UnityEvent onGameStart;
	public UnityEvent onGameEnd;
	public UnityEvent onHighScoresUpdated;
	public UnityEvent onPointAdded;

	private void Awake() {
		inst = this;
	}

	private void Start() {
		TryUpdateHighScores();
		TogglePreGameUI( true );
		ToggleEndGameUI( false );
	}

	private void Update() {

		if (!gameHasStarted) {

			if (!AllHighScores.inst.IsShowing) {

				if (!GameCredits.inst.IsShowing) {

					if (Input.GetKeyDown( KeyCode.Space )
					|| Input.GetMouseButtonDown( 0 )) {
						StartGame();
					}

					if (Input.GetKeyDown( KeyCode.C )) {
						ShowCredits();
						return;
					}
				}
			}
		}

		if (Input.GetKeyDown( KeyCode.Tab )) {
			if (!AllHighScores.inst.IsShowing) {
				AllHighScores.inst.Show();
			}
		} else if (Input.GetKeyUp( KeyCode.Tab )) {
			if (!AllHighScores.inst.IsInputtingHighScore) {
				AllHighScores.inst.Hide();
			}
		}
	}

	public void StartGame() {
		if (!gameHasStarted) {

			SFXManager.inst.PlayStartGameSound();

			// start player flapping/control
			playerSanta.ToggleControl( true );
			playerSanta.ToggleRigidBodies( true );

			// stage ui
			TogglePreGameUI( false );
			ToggleEndGameUI( false );

			// begin obstacles
			obstacleScroller.Play();

			// switch mode
			gameHasStarted = true;
			gameHasEnded = false;

			if (onGameStart != null) {
				onGameStart.Invoke();
			}
		}
	}

	public void EndGame() {
		if (!gameHasEnded) {

			this.TryUpdateHighScores();

			// stop player flapping/control
			playerSanta.ToggleControl( false );
			playerSanta.ToggleRigidBodies( true );

			// end obstacles
			obstacleScroller.Pause();

			StartCoroutine( ShowEndGameAfterTime( 1.0f ) );

			// switch mode
			gameHasStarted = true;
			gameHasEnded = true;

			if (onGameEnd != null) {
				onGameEnd.Invoke();
			}
		}
	}

	public void AddPoint() {
		points++;
		SFXManager.inst.PlayAddPointSound();
		if(onPointAdded != null) {
			onPointAdded.Invoke();
		}
	}

	private IEnumerator ShowEndGameAfterTime( float t ) {

		float _t = t;
		Vector3 playerVelocity = playerSanta.spineRb.velocity;
		while (_t > 0.0f && playerVelocity.y != 0) {
			playerVelocity = playerSanta.spineRb.velocity;
			_t -= Time.deltaTime;
			yield return null;
		}

		SFXManager.inst.PlayGameOverShowSound();

		// stage ui
		TogglePreGameUI( false );
		ToggleEndGameUI( true );

		int leaderboardIdx;
		if(IsScoreHighScore( points, out leaderboardIdx )) {

			restartGameButton.interactable = false;
			yield return new WaitForSeconds( 0.5f );
			AllHighScores.inst.Show( leaderboardIdx, points );

			restartGameButton.interactable = true;
			//while (AllHighScores.inst.IsInputtingHighScore) {
			//	yield return null;
			//}
		}

		while (true) {
			if (!AllHighScores.inst.IsShowing) {
				if (Input.GetKeyDown( KeyCode.Space )) {
					RestartGame();
				}
			}
			yield return null;
		}
	}

	public void RestartGame() {
		StopAllCoroutines();
		SceneManager.LoadScene( 0 );
	}

	private void TogglePreGameUI( bool active ) {
		PreGameUI.gameObject.SetActive( active );
	}

	private void ToggleEndGameUI( bool active ) {
		EndGameUI.gameObject.SetActive( active );
	}

	private void ShowCredits() {
		GameCredits.inst.Open();
	}

	#region HI SCORES
	public HighScoreEntry[ ] HighScores { get; private set; }

	public void TryUpdateHighScores() {
		StartCoroutine( UpdateHighScores() );
	}

	private IEnumerator UpdateHighScores() {

		using (UnityWebRequest www = UnityWebRequest.Get( URI_GET_HIGH_SCORES )) {

			yield return www.SendWebRequest();

			if (www.isNetworkError || www.isHttpError) {
				Debug.Log( www.error );
				yield break;
			}

			string response = www.downloadHandler.text;
			string[ ] responseLines;

			responseLines = response.Split(
				new string[ ] { "\r\n", "\r", "\n" },
				StringSplitOptions.None );

			int lineCount = responseLines.Length - 1;
			List<HighScoreEntry> newHighScores = new List<HighScoreEntry>();

			for(int i = 0; i < lineCount; i++) {

				string line = responseLines[i];
				string[ ] lineData = line.Split( ' ' );
				string userId = lineData[0];
				int score;

				if (!string.IsNullOrEmpty( userId )) {

					if (int.TryParse( lineData[1], out score )) {

						HighScoreEntry highScore = new HighScoreEntry(
							userId: userId,
							score: score );

						newHighScores.Add( highScore );
					}
				}
			}

			// sort high scores
			newHighScores.Sort( ( x, y ) => {
				return -x.Score.CompareTo( y.Score );
			} );
			HighScores = newHighScores.ToArray();

			if (onHighScoresUpdated != null) {
				onHighScoresUpdated.Invoke();
			}
		}
	}

	public bool TrySubmitHiScore(
	string name = "TEST_USER",
	int score = 32 ) {
		if(string.IsNullOrEmpty(name) || name.Contains(' ' )) {
			return false;
		}

		//StartCoroutine( SubmitHighScore( name, score ) );
		return true;
	}

	private IEnumerator SubmitHighScore( string name, int score ) {

		WWWForm form = new WWWForm();
		form.AddField( "user_id", name );
		form.AddField( "score", score );

		using (UnityWebRequest www = UnityWebRequest.Post( URI_INSERT_HIGH_SCORE, form )) {

			yield return www.SendWebRequest();

			if (www.isNetworkError || www.isHttpError) {
				Debug.Log( www.error );
			} else {
				Debug.Log( "Success" );
			}
		}
	}

	public bool IsScoreHighScore(int score, out int index) {
		if(score <= 0
			|| HighScores == null) {
			index = -1;
			return false;
		}

		index = 0;
		for(;index < 10; index++) {
			if(index < HighScores.Length) {
				if(HighScores[index].Score < score) {
					return true;
				}
			} else {
				return true;
			}
		}
		
		return false;
	}
	#endregion
}