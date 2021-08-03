using System.Xml.Linq;
using UnityEngine;

public class AssetLoader : MonoBehaviour
{
    private void Awake()
    {
        LoadSpriteSheets();
        LoadXmls();
        Load3DModels();
        Destroy(gameObject);
    }

    private void LoadSpriteSheets()
    {
        var spritesXml = XElement.Parse(Resources.Load<TextAsset>("Sprite Sheets/SpriteSheets").text);

        foreach (var sheetXml in spritesXml.Elements("Sheet"))
        {
            var sheetData = new SpriteSheetData(sheetXml);
            var texture = Resources.Load<Texture2D>($"Sprite Sheets/{sheetData.SheetName}");
            if (sheetData.IsAnimation())
            {
                AssetLibrary.AddAnimations(texture, sheetData);
            }
            else
            {
                AssetLibrary.AddImages(texture, sheetData);
            }
        }
    }

    private void LoadXmls()
    {
        var xmlAssets = Resources.LoadAll<TextAsset>("Xmls");

        foreach (var xmlAsset in xmlAssets)
        {
            var xml = XElement.Parse(xmlAsset.text);
            AssetLibrary.ParseXml(xml);
        }
    }

    private void Load3DModels()
    {
        var modelAsset = Resources.LoadAll<GameObject>("Models");

        foreach (var model in modelAsset)
        {
            AssetLibrary.AddModel(model);
        }
    }
}