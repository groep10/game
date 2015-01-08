using System;
using UnityEngine;

namespace Game.Level
{
	public abstract class BaseMode : MonoBehaviour, Game.Mode  {
			
		protected bool running = false;
		public BaseMode() {
				
		}

		public abstract void beginMode(EventHandler finishHandler);
		
		public abstract void onTick();
		
		public abstract string getName();

		public bool isActive() {
			return running;
		}
	}
}

