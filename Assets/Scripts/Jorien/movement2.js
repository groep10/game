#pragma strict

public class movement2 extends MonoBehaviour {
    public var speed : float = 90f;
    public var turnSpeed : float = 5f;
    public var hoverForce : float = 65f;
    public var hoverHeight : float = 3.5f;
    private var powerInput : float;
    private var turnInput : float;
    private var carRigidbody : Rigidbody;


    function Awake () 
    {
        carRigidbody = gameObject.GetComponent(Rigidbody);
    }

    function Update () 
    {
        powerInput = Input.GetAxis ("Vertical");
        turnInput = Input.GetAxis ("Horizontal");
    }

    function FixedUpdate()
    {
        var ray : Ray = new Ray (transform.position, -transform.up);
        var hit : RaycastHit;

        /*if (Physics.Raycast(ray, out hit, hoverHeight))
        {
            float proportionalHeight = (hoverHeight - hit.distance) / hoverHeight;
            Vector3 appliedHoverForce = Vector3.up * proportionalHeight * hoverForce;
            carRigidbody.AddForce(appliedHoverForce, ForceMode.Acceleration);
        }*/

        carRigidbody.AddRelativeForce(0f, 0f, powerInput * speed);
        carRigidbody.AddRelativeTorque(0f, turnInput * turnSpeed, 0f);

    }
}

