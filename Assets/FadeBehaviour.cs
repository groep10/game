using UnityEngine;
using System.Collections;

public class FadeBehaviour : MonoBehaviour {

	public Renderer renderer;
	private float opacity;
	private Shader transparentShader;
	private Color currentColor;
	private float fadeTime = 3;

	// creates the transparent shader
	void setTransparentShader(){
		transparentShader = Shader.Find ("Transparent/Diffuse");
		renderer.material.shader = transparentShader;
	}

	// get the red value of rgb
	float getRed(){
		currentColor = renderer.material.color;
		float red = currentColor.r;
		return red;
	}

	// get the green value of rgb
	float getGreen(){
		currentColor = renderer.material.color;
		float green = currentColor.g;
		return green;
	}

	// get the blue value of rgb
	float getBlue(){
		currentColor = renderer.material.color;
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
		curTime = 0;
	}

	// starts the fading out of the object
	void startFadeOut() {
		fadeIn = false;
		curTime = 0;
	}
	// Use this for initialization
	void Start () {
		// make the objects shader transparent
		setTransparentShader ();
	}
	
	// Update is called once per frame
	float time = 5;
	float curTime = 0;
	bool fadeIn = true;

	void Update () {
		if (curTime >= time) {
			return;
		}
		curTime += Time.deltaTime;
		if(fadeIn) {
			// fade the object in
			renderer.material.color = Color.Lerp (setFullyTransparentColor(), setFullyOpaqueColor(), curTime / time);
		} else {
			// fade the object out
			renderer.material.color = Color.Lerp (setFullyOpaqueColor(), setFullyTransparentColor(), curTime / time);
		}
	}
}
