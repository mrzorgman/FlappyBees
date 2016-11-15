using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

    public Transform follow;
    [SerializeField]
    private float offset;

	void Start () {
        follow = FindObjectOfType<PlayerController>().transform;
	}
	
	void Update () {
       if (follow) transform.position = new Vector3(follow.position.x  + (follow == transform ? 0 : offset), transform.position.y, transform.position.z);
	}
}
