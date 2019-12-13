using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopScoresPreview : MonoBehaviour {

	public UIHighScoreEntry highScoreEntryPrefab;
	public Transform highScoreList;
	public GameObject noScoresText;

	public void RefreshHighScores () {

		ClearHighScoreList();

		HighScoreEntry[ ] entries = GameController.inst.HighScores;

		noScoresText.gameObject.SetActive(
			entries == null || entries.Length == 0 );

		for (int i = 0; i < Mathf.Min(5, entries.Length); i++) {
			AddHighScore( entries[i] );
		}
	}

	public void ClearHighScoreList() {
		for(int i = 0; i < highScoreList.childCount; i++) {
			GameObject.Destroy( highScoreList.GetChild( i ).gameObject );
		}
	}

	public void AddHighScore( HighScoreEntry entry ) {
		UIHighScoreEntry newUIEntry = GetUIEntry();
		if(newUIEntry != null) {

			newUIEntry.SetUserId( entry.UserID );
			newUIEntry.SetScore( entry.Score );

			newUIEntry.transform.SetParent( highScoreList, false );
			newUIEntry.gameObject.SetActive( true );
		}
	}

	private UIHighScoreEntry GetUIEntry() {
		if(highScoreEntryPrefab != null) {
			return UIHighScoreEntry.Instantiate( highScoreEntryPrefab );
		}
		return null;
	}
}
