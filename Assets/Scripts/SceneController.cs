using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;

    //field for fade animation
    public Animator fadeAnimator;

    //field for player
    private PlayerController _playerController;
    private GameObject _player;

    //field for all data
    private int[] _intStats;
    private float[] _floatStats;
    private int[] _permUpStats;
    private int[] _tempUpStats;

    //number of next room
    private int _nextRoom;
    
    //pool of rooms to draw from
    private List<int> _chapter1 = new List<int>{3,5,7,9};
    private List<int> _chapter2 = new List<int>{13,15};
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ReloadScene()
    {
        NextScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextScene(int id)
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerController = _player.GetComponent<PlayerController>(); //finds the player
        EndRoom();
        SaveStats(); //save all needed stats
        //fade out
        fadeAnimator.SetTrigger("FadeOut");
        StartCoroutine(LoadScene(id)); //make sure the second player component is only after scene loads
    }

    //resets pool of levels
    public void ResetPool()
    {
        _chapter1 = new List<int>{3,5,7,9};
        _chapter2 = new List<int>{13,15};
    }


    //load scene
    private IEnumerator LoadScene(int id)
    {
        //puts player into stasis
        _playerController.Stasis();
        //Wait for 1 seconds for the animation to finish
        yield return new WaitForSeconds(0.8f);

        AsyncOperation asyncLoadLevel;
        // Start loading the scene
        
        if (id == -1)//next scene id
        {
            asyncLoadLevel = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1,
                LoadSceneMode.Single);
        }
        else if (id == -2) //pool next level in 1-
        {
            asyncLoadLevel = SceneManager.LoadSceneAsync(GetNextRoom(1), LoadSceneMode.Single);
        }
        else if (id == -3) //pool next level in 2-
        {
            asyncLoadLevel = SceneManager.LoadSceneAsync(GetNextRoom(2), LoadSceneMode.Single);
        }
        else
        {
            asyncLoadLevel = SceneManager.LoadSceneAsync(id, LoadSceneMode.Single);
        }

        fadeAnimator.SetTrigger("FadeIn");

        // Wait until the level finish loading
        while (!asyncLoadLevel.isDone)
            yield return null;
        // Wait a frame so every Awake and Start method is called
        yield return new WaitForEndOfFrame();

        //reload player
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerController = _player.GetComponent<PlayerController>(); //finds the player
        LoadStats(); //save all needed stats
        StartRoom();
    }

    //save the stats locally
    private void SaveStats()
    {
        var data = _playerController.SaveStats();
        _intStats = data.intStats;
        _floatStats = data.floatStats;
        _permUpStats = data.permUpStats;
        _tempUpStats = data.tempUpStats;
    }

    //load the stats to the player
    private void LoadStats()
    {
        _playerController.LoadStats(_intStats, _floatStats, _permUpStats, _tempUpStats);
    }

    //action to player when ending scene
    private void EndRoom()
    {
        //heal if upgrade
        _playerController.RoomHeal();
    }

    //action to player when new scene
    private void StartRoom()
    {
        //activate vanguard
        _playerController.Vanguard();
    }

    
    //gets the next room number by randomizing
    private int GetNextRoom(int index)
    {
        //pool from chapter 1
        if (index == 1)
        {
            //load boss fight
            if (_chapter1.Count == 0)
            {
                return 11;
            }
            //gets index, return scene number, remove from pool
            int sceneNum = Random.Range(0, _chapter1.Count);
            int scene = _chapter1[sceneNum];
            _chapter1.RemoveAt(sceneNum);
            return scene;
        }
        //pool from chapter 2
        if (index == 2)
        {
            //gets index, return scene number, remove from pool
            int sceneNum = Random.Range(0, _chapter2.Count);
            int scene = _chapter2[sceneNum];
            _chapter2.RemoveAt(sceneNum);
            return scene;
        }

        return 0;
    }
}