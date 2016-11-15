using UnityEngine;
using System.Collections;

public class TextureScroller : MonoBehaviour {

    public float speed;
    private Transform pos;
    private Renderer r;

    public Texture[] textures;

    //private Vector2 p;

    private void Awake()
    {
        r = GetComponent<Renderer>();
      if (textures.Length > 0)  r.material.mainTexture = textures[Random.Range(0, textures.Length)];
    }

    private void Start()
    {
        pos = Camera.main.transform;
        //p = pos.position;
    }

    private void Update()
    {
        //print(pos.position.x - p.x);

        //r.material.mainTextureOffset = new Vector2(Mathf.Repeat( Time.time * speed,1f), r.material.mainTextureOffset.y);
        r.material.mainTextureOffset = new Vector2(Mathf.Repeat(Camera.main.transform.position.x * speed, 1f), r.material.mainTextureOffset.y);

        //p = pos.position;
    }
}
