using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillResponse : MonoBehaviour {

	private struct IntervalStats {
		public Vector3 StartPos;
		public Vector3 LastPos;
		public Vector3 EndPos;
		public int numOfJumps;
	}

	public static SkillResponse inst;

	public SkillMessage skillMessagePrefab;
	public AudioSource ASrc_Streak;
	public AudioSource ASrc_Distance;
	public AudioSource ASrc_Spammer;
	public AudioSource ASrc_HighScorePassed;
	public AudioSource ASrc_FirstPlace;

	private IntervalStats currentInterval;

	public void OnGameStarted() {
		GameController.inst.playerSanta.onPlayerJump.AddListener( OnPlayerJump );
		ResetInterval();
		StartInterval();
	}

	public void OnGameEnded() {
	}

	public void OnPlayerJump() {
		currentInterval.numOfJumps++;
	}

	public void OnPointAdded() {
		EndInterval();
		ResetInterval();
		StartInterval();
	}

	private void ResetInterval() {
		currentInterval = new IntervalStats();
	}

	private void StartInterval() {
		Santa player = GameController.inst.playerSanta;
		if (player != null) {
			currentInterval.StartPos = player.spineRb.transform.position;
		}
	}

	private void EndInterval() {
		Santa player = GameController.inst.playerSanta;
		if(player != null) {
			currentInterval.EndPos = player.spineRb.transform.position;
			CheckIntervalForSkillResponse( currentInterval );
		}
	}

	private void CheckIntervalForSkillResponse( IntervalStats stats ) {
		if(GameController.inst.points > 1) {

			float dist = Vector3.Distance( stats.StartPos, stats.EndPos );
			Vector3 intervalVector = stats.StartPos - stats.EndPos;

			// high scores
			CheckForHighScoreResponse();

			// long stretch
			if (Mathf.Abs(intervalVector.y) > 4.0f) {
				ThrowDistanceResponse();
			}
			// jump spammer
			else if (stats.numOfJumps > 5) {
				ThrowJumpSpammerResponse();
			}else if(GameController.inst.points % 10 == 0) {

				string m = "";
				switch (GameController.inst.points) {
					case 20:
						m = "BABY STEPS";
						break;
					case 40:
						m = "THE BIG 4-0!";
						break;
					case 50:
						m = "YUUGEE 50!";
						break;
					case 60:
						m = "WOAH! 60!";
						break;
					case 100:
						m = "YOU'RE INSANE! 100!";
						break;
					case 110:
						m = "NO WAY!";
						break;
					case 120:
						m = "YOU'RE GONNA BREAK IT!";
						break;
					case 150:
						m = "WHO IS THIS!?";
						break;
					case 200:
						m = "YOU ALREADY WON. STOP.";
						break;
					case 210:
						m = "I'M NOT COUNTING ANYMORE";
						break;
				}

				if (!string.IsNullOrEmpty( m )) {
					string anim = Random.Range( 0, 100 ) > 50
						? "Anim_SR_FlyIn"
						: "Anim_SR_FadeIn";

					SpawnMessage( m, anim );
					ASrc_Streak.Play();
				} else {
					ThrowStreakResponse();
				}
			}
		}
	}

	private void CheckForHighScoreResponse() {
		HighScoreEntry[] highScores = GameController.inst.HighScores;
		int points = GameController.inst.points;

		for(int i = highScores.Length - 1; i >= 0; i--) {
			if(points == highScores[i].Score + 1) {
				if(i == 0) {
					ThrowFirstPlaceResponse();
				} else {
					//ThrowNewHighScoreResponse();
				}
			}
		}
	}

	private SkillMessage SpawnMessage(string message, string anim) {
		SkillMessage newMessage = SkillMessage.Instantiate( skillMessagePrefab );
		newMessage.transform.SetParent( this.transform, false );

		Vector3 playerPos = GameController.inst.playerSanta.spineRb.transform.position;
		Vector3 spawnPos = Vector3.zero;
		Vector3 viewPos;

		spawnPos.y += Random.Range( -1.0f, 0.7f );
		if (playerPos.y > 1.5f) {
			spawnPos.y -= 3.0f;
		} else {
			spawnPos.y += 3.0f;
		}

		viewPos = Camera.main.WorldToViewportPoint( spawnPos );
		newMessage.rect.anchorMin = viewPos;
		newMessage.rect.anchorMax = viewPos;

		newMessage.SetMessage( message );
		newMessage.Play( anim );
		newMessage.gameObject.SetActive( true );

		return newMessage;
	}

	public string[ ] longDistanceMessages;
	private void ThrowDistanceResponse() {
		string m = longDistanceMessages[Random.Range( 0, longDistanceMessages.Length )];
		SpawnMessage( m, "Anim_SR_FlyIn" );
		ASrc_Distance.Play();
	}

	public string[ ] jumpSpamMessages;
	private void ThrowJumpSpammerResponse() {
		string m = jumpSpamMessages[Random.Range( 0, jumpSpamMessages.Length )];

		string anim = Random.Range( 0, 100 ) > 50
			? "Anim_SR_FlyIn"
			: "Anim_SR_FadeIn";

		SpawnMessage( m, anim );
		ASrc_Spammer.Play();
	}

	public string[ ] streakMessages;
	private void ThrowStreakResponse() {
		string m = streakMessages[Random.Range( 0, streakMessages.Length )];
		SpawnMessage( m, "Anim_SR_Default" );
		ASrc_Streak.Play();
	}

	private void ThrowNewHighScoreResponse() {
		string m = "NEW HIGH SCORE!";
		SpawnMessage( m, "Anim_SR_Default" );
		ASrc_HighScorePassed.Play();
	}

	private  void ThrowFirstPlaceResponse() {
		string m = "1ST PLACE!!!";
		SpawnMessage( m, "Anim_SR_FlyIn" );
		ASrc_FirstPlace.Play();
	}
}