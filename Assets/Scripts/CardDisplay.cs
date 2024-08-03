using Normal.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : RealtimeComponent<CardDisplayModel>
{
    private Image _image;
    
    private void Awake()
    {
        // Get a reference to the image component
        _image = GetComponentInChildren<Image>();
    }
    
    protected override void OnRealtimeModelReplaced(CardDisplayModel previousModel, CardDisplayModel currentModel) {
        if (previousModel != null) {
            // Unregister from old events in case of a server change
            previousModel.spriteDidChange -= SpriteDidChange;
        }
        
        if (currentModel != null) {
            // If this is a model that has no data set on it, populate it with the current sprite.
            if (currentModel.isFreshModel)
                currentModel.sprite = _image.overrideSprite.name;

            // Update the sprite to match the new model
            UpdateSprite();

            // Register for events so we'll know if the sprite changes later
            currentModel.spriteDidChange += SpriteDidChange;
        }
    }

    private void SpriteDidChange(CardDisplayModel model, string value) {
        UpdateSprite();
    }

    private void UpdateSprite() {
        // Get the sprite location from the model, load it and set it on the image component.
        _image.overrideSprite = Resources.Load<Sprite>(model.sprite) ;
    }

    public void SetSprite(string sprite) {
        // Set the sprite on the model
        // This will fire the spriteDidChange event on the model, which will update the image component for both the local player and all remote players.
        model.sprite = sprite;
    }
}