using UnityEngine;

public class PaletteSwitcher : MonoBehaviour
{
    public Material tilemapMaterial;

    public Texture2D desertPalette;
    public Texture2D lavaPalette;

    void Start()
    {
        // example: start with desert palette
        tilemapMaterial.SetTexture("_Palette", desertPalette);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            tilemapMaterial.SetTexture("_Palette", desertPalette);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            tilemapMaterial.SetTexture("_Palette", lavaPalette);
        }
    }
}