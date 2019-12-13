using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class TextureScroller : MonoBehaviour{

	public float scrollSpeed;

	private MeshRenderer m_Renderer;
	private Vector2 offset;
	private bool isPlaying;

	private void Start() {
		m_Renderer = this.GetComponent<MeshRenderer>();
	}

	public void Play() {
		isPlaying = true;
	}

	public void Pause() {
		isPlaying = false;
	}

	private void Update() {
		if (isPlaying) {
			offset += Vector2.right * scrollSpeed * Time.deltaTime;
			m_Renderer.material.SetTextureOffset( "_MainTex", offset );
		}
	}
}
