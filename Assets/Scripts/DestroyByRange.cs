using UnityEngine;
using System.Collections;

public class DestroyByRange : MonoBehaviour {

    Transform p;

    [SerializeField]
    private float shift;

	void Start () {
        p = FindObjectOfType<PlayerController>().transform;
	}
	
	void Update () {
        if (p.position.x - transform.position.x > shift) Destroy(gameObject);
	}
}
