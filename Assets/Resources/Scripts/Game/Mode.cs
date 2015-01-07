using System;
namespace Game {
	public interface Mode {
		void beginMode(EventHandler finishHandler);

		void onTick();

		bool isActive();

		string getName();
	}
}

