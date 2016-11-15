using UnityEngine;
using System.Collections;

public class UI_Bee : MonoBehaviour {

    private Rigidbody2D rb;
    private Animator anim;
    [SerializeField] private float speed;
    private bool dir;

	void Start () {
        dir = true;
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
        rb.velocity = new Vector2(dir ? speed : -speed, 0f);
        anim = GetComponent<Animator>();
        anim.SetBool("isFly", true);
    }
	
    void OnTriggerEnter2D(Collider2D c)
    {
        dir = !dir;
        transform.localScale = new Vector3(dir ? Mathf.Abs(transform.localScale.x) : -Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        rb.velocity = new Vector2(dir ? speed * Random.Range(0.5f, 2.5f) : -speed * Random.Range(0.5f, 2.5f), 0f);
        anim.SetBool("isFast", Mathf.Abs(rb.velocity.x) > speed * 1.75f );
    }
}
