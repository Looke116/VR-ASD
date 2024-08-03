using System;
using System.Collections.Generic;
using Normal.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Communication : MonoBehaviour
{
    public Realtime realtime;
    public DataCollector dataCollector;

    private Queue<GameObject> displays;

    private void Start()
    {
        // Initialize the variables
        displays = new Queue<GameObject>();
    }

    // Called each time a button is pressed
    public void ButtonPressed(String card)
    {
        // Record which button was pressed
        dataCollector.Record($"Player pressed the \"{card}\" button");

        // Handle special buttons
        // If a button with a scene associated to it is pressed check if the current
        // scene is different from the scene that needs to be changed to and change to it
        // if not ignore and continue
        if (card.Equals("Acasa") && !SceneManager.GetActiveScene().name.Equals("Starting Room"))
        {
            // Record the scene change and change the scene
            dataCollector.Record("Changing scene: Safe Space");
            SceneManager.LoadScene("Starting Room");
        }
        else if (card.Equals("Padure") && !SceneManager.GetActiveScene().name.Equals("Forest Multiplayer"))
        {
            // Record the scene change and change the scene
            dataCollector.Record("Changing scene: Forest");
            SceneManager.LoadScene("Forest Multiplayer");
        }

        // Option needed for Normcore Instantiation
        var options = new Realtime.InstantiateOptions
        {
            ownedByClient = true,
            preventOwnershipTakeover = true,
            useInstance = realtime
        };
        
        // Declare the position and rotation of the objects and instantiate it
        var pos = Camera.main.transform.position + Camera.main.transform.forward + new Vector3(0, 0.2f, 0);
        var rot = new Quaternion();
        var cardDisplay = Realtime.Instantiate("Prefabs/Card Display", pos, rot, options);

        // Get a reference to the card model and update its sprite
        var model = cardDisplay.GetComponent<CardDisplay>();
        model.SetSprite($"Images/{card}");

        // Add the game object to the queue
        displays.Enqueue(cardDisplay);

        // Schedule the destruction of the object to 5 seconds later
        Invoke(nameof(DestroyDisplay), 5);
    }

    private void DestroyDisplay()
    {
        // Destroy the oldest object in the queue
        Realtime.Destroy(displays.Dequeue());
    }
}