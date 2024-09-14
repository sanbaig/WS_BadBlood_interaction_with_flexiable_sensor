﻿using UnityEngine;
using System.Collections;

public class BattleController : MonoBehaviour {

	public int roundTime = 100;
	private float lastTimeUpdate = 0;
	private bool battleStarted = false;
	private bool battleEnded;

	public Fighter player1;
	public Fighter player2;
	public BannerController banner;
	public AudioSource musicPlayer;
	public AudioClip backgroundMusic;
	
	void Start () {
		banner.showFight ();
			musicPlayer = gameObject.AddComponent<AudioSource>();
			GameUtils.playSoundtwo(backgroundMusic, musicPlayer);
	}

	private void expireTime(){
		if (player1.healthPercent > player2.healthPercent) {
			player2.health = 0;
		} else if(player2.healthPercent > player1.healthPercent){
			player1.health = 0;
		} else{
			player2.health = player1.health = 0;
		}
	}

	void Update () {

		if (!battleStarted){
			battleStarted = true;

			player1.enable = true;
			player2.enable = true;
		}

		if (battleStarted && !battleEnded) {
			if (roundTime > 0 && Time.time - lastTimeUpdate > 1) {
				roundTime--;
				lastTimeUpdate = Time.time;
				if (roundTime == 0){
					expireTime();
				}
			}

			if (player1.healthPercent <= 0) {
				banner.showYouLose ();
				battleEnded = true;
			} else if (player2.healthPercent <= 0) {
				banner.showYouWin ();
				battleEnded = true;
			}
		}

		/*if (battleEnded){
			Application.LoadLevel("StartScene");
		}*/


	}
}
