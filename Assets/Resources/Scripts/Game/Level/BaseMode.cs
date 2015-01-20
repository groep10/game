using UnityEngine;
using System.Collections;

namespace Game.Level
{
	public abstract class BaseMode : MonoBehaviour, Game.Mode  {
			
		protected bool isRunning = false;
		protected System.Action onDone;
		public BaseMode() {
				
		}

		public virtual void beginMode(System.Action finishHandler) {
			isRunning = true;
			onDone = finishHandler;
			reset();
		}

		public virtual void endMode() {
			CancelInvoke();
			isRunning = false;
			onDone();
		}

		public virtual void reset() {
			CancelInvoke();
		}
		
		public abstract void onTick();
		
		public abstract string getName();

		public abstract Hashtable[] getScores();

		public bool isActive() {
			return isRunning;
		}
	}
}

