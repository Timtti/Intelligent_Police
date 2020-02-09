using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSteering : MonoBehaviour {
	
	//set up variables
	//reference to 2D rigidbody of car
	Rigidbody2D rb;

	//following variable is serialized
	[SerializeField]
	float accelerationPower = 5f;
	//following variable is serialized
	[SerializeField]
	float steeringPower = 5f;
	float steeringAmount, speed, direction;

	// Use this for initialization
	void Start () {
		//set rigidbody to the component
		rb = GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	//Fixed Update controls the movement of the car
	void FixedUpdate () {

		//assign steering amount to read left and right inputs as negative values
		steeringAmount = - Input.GetAxis ("Horizontal");
		//assign speed to up and down inputs multiplied against accerkeration power variable
		speed = Input.GetAxis ("Vertical") * accelerationPower;
		//assign direction to Mathf.sign of a product of the rigidbody's vectors velocity and up
		//Mathf.sign gives 1 when positive value given and -1 when negative value given
		direction = Mathf.Sign(Vector2.Dot (rb.velocity, rb.GetRelativeVector(Vector2.up)));
		//increase rigidbody rotation to product of steering amount, steering power, magnitude of rigidbodies velocity and direction
		rb.rotation += steeringAmount * steeringPower * rb.velocity.magnitude * direction;

		//force to the rigidbody is added by the vector up multiplied by the speed variable
		//this moves the car forward
		rb.AddRelativeForce (Vector2.up * speed);

		/*more force to the rigidbody is added by negative vector right multiplied by the magnitude of the velocity 
		 * and steering amount all divided by 2
		 * this adds a drift affect when the arrow buttons are released
		 * */
		rb.AddRelativeForce ( - Vector2.right * rb.velocity.magnitude * steeringAmount / 2);
			
	}


}
