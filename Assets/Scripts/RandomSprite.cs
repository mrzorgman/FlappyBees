using UnityEngine;
using System.Collections;

public class RandomSprite : MonoBehaviour {

    public Sprite[] sprites;
	
	void Start () {
	if (sprites.Length > 0)
        {
            var s = GetComponent<SpriteRenderer>();
            if (s)
            {
                s.sprite = sprites[Random.Range(0, sprites.Length)];
            }
        }
	}
	}
