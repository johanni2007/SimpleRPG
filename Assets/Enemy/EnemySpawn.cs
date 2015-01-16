﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct monsters {
		//GeneralStats
		public int Monster_ID;
		public string pname;
		public Vector2 pos;
		public GameObject prefab;
		public bool boss;
	
		//Stats for Fight
		public int hp ; 
		public int maxhp ; 
		public int mana ; 
		public int maxmana ; 
		public int lvl ; 
		public int pwr ; 
		public int armor ; 
		public int agility ; 
	
		//Drops
		public int golddrop;
		public int xpdrop;
		public List<items> loot;
		public int[] spawnregion;
}

public class EnemySpawn : MonoBehaviour {
		public List<monsters> enemyTypes = new List<monsters> ();
		int maxmobs = 15;
		public int mobs = 0;
		float spawncooldown = 1.0f;
		float spawntimer;
		bool isspawning = true;
		player p001;
		TileMap	map;
		
		// Use this for initialization
		monsters CreatEmpty () {
				monsters mob = new monsters ();
				mob.Monster_ID = enemyTypes.Count + 1;
				mob.pname = "Standart Name";
				mob.pos = Vector2.zero;
				mob.prefab = (GameObject)Resources.Load ("Mob/Enemy");
				mob.boss = false;
				mob.maxhp = 1; 
				mob.hp = mob.maxhp;
				mob.maxmana = 1; 
				mob.mana = mob.maxmana; 
				mob.lvl = 1; 
				mob.pwr = 1; 
				mob.armor = 0; 
				mob.agility = 1; 
				mob.golddrop = 0;
				mob.xpdrop = 0;
				mob.loot = new List<items> ();
				return mob;
		}
		void Start () {
				p001 = GameObject.Find ("Main Camera").GetComponent<player> ();
				map = GameObject.Find ("Map").GetComponent<TileMap> ();
			
				monsters tmpmob;
				tmpmob = CreatEmpty ();
				tmpmob.pname = "Stefan";
				tmpmob.maxhp = 200;
				tmpmob.maxmana = 100;
				tmpmob.pwr = 10;
				tmpmob.armor = 10;
				tmpmob.agility = 1;
				tmpmob.golddrop = 80;
				tmpmob.xpdrop = 35;
				tmpmob.prefab = (GameObject)Resources.Load ("Mob/Gorilla");
				tmpmob.spawnregion = new int[1];
				tmpmob.spawnregion [0] = map.GetRegionWithName ("Forest");
				enemyTypes.Add (tmpmob);
		
				tmpmob = CreatEmpty ();
				tmpmob.pname = "Spider";
				tmpmob.maxhp = 100;
				tmpmob.maxmana = 100;
				tmpmob.pwr = 5;
				tmpmob.armor = 10;
				tmpmob.agility = 2;
				tmpmob.golddrop = 100;
				tmpmob.xpdrop = 25;
				tmpmob.spawnregion = new int[2];
				tmpmob.spawnregion [0] = map.GetRegionWithName ("Forest");
				tmpmob.spawnregion [1] = map.GetRegionWithName ("Grass");
				enemyTypes.Add (tmpmob);
		
				tmpmob = CreatEmpty ();
				tmpmob.pname = "Cyclop";
				tmpmob.maxhp = 1000;
				tmpmob.maxmana = 100;
				tmpmob.pwr = 100;
				tmpmob.armor = 50;
				tmpmob.agility = 1;
				tmpmob.golddrop = 1000;
				tmpmob.xpdrop = 250;
				tmpmob.prefab = (GameObject)Resources.Load ("Mob/Cyclop");
				tmpmob.boss = true;
				tmpmob.pos = new Vector2 (50, 52);
				//tmpmob.spawnregion = new int[1];
				//tmpmob.spawnregion [0] = map.GetRegionWithName ("Bei Boss egal");
				enemyTypes.Add (tmpmob);
		
				spawnbosses ();
		}
	
		// Update is called once per frame
		void Update () {
				if (mobs < maxmobs) {
						if (isspawning) {
								spawnmob (); // Counter und so jetzt in der Funktion
						}
						spawntimer -= Time.deltaTime;
						if (spawntimer <= 0) {
								isspawning = true;
						}
				}
		}
		bool MobInRegion (monsters mob, int region) {
				bool check = false;
				foreach (int mobregion in mob.spawnregion) {
						if (mobregion == region) {
								check = true;	
						}
				}
				return check;
		}
		void spawnmob () {
				int maxtry = 10;
				Vector3 pos = new Vector3 (Random.Range (p001.pos.x - 30, p001.pos.x + 30), Random.Range (p001.pos.y - 30, p001.pos.y + 30), 0);
				if (map.tiles [(int)pos.x, (int)pos.y] == null) {
						return;
				}
				int mob_id = Random.Range (0, enemyTypes.Count);
				monsters tt = enemyTypes [mob_id];
				while (tt.boss && maxtry>0 && !MobInRegion(tt, map.tiles[(int)pos.x, (int)pos.y])) { // Damit Standart keine Bosse gespawnt werden und er nicht unendlich oft versucht
						mob_id = Random.Range (0, enemyTypes.Count);
						tt = enemyTypes [mob_id];
						maxtry--;
				}
				if (tt.prefab != null) {
						GameObject tmpobjct = (GameObject)Instantiate (tt.prefab, pos, Quaternion.identity);
						tmpobjct.transform.parent = GameObject.Find ("MonsterSpawner").transform;
						tmpobjct.GetComponent<enemy> ().SettingStats (tt);
						tmpobjct.GetComponent<enemy> ().thismob.pos = pos;
						isspawning = false;
						spawntimer = spawncooldown;
						mobs++;
				}
				//Debug.Log ("Mob: " + pos.x + "/" + pos.y);
		}
	
		void spawnbosses () {
				foreach (monsters tmpmob in enemyTypes) {
						if (tmpmob.boss) {
								Vector3 pos = new Vector3 (tmpmob.pos.x, tmpmob.pos.y, 0);	
								GameObject tmpobjct = (GameObject)Instantiate (tmpmob.prefab, pos, Quaternion.identity);
								tmpobjct.transform.parent = GameObject.Find ("MonsterSpawner").transform;
								tmpobjct.GetComponent<enemy> ().SettingStats (tmpmob);
								tmpobjct.GetComponent<enemy> ().thismob.pos = pos;
						}
				}
		}
}
