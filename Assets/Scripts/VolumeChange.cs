using System;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class VolumeChange : MonoBehaviour
{
    public GameObject canvasPrefab;
    public GameObject leftController;
    
    private GameObject _canvas;
    private NearFarInteractor _leftInteractor;
    private NearFarInteractor _rightInteractor;

    private void Start()
    {
        // Get a reference to the NearFarInteractor component
        _leftInteractor = leftController.GetComponentInChildren<NearFarInteractor>();

        // Assign relevant functions to the select listners
        _leftInteractor.selectEntered.AddListener(CreateCanvas);
        _leftInteractor.selectExited.AddListener(DeleteCanvas);
    }

    private void CreateCanvas(SelectEnterEventArgs args)
    {
        // Get the selected game object
        XRBaseInteractable selectedObject;
        if (_leftInteractor.hasSelection)
            selectedObject = (XRBaseInteractable)_leftInteractor.interactablesSelected[0];
        else if (_rightInteractor.hasSelection)
            selectedObject = (XRBaseInteractable)_rightInteractor.interactablesSelected[0];
        else return;

        // Get the audio source attached to the game object
        AudioSource audioSource = selectedObject.GetComponentInChildren<AudioSource>();

        // If the game object has no audio source then end the execution here
        if (audioSource == null) return;
        
        // Create the UI
        _canvas = Instantiate(canvasPrefab);

        // Get references to important objects
        var label = _canvas.GetComponentInChildren<TMP_Text>();
        var slider = _canvas.GetComponentInChildren<Slider>();
        var valueDisplay = slider.gameObject.GetNamedChild("Value Text").GetComponent<TMP_Text>();

        // Update the UI with the relevant information
        _canvas.transform.position =
            leftController.transform.position + Camera.main.transform.forward - new Vector3(0, 0.2f, 0);
        label.text = selectedObject.name;
        ChangeVolume(audioSource, audioSource.volume, valueDisplay);

        // Create a listener to change the volume every time the slider is updated
        slider.onValueChanged.AddListener(delegate { ChangeVolume(audioSource, slider.value, valueDisplay); });
    }

    private void DeleteCanvas(SelectExitEventArgs args)
    {
        // Delete the game object
        Destroy(_canvas);
    }

    private void ChangeVolume(AudioSource source, float volume, TMP_Text textDisplay)
    {
        // Update the volume on the slider and the audio source
        _canvas.GetComponentInChildren<Slider>().value = volume;
        source.volume = volume;

        // Map the volume variable from (0,1) to (0,100) 
        textDisplay.text = (Math.Round(volume, 2) * 100) + "%";
    }
}