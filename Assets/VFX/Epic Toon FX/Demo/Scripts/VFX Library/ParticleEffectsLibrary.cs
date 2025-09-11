using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ETFXPEL
{

public class ParticleEffectsLibrary : MonoBehaviour {
	public static ParticleEffectsLibrary GlobalAccess;
	void Awake () {
		GlobalAccess = this;

		nowActivePEList = new List<Transform> ();

		TotalEffects = ParticleEffectPrefabs.Length;

		nowParticleEffectNum = 1;

		// Warn About Lengths of Arrays not matching
		if (ParticleEffectSpawnOffsets.Length != TotalEffects) {
			Debug.LogError ("ParticleEffectsLibrary-ParticleEffectSpawnOffset: Not all arrays match length, double check counts.");
		}
		if (ParticleEffectPrefabs.Length != TotalEffects) {
			Debug.LogError ("ParticleEffectsLibrary-ParticleEffectPrefabs: Not all arrays match length, double check counts.");
		}

		// Setup Starting PE Name String
		effectNameString = ParticleEffectPrefabs [nowParticleEffectIndex].name + " (" + nowParticleEffectNum.ToString() + " of " + TotalEffects.ToString() + ")";
	}

	// Stores total number of effects in arrays - NOTE: All Arrays must match length.
	public int TotalEffects = 0;
	public int nowParticleEffectIndex = 0;
	public int nowParticleEffectNum = 0;
//	public string[] ParticleEffectDisplayNames;
	public Vector3[] ParticleEffectSpawnOffsets;
	// How long until Particle Effect is Destroyed - 0 = never
	public float[] ParticleEffectLifetimes;
	public GameObject[] ParticleEffectPrefabs;

	// Storing for deleting if looping particle effect
	#pragma warning disable 414
	private string effectNameString = "";
	#pragma warning disable 414
	private List<Transform> nowActivePEList;

	void Start () {
	}

	public string GetnowPENameString() {
		return ParticleEffectPrefabs [nowParticleEffectIndex].name + " (" + nowParticleEffectNum.ToString() + " of " + TotalEffects.ToString() + ")";
	}

	public void PreviousParticleEffect() {
		// Destroy Looping Particle Effects
		if (ParticleEffectLifetimes [nowParticleEffectIndex] == 0) {
			if (nowActivePEList.Count > 0) {
				for (int i = 0; i < nowActivePEList.Count; i++) {
					if (nowActivePEList [i] != null) {
						Destroy (nowActivePEList [i].gameObject);
					}
				}
				nowActivePEList.Clear ();
			}
		}

		// Select Previous Particle Effect
		if (nowParticleEffectIndex > 0) {
			nowParticleEffectIndex -= 1;
		} else {
			nowParticleEffectIndex = TotalEffects - 1;
		}
		nowParticleEffectNum = nowParticleEffectIndex + 1;

		// Update PE Name String
		effectNameString = ParticleEffectPrefabs [nowParticleEffectIndex].name + " (" + nowParticleEffectNum.ToString() + " of " + TotalEffects.ToString() + ")";
	}
	public void NextParticleEffect() {
		// Destroy Looping Particle Effects
		if (ParticleEffectLifetimes [nowParticleEffectIndex] == 0) {
			if (nowActivePEList.Count > 0) {
				for (int i = 0; i < nowActivePEList.Count; i++) {
					if (nowActivePEList [i] != null) {
						Destroy (nowActivePEList [i].gameObject);
					}
				}
				nowActivePEList.Clear ();
			}
		}

		// Select Next Particle Effect
		if (nowParticleEffectIndex < TotalEffects - 1) {
			nowParticleEffectIndex += 1;
		} else {
			nowParticleEffectIndex = 0;
		}
		nowParticleEffectNum = nowParticleEffectIndex + 1;

		// Update PE Name String
		effectNameString = ParticleEffectPrefabs [nowParticleEffectIndex].name + " (" + nowParticleEffectNum.ToString() + " of " + TotalEffects.ToString() + ")";
	}

	private Vector3 spawnPosition = Vector3.zero;
	public void SpawnParticleEffect(Vector3 positionInWorldToSpawn) {
		// Spawn nowly Selected Particle Effect
		spawnPosition = positionInWorldToSpawn + ParticleEffectSpawnOffsets[nowParticleEffectIndex];
		GameObject newParticleEffect = GameObject.Instantiate(ParticleEffectPrefabs[nowParticleEffectIndex], spawnPosition, ParticleEffectPrefabs[nowParticleEffectIndex].transform.rotation) as GameObject;
		newParticleEffect.name = "PE_" + ParticleEffectPrefabs[nowParticleEffectIndex];
		// Store Looping Particle Effects Systems
		if (ParticleEffectLifetimes [nowParticleEffectIndex] == 0) {
			nowActivePEList.Add (newParticleEffect.transform);
		}
		nowActivePEList.Add(newParticleEffect.transform);
		// Destroy Particle Effect After Lifetime expired
		if (ParticleEffectLifetimes [nowParticleEffectIndex] != 0) {
			Destroy(newParticleEffect, ParticleEffectLifetimes[nowParticleEffectIndex]);
		}
	}
}
}