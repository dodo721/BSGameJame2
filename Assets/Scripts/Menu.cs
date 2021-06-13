using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{

    public MapGen mapGen;
    public BoatController player;
    public GameObject tutorial;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame () {
        player.enabled = true;
        //mapGen.enabled = true;
        gameObject.SetActive(false);
    }

    public void Tutorial () {
        tutorial.SetActive(true);
    }

    public void StopTutorial () {
        tutorial.SetActive(false);
    }

    public void Quit () {
        Application.Quit();
    }
}
