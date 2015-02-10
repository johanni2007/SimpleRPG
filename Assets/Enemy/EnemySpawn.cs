﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawn : MonoBehaviour {
		public CreatureDataList DataListObj;
		public List<CreatureData> enemyTypes = new List<CreatureData> ();
		CreatureData random_mob;
		int maxmobs = 15;
		public int mobs = 0;
		float spawncooldown = 0.5f;
		float spawntimer;
		bool isspawning = true;
		player p001;
		TileMap	map;
		
		void Start () {
				p001 = GameObject.Find ("Main Camera").GetComponent<player> ();
				map = GameObject.Find ("Map").GetComponent<TileMap> ();
				DataListObj = (CreatureDataList)Resources.Load ("Creatures");
				//enemyTypes = DataListObj.CreatureList;
				testmob = new CreatureOriginData[DataListObj.CreatureList.Count];
				DataListObj.CreatureList.CopyTo (testmob);
				foreach (CreatureOriginData tmp in testmob) {
						CreatureData tmp2 = new CreatureData ();
						tmp2.Start (tmp.Clone ());
						enemyTypes.Add (tmp2.Clone ());
				}
				//enemyTypes.();
		}
	
		// Update is called once per frame
		CreatureOriginData[] testmob;
		void Update () {
				if (mobs < maxmobs && GameObject.Find ("Main Camera").GetComponent<mainmenu> ().gameloaded) {
						if (isspawning) {
								
								spawnmob (); // Counter und so jetzt in der Funktion
						}
						spawntimer -= Time.deltaTime;
						if (spawntimer <= 0) {
								isspawning = true;
						}
				}
		}
		bool MobInRegion (CreatureData testmob, int region) {
				bool check = false;
				foreach (int mobregion in testmob.InitalStats.SpawnRegions) {
						if (mobregion == region) {
								check = true;	
						}
				}
				return check;
		}

		bool RandomMobGen () {
				bool return_mob = false;
				bool mob_gefunden = false;
				//int maxtry = 10;
				int maxtry = 10;
				while (!mob_gefunden && maxtry>0) {
						mob_gefunden = true;
						int mob_id = Random.Range (0, enemyTypes.Count);
						random_mob = enemyTypes [mob_id];
						Vector3 pos = new Vector3 (Random.Range (p001.pos.x - 30, p001.pos.x + 30), Random.Range (p001.pos.y - 30, p001.pos.y + 30), 0);
						if (map.tiles [(int)pos.x, (int)pos.y] == null) {
								mob_gefunden = false;
						}
						if (random_mob.InitalStats.Prefab == null) {
								mob_gefunden = false;
						} 
						if (random_mob.InitalStats.IsBoss) {
								mob_gefunden = false;
						} 
						if (!MobInRegion (random_mob, map.tiles [(int)pos.x, (int)pos.y])) {
								mob_gefunden = false;
						}
						if (mob_gefunden) {
								random_mob.Position.x = (int)pos.x;
								random_mob.Position.y = (int)pos.y;
								return_mob = true;
						}
						maxtry--;
				}
		
				return return_mob;	
		}
		void spawnmob () {
				if (RandomMobGen ()) {
						Vector3 pos = new Vector3 (random_mob.Position.x, random_mob.Position.y, 0);
						GameObject tmpobjct = (GameObject)Instantiate (random_mob.InitalStats.Prefab, pos, Quaternion.identity);
						tmpobjct.transform.parent = GameObject.Find ("MonsterSpawner").transform;
						tmpobjct.GetComponent<enemy> ().SettingStats (random_mob);
						tmpobjct.GetComponent<enemy> ().thismob.Position = pos;
						isspawning = false;
						spawntimer = spawncooldown;
						mobs++;
				}
		}
	
		public void spawnbosses () {
				foreach (CreatureData tmpmob in enemyTypes) {
						if (tmpmob.InitalStats.IsBoss) {
								Vector3 pos = new Vector3 (tmpmob.InitalStats.Position.x, tmpmob.InitalStats.Position.y, 0);	
								GameObject tmpobjct = (GameObject)Instantiate (tmpmob.InitalStats.Prefab, pos, Quaternion.identity);
								tmpobjct.transform.parent = GameObject.Find ("MonsterSpawner").transform;
								tmpobjct.GetComponent<enemy> ().SettingStats (tmpmob);
								tmpobjct.GetComponent<enemy> ().thismob.Position = pos;
						}
				}
		}
}
