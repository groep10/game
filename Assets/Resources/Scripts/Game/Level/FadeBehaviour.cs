using UnityEngine;
using System.Collections;

namespace Game.Level {

	public class FadeBehaviour : MonoBehaviour {

		public Renderer objectRenderer;
		private float opacity;
		private Shader transparentShader;
		private Color currentColor;
		private float totalLifeTime;
		private float alp, alpha;

		private System.Action ondone;

		public void setOnDone(System.Action ondone) {
			this.ondone = ondone;
		}

		// creates the transparent shader
		void setTransparentShader(){
			transparentShader = Shader.Find ("Transparent/Diffuse");
			objectRenderer.material.shader = transparentShader;
		}

		// get the red value of rgb
		float getRed(){
			currentColor = objectRenderer.material.color;
			float red = currentColor.r;
			return red;
		}

		// get the green value of rgb
		float getGreen(){
			currentColor = objectRenderer.material.color;
			float green = currentColor.g;
			return green;
		}

		// get the blue value of rgb
		float getBlue(){
			currentColor = objectRenderer.material.color;
			float blue = currentColor.b;
			return blue;
		}

		// returns an invisible color of the provided rgb value
		Color setFullyTransparentColor(){
			float opacity = 0f;
			Color result = new Color (getRed(), getGreen(), getBlue(), opacity);
			return result;
		}

		// returns a fully opaque color of the provided rgb value
		Color setFullyOpaqueColor(){
			float opacity = 1f;
			Color result = new Color (getRed(), getGreen(), getBlue(), opacity);
			return result;
		}

		// starts the fading in of the object
		void startFadeIn() {
			fadeIn = true;
			currentFadeTime = 0;
		}

		// starts the fading out of the object
		void startFadeOut() {
			fadeIn = false;
			currentFadeTime = 0;
		}
		// Use this for initialization
		void Start () {
			// make the objects shader transparent
			setTransparentShader ();
			// Make the object transparent at start-up
			objectRenderer.material.color = setFullyTransparentColor ();
			// The time it takes before the asset is destroyed
			totalLifeTime = Random.Range(10, 15);
		}
		

		float fadeTime = 5;
		float currentFadeTime = 0;
		bool fadeIn = true;
		float currentLifeTime = 0;
		bool isFadingOut = false;

		void doDestroy() {
			Network.Destroy(this.gameObject.networkView.viewID);
			Network.RemoveRPCs (this.gameObject.networkView.viewID);
			if(ondone != null) {
				ondone();
			}
		}

		// Update is called once per frame
		void Update () {
			if(Network.isServer){
				currentLifeTime += Time.deltaTime;
				if (!isFadingOut){
					if (totalLifeTime - currentLifeTime <= fadeTime) {
						startFadeOut();
						isFadingOut = true;
					}
				}

				if (currentFadeTime >= fadeTime) {
					return;
				}
				currentFadeTime += Time.deltaTime;
				if(fadeIn) {
					// fade the object in
					Color color = objectRenderer.material.color;
					color.a += 1/fadeTime*Time.deltaTime;
					objectRenderer.material.color = color;
				} else {
					// fade the object out
					Color color = objectRenderer.material.color;
					color.a -= 1/fadeTime*Time.deltaTime;
					objectRenderer.material.color = color;
					if (currentFadeTime >= fadeTime) {
						if(Network.isServer){
							doDestroy ();
						}
					}
				}
			}
			else{
				Color color = objectRenderer.material.color;
				color.a = alpha;
				Color newColor = color;
				objectRenderer.material.color = Color.Lerp(objectRenderer.material.color,newColor,0.25f);
			}
		}

		void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
			if (stream.isWriting) {
				alp = objectRenderer.material.color.a;
				stream.Serialize(ref alp);
			} else {
				stream.Serialize(ref alp);
				alpha = alp;
			}
		}

	}

}