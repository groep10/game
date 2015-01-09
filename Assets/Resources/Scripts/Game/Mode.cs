namespace Game {
	public interface Mode {
		void beginMode(System.Action finishHandler);
		void endMode();

		void onTick();

		void reset();

		bool isActive();

		string getName();
	}
}

