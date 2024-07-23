using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Level1 : MonoBehaviour
{
    //parent
    public GameObject enemies;

    //setups for ground enemies
    public GameObject octo;
    public int octoCount = 3;

    public int groundCount = 3;
    private int _groundTypes = 1;


    //array of positions to spawn ground enemies in
    [SerializeField] private List<Transform> groundPos;


    // Start is called before the first frame update
    void Start()
    {
        //randomizes the spawns by taking random positions
        Transform[] groundTransforms = groundPos.OrderBy(x => Random.value).Take(groundCount).ToArray();


        int[] groundCounts = new[] { octoCount };
        GameObject[] groundObj = new[] { octo };
        int spawnCount = 0;
        //spawn all of the required ground enemies randomly
        for (int pointer = 0; pointer < _groundTypes; pointer++)
        {
            for (int i = 0; i < groundCounts[pointer]; i++)
            {
                Instantiate(groundObj[pointer], groundTransforms[spawnCount].position, Quaternion.identity,
                    enemies.transform);
                spawnCount++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}