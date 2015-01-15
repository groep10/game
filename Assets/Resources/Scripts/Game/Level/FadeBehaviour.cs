using UnityEngine;
using System.Collections;

namespace Game.Level {

	public class FadeBehaviour : MonoBehaviour {

		public Renderer objectRenderer;
		private Material[] mat;
		private MeshCollider collider;
		private Shader transparentShader;
		private Shader diffuseShader;
		private float totalLifeTime;
		private float alp, alpha;

		private System.Action ondone;

		public void setOnDone(System.Action ondone) {
			this.ondone = ondone;
		}

		// creates the transparent shader
		void setTransparentShader(){
			transparentShader = Shader.Find ("Transparent/Diffuse");
			for(int i=0;i<objectRenderer.materials.Length;i++){
				mat[i].shader = transparentShader;
			}
		}

		// creates the diffuse shader
		void setDiffuseShader(){
			diffuseShader = Shader.Find ("Bumped Diffuse");
			for(int i=0;i<objectRenderer.materials.Length;i++){
				mat[i].shader = diffuseShader;
			}
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
			collider = objectRenderer.gameObject.GetComponent<MeshCollider>();
			mat = objectRenderer.materials;
			setTransparentShader ();
			// Make the object transparent at start-up
			Color temp = objectRenderer.material.color;
			temp.a = 0;
			for(int i=0;i<objectRenderer.materials.Length;i++){
				mat[i].color = temp;
			}
			// The time it takes before the asset is destroyed
			totalLifeTime = Random.Range(30, 40);
		}

		float fadeTime = 5;
		float currentFadeTime = 0;
		bool fadeIn = true;
		float currentLifeTime = 0;
		bool isFadingOut = false;
		bool makediffuse = false;

		void doDestroy() {
			Network.Destroy(transform.parent.gameObject.networkView.viewID);
			Network.RemoveRPCs (transform.parent.gameObject.networkView.viewID);
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
					for(int i=0;i<objectRenderer.materials.Length;i++){
						mat[i].color = color;
					}
				}
				else {
					// fade the object out
					makediffuse = false;
					setTransparentShader ();
					Color color = objectRenderer.material.color;
					color.a -= 1/fadeTime*Time.deltaTime;
					for(int i=0;i<objectRenderer.materials.Length;i++){
						mat[i].color = color;
					}
					if (currentFadeTime >= fadeTime) {
						if(Network.isServer){
							doDestroy ();
						}
					}
				}
			}
			// if client
			else{
				Color color = objectRenderer.material.color;
				color.a = alpha;
				Color newColor = color;
				for(int i=0;i<objectRenderer.materials.Length;i++){
					mat[i].color = Color.Lerp(objectRenderer.material.color,newColor,0.25f);
				}
			}
			//disable collider when fading
			if(objectRenderer.material.color.a < 1){
				collider.enabled = false;
			}
			else{
				collider.enabled = true;
				makediffuse = true;
			}
			if(makediffuse){
				setDiffuseShader();
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