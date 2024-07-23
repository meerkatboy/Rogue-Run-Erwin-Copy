using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.iOS;
using UnityEngine;

public class Level4 : MonoBehaviour
{
    //parent
    public GameObject enemies;
    public GameObject traps;

    //setups for ground enemies
    public GameObject octo;
    public int octoCount;

    public GameObject exploder;
    public int exploderCount;

    public int groundCount;
    public int groundTypes;


    //setups for traps
    public GameObject lightning;
    public int lightningCount;

    public GameObject turret;
    public int turretCount;

    public int trapCount;
    public int trapTypes;


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
        for (int pointer = 0; pointer < groundTypes; pointer++)
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


        int[] trapCounts = new[] { lightningCount, turretCount };
        GameObject[] trapObj = new[] { lightning, turret };
        int trapSpawnCount = 0;
        //spawn all of the required trap enemies randomly
        for (int pointer = 0; pointer < trapTypes; pointer++)
        {
            for (int i = 0; i < trapCounts[pointer]; i++)
            {
                GameObject temp = Instantiate(trapObj[pointer], trapTransforms[trapSpawnCount].position,
                    Quaternion.identity,
                    enemies.transform);
                trapSpawnCount++;

                //if turret, randomize facing direction
                if (pointer == 1)
                {
                    temp.transform.localScale = Random.Range(0f, 100f) > 50
                        ? new Vector2(temp.transform.localScale.x * -1,
                            temp.transform.localScale.y)
                        : temp.transform.localScale; //changes localscale randomly;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}