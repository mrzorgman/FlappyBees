using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] playerPrefabs;
    [SerializeField]
    private GameObject[] wallPrefabs;
    [SerializeField]
    private GameObject hivePrefab;

    [SerializeField]
    public float lengthDelay;

    public float lengthLevel = 50f;

    public Transform player;

    [SerializeField]
    private float shift = 20f;

    private float pos = 0;

    private void OnEnable()
    {
        int l = 1;

        var plyr = Instantiate(playerPrefabs[PlayerPrefsPlus.GetInt("skin")], transform.position, Quaternion.identity);

        if (GameManager.instance)
        {
            print("LEVEL " + GameManager.instance.level);
            l = GameManager.instance.level;

            if (GameManager.instance.isEndless)
                lengthLevel = float.MaxValue;
            else
                lengthLevel = 20f + l + Mathf.Pow(1.2f, l);
        }

        pos = lengthDelay;
        while (pos < shift)
        { 
            var wall = Instantiate(wallPrefabs[Random.Range(0, wallPrefabs.Length)], new Vector3(pos, 0f, 0f), Quaternion.identity);
            pos += lengthDelay;
        }

        var hive = Instantiate(hivePrefab, new Vector3(lengthLevel, 0f, 0f), Quaternion.identity);

        player = FindObjectOfType<PlayerController>().transform;
        player.GetComponent<PlayerController>().enabled = true;
    }

    private void Update()
    {
        if (player == null) player = FindObjectOfType<PlayerController>().transform;
        else
        {
            if (player.position.x + shift >= pos && lengthLevel > pos)
            {
                //print("game - " + pos);
                var wall = Instantiate(wallPrefabs[Random.Range(0, wallPrefabs.Length)], new Vector3(pos, 0f, 0f), Quaternion.identity);
                pos += lengthDelay;
            }
        }
    }
}