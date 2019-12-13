using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllHighScores : MonoBehaviour {

	public static AllHighScores inst;

	public Transform content_panel;
	public UIHighScoreEntry high_score_entry_prefab;
	public Transform high_scores_list;
	public Transform noScoresText;

	public bool IsShowing { get {
			return content_panel.gameObject.activeInHierarchy;
		} }

	public bool IsInputtingHighScore {
		get {
			UIHighScoreEntry[ ] highScoreEntries = high_scores_list.GetComponentsInChildren<UIHighScoreEntry>();
			for(int i = 0; i < highScoreEntries.Length; i++) {
				UIHighScoreEntry entry = highScoreEntries[i];
				UnityEngine.UI.InputField entryInput = entry.new_entry_input;

				if (entryInput.IsInteractable()
					&& entryInput.isFocused
					&& entryInput.IsActive()) {
					return true;
				}
			}
			return false;
		}
	}

	private void Awake() {
		inst = this;
	}

	public void Show(int overrideIndex = -1, int overrideScore = -1) {
		StartCoroutine( DoShow( overrideIndex, overrideScore ) );
	}

	private IEnumerator DoShow( int overrideIndex = -1, int overrideScore = -1) {

		RefreshHighScores();
		content_panel.gameObject.SetActive( true );

		yield return new WaitForSeconds( 0.1f );

		if (overrideIndex >= 0
			&& overrideScore > 0) {

			UIHighScoreEntry[ ] highScoreEntries = high_scores_list
				.GetComponentsInChildren<UIHighScoreEntry>();

			if (overrideIndex < highScoreEntries.Length) {
				if (highScoreEntries.Length < 10) {
					HighScoreEntry scoreEntry = new HighScoreEntry( "FlappyPlayer", overrideScore );
					AddHighScore( scoreEntry, overrideIndex );
					OverrideScore( overrideIndex, overrideScore );
				} else {
					OverrideScore(
						overrideIndex,
						overrideScore );
				}
			} else {
				HighScoreEntry scoreEntry = new HighScoreEntry( "FlappyPlayer", overrideScore );
				AddHighScore( scoreEntry );
				OverrideScore( overrideIndex, overrideScore );
			}
		}

		noScoresText.gameObject.SetActive( high_scores_list.childCount == 0 );
		yield return null;
	}

	public void Hide() {
		content_panel.gameObject.SetActive( false );
	}

	public void RefreshHighScores() {

		ClearHighScoreList();

		HighScoreEntry[ ] entries = GameController.inst.HighScores;

		noScoresText.gameObject.SetActive(
			entries == null || entries.Length == 0 );

		for (int i = 0; i < entries.Length; i++) {
			UIHighScoreEntry newEntry = AddHighScore( entries[i] );
			newEntry.ShowNumberIndex( i + 1 );
		}
	}

	public void OverrideScore(int index, int score) {
		UIHighScoreEntry[ ] highScoreEntries = high_scores_list
			.GetComponentsInChildren<UIHighScoreEntry>();
		UIHighScoreEntry entry = highScoreEntries[index];

		entry.SetScore( score );
		entry.BeginNewScoreEntry();
	}

	public void ClearHighScoreList() {
		for (int i = 0; i < high_scores_list.childCount; i++) {
			GameObject.Destroy( high_scores_list.GetChild( i ).gameObject );
		}
	}

	public UIHighScoreEntry AddHighScore( HighScoreEntry entry, int forcedSiblingIndex = -1 ) {
		UIHighScoreEntry newUIEntry = GetUIEntry();
		if (newUIEntry != null) {

			newUIEntry.SetUserId( entry.UserID );
			newUIEntry.SetScore( entry.Score );

			newUIEntry.transform.SetParent( high_scores_list, false );
			if(forcedSiblingIndex >= 0) {
				newUIEntry.transform.SetSiblingIndex( forcedSiblingIndex );
			}

			newUIEntry.gameObject.SetActive( true );
			return newUIEntry;
		}
		return null;
	}

	private UIHighScoreEntry GetUIEntry() {
		if (high_score_entry_prefab != null) {
			return UIHighScoreEntry.Instantiate( high_score_entry_prefab );
		}
		return null;
	}
}