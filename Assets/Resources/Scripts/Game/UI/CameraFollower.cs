using UnityEngine;
using System.Collections;

namespace Game.UI {
	public class CameraFollower : MonoBehaviour {
		public Transform target;
		public Transform camera;
		public float distance = 10.0f; //afstand van de target in x-z veld
		public float height = 5.0f; //hoeveel hoger de camera zit
		public float heightDamping = 2.0f;
		public float rotationDamping = 3.0f;

		void LateUpdate () {
			while (camera == null){
				camera = Camera.main.transform;
			}

			if (!target) { //Check of er een target is
				return;
			}

			//Rotatie hoek van target bepalen en hoogte bepalen waar camera heen moet
			float wantedHeight = target.position.y + height;
			float wantedRotationAngle;
			if (target.rigidbody.velocity.magnitude < 0.01) { //Anders geeft de Quaternion.LookRotation error
				wantedRotationAngle = transform.eulerAngles.y;
			} else {
				wantedRotationAngle = Quaternion.LookRotation(target.rigidbody.velocity + target.transform.forward).eulerAngles.y;
			}


			//Huidige locatie camera
			float currentRotationAngle = camera.eulerAngles.y;
			float currentHeight = camera.position.y;

			// Met lerp interpoleren tussen huidige locatie en waar camera naartoe moet zodat beweging smooth is
			currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
			currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);

			//rotatie van camera goed zetten met hierboven berekende draaihoek
			Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

			// positie camera goed zetten met berekende hoogte en rotatie en stuk naar achter plaatsen.
			Vector3 pos = target.position;
			camera.position = pos;
			camera.position -= currentRotation * Vector3.forward * distance;
			camera.position = new Vector3(camera.position.x, currentHeight, camera.position.z);

			//Roteert de camera zodat de voorwaartse vector op target gericht is.
			camera.LookAt (pos);
		}
	}
}