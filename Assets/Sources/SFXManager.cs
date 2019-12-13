using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SFXManager : MonoBehaviour
{
	public static SFXManager inst;
	public AudioSource[] sfx_sources;

	[Header( "Sounds" )]
	public AudioClip[] sfx_jumps;
	public AudioClip[] sfx_hits;
	public AudioClip sfx_onGameOverShow;
	public AudioClip sfx_addPoint;
	public AudioClip sfx_startGame;

	public void Awake() {
		inst = this;
	}

	public void PlayJumpSound() {
		AudioSource s = GetVacantAudioSource();
		if (s != null) {
			int idx = Random.Range( 0, sfx_jumps.Length );
			if (sfx_jumps[idx] != null) {
				s.PlayOneShot( sfx_jumps[idx] );
			}
		}
	}

	public void PlayHitSound() {
		AudioSource s = GetVacantAudioSource();
		if (s != null) {
			int idx = Random.Range( 0, sfx_hits.Length );
			if(sfx_hits[idx] != null) {
				s.PlayOneShot( sfx_hits[idx] );
			}
		}
	}

	public void PlayGameOverShowSound() {
		AudioSource s = GetVacantAudioSource();
		if (s != null) {
			s.PlayOneShot( sfx_onGameOverShow );
		}
	}

	public void PlayAddPointSound() {
		AudioSource s = GetVacantAudioSource();
		if (s != null) {
			s.PlayOneShot( sfx_addPoint );
		}
	}

	public void PlayStartGameSound() {
		AudioSource s = GetVacantAudioSource();
		if (s != null) {
			s.PlayOneShot( sfx_startGame );
		}
	}

	private AudioSource GetVacantAudioSource() {
		for(int i = 0; i < sfx_sources.Length; i++) {
			AudioSource s = sfx_sources[i];
			if (!s.isPlaying) {
				return s;
			}
		}

		return null;
	}
}
