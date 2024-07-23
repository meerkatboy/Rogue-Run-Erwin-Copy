using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Level2 : MonoBehaviour
{
    //parent
    public GameObject enemies;
    public GameObject traps;

    //setups for ground enemies
    public GameObject octo;
    public int octoCount = 1;

    public GameObject exploder;
    public int exploderCount = 1;

    public int groundCount = 2;
    private int _groundTypes = 2;


    //setups for traps
    public GameObject lightning;
    public int lightningCount = 1;
    public int trapCount = 1;
    private int _trapTypes = 1;


    //array of positions to spawn ground enemies in
    [SerializeField] private List<Transform> groundPos;

    //array of positions to spawn traps in
    [SerializeField] private List<Transform> trapPos;


    // Start is called before the first frame update
    void Start()
    {
        //randomizes the spawns by taking random positions
        Transform[] groundTransforms = groundPos.OrderBy(x => Random.value).Take(groundCount).ToArray();


        int[] groundCounts = new[] { octoCount, exploderCount };
        GameObject[] groundObj = new[] { octo, exploder };
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

        //randomizes the trap spawns by taking random positions
        Transform[] trapTransforms = trapPos.OrderBy(x => Random.value).Take(trapCount).ToArray();


        int[] trapCounts = new[] { lightningCount };
        GameObject[] trapObj = new[] { lightning };
        int trapSpawnCount = 0;
        //spawn all of the required trap enemies randomly
        for (int pointer = 0; pointer < _trapTypes; pointer++)
        {
            for (int i = 0; i < trapCounts[pointer]; i++)
            {
                Instantiate(trapObj[pointer], trapTransforms[trapSpawnCount].position, Quaternion.identity,
                    enemies.transform);
                trapSpawnCount++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}