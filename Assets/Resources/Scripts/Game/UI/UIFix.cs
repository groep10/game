using UnityEngine;
using System.Collections;

namespace Game.UI {
	public class UIFix : MonoBehaviour {

		public UIFix parent1;
		public RectTransform.Edge edge1;
		public float edge1_offset_pixels;
		public float edge1_offset_percent;

		public float edge1_size_pixels;
		public float edge1_size_percent;


		public UIFix parent2;
		public RectTransform.Edge edge2;
		public float edge2_offset_pixels;
		public float edge2_offset_percent;
		
		public float edge2_size_pixels;
		public float edge2_size_percent;

		void fix() {
			// Debug.Log ("fix");
			RectTransform rect = GetComponent<RectTransform> ();

			rect.SetInsetAndSizeFromParentEdge (edge1, offset1 (), size1 ());
			rect.SetInsetAndSizeFromParentEdge (edge2, offset2 (), size2 ());
		}

		public float offset1() {
			float w = Screen.currentResolution.width;
			return edge1_offset_pixels + (w * (edge1_offset_percent / 100f)) + (parent1 != null ? parent1.offset1() + parent1.size1() : 0f);
		}

		public float size1() {
			float w = Screen.currentResolution.width;
			return edge1_size_pixels + (w * (edge1_size_percent / 100f));
		}

		public float offset2() {
			float w = Screen.currentResolution.width;
			return edge2_offset_pixels + (w * (edge2_offset_percent / 100f)) + (parent2 != null ? parent2.offset2() + parent2.size2() : 0f);
		}
		
		public float size2() {
			float w = Screen.currentResolution.width;
			return edge2_size_pixels + (w * (edge2_size_percent / 100f));
		}

		// Update is called once per frame
		float time = 1;
		float cur = 0;
		void Update () {
			cur += Time.deltaTime;
			if (cur >= time) {
				cur = 0;
				fix ();
			}
		}
	}
}