using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class UIHighScoreEntry : MonoBehaviour {

	public Text txt_user_id;
	public Text txt_score;
	public Text txt_numberIndex;
	public InputField new_entry_input;

	private bool isEnteringNewScore;

	public void SetUserId(string userId ) {
		txt_user_id.text = userId;
	}

	public void SetScore(int score ) {
		txt_score.text = score.ToString();
	}

	public void ShowNumberIndex(int idx ) {
		txt_numberIndex.text = string.Format( "[{0}] ", idx.ToString() );
		txt_numberIndex.gameObject.SetActive( true );
	}

	[ContextMenu("Begin New Score Entry")]
	public void BeginNewScoreEntry() {

		isEnteringNewScore = true;
		new_entry_input.gameObject.SetActive( true );
		txt_user_id.gameObject.SetActive( false );

		new_entry_input.onEndEdit.RemoveAllListeners();
		new_entry_input.onEndEdit.AddListener( ( string str ) => {

			if (string.IsNullOrEmpty( str )) {
				BeginNewScoreEntry();
				return;
			}

			SetUserId( str );
			new_entry_input.interactable = false;
			new_entry_input.gameObject.SetActive( false );
			txt_user_id.gameObject.SetActive( true );

			int score;
			if(int.TryParse(txt_score.text, out score )) {
				GameController.inst.TrySubmitHiScore( txt_user_id.text, score );
				GameController.inst.TryUpdateHighScores();
			}

			AllHighScores.inst.Hide();
		} );

		new_entry_input.interactable = true;
		StartCoroutine( ActivateAndSelectInputField() );
	}

	private IEnumerator ActivateAndSelectInputField() {

		yield return new WaitForSeconds( 0.1f );

		new_entry_input.Select();
		new_entry_input.ActivateInputField();
		EventSystem.current.SetSelectedGameObject( new_entry_input.gameObject, null );
		new_entry_input.OnPointerClick( new PointerEventData( EventSystem.current ) );
	}
}