using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneSelector
{
		//Reference.
		private static SceneSelector _instance;
		
		public static List<GameObject> spriteEffects = new List<GameObject> ();
		public float effectSpeed = 1;
		
		private SceneSelector ()
		{
		}
	
		public static SceneSelector Instance {
				get {
						if (_instance == null) {
								_instance = new SceneSelector ();    
						}
			
						return _instance;
				}
		}
	
		public static void GetEffects ()
		{
				Object[] effectsFromDataPath = Resources.LoadAll (@"Prefabs", typeof(GameObject));
				if (effectsFromDataPath == null) {
						return;
				}
		
				foreach (GameObject effect in effectsFromDataPath) {
						spriteEffects.Add (effect);
				}
		}
}
