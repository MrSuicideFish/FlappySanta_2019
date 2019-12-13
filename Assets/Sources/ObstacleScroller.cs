using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleScroller : MonoBehaviour {

	[Header( "Obstacles" )]
	public GameObject[ ] obstacleCollection;

	[Header( "Settings" )]
	[Range(0.25f, 10.0f)]
	public float frequency;
	[Range( 1.0f, 10.0f)]
	public float speed;
	public Bounds bounds;

	public bool spawnsEnabled;

	private Coroutine stepObstacleRoutine;
	private bool isPlaying;

	public void Play() {
		isPlaying = true;
		if(stepObstacleRoutine == null) {
			stepObstacleRoutine = StartCoroutine( StepObstacles() );
		}
	}

	public void Pause() {
		isPlaying = false;
		stepObstacleRoutine = null;
	}

	private IEnumerator StepObstacles() {

		float t = 0.0f;
		List<Transform> spawnedObstacles = new List<Transform>();

		while (isPlaying) {

			if (spawnsEnabled) {

				t += Time.deltaTime;
				float p = t / frequency;
				if (p >= 1.0f || spawnedObstacles.Count == 0) {
					Transform newObstacle = SpawnNewObstacle();
					if (newObstacle != null) {
						spawnedObstacles.Add( newObstacle );
					}
					t = 0.0f;
				}

				for (int i = 0; i < spawnedObstacles.Count; i++) {

					Transform o = spawnedObstacles[i];
					if (o != null) {

						Vector3 oldPos = o.position;
						Vector3 offset = Vector3.left * speed * Time.deltaTime;

						// is this a cross frame?
						float crossX = GameController.inst.playerSanta.transform.position.x;
						if (oldPos.x > crossX && oldPos.x + offset.x < crossX) {
							GameController.inst.AddPoint();
						}

						// move obstacle
						o.position += offset;

						// check if obstacle is outside of bounds
						if (!bounds.Contains( o.position )) {
							GameObject.Destroy( o.gameObject );
							spawnedObstacles.RemoveAt( i );
						}
					}
				}
			}

			yield return null;
		}

		stepObstacleRoutine = null;
	}

	private Transform SpawnNewObstacle() {

		int idx = UnityEngine.Random.Range( 0, obstacleCollection.Length );
		GameObject prefab = obstacleCollection[idx];

		if(prefab != null) {

			Vector3 spawnPos = bounds.center;
			spawnPos.x += bounds.extents.x;

			float yMin = bounds.center.y - (bounds.extents.y / 2.0f);
			float yMax = bounds.center.y + (bounds.extents.y / 2.0f);
			spawnPos.y = UnityEngine.Random.Range( yMin, yMax );

			GameObject newInst = GameObject.Instantiate(
				original: prefab,
				position: spawnPos,
				rotation: Quaternion.Euler( 0, 0, 0 ) );

			//newInst.transform.SetParent(
			//	parent: this.transform,
			//	worldPositionStays: true );

			return newInst.transform;
		}

		return null;
	}

#if UNITY_EDITOR
	private void OnDrawGizmos() {
		Gizmos.DrawWireCube( bounds.center, bounds.extents );
	}
#endif
}
