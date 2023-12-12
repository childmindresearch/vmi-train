using UnityEngine;
using UnityEngine.UI;

public class Tiles : MonoBehaviour
{
    public void Start()
    {
        var config = ExperimentSerialization.LoadFromTxt(GlobalManager.Instance.configFile);
        int n_rooms = config.rooms.Length;
        int current_room = GlobalManager.Instance.currentRoom;

        int numColumns = Mathf.Max(2, Mathf.CeilToInt(Mathf.Sqrt(n_rooms)));
        int numRows = Mathf.Max(2, Mathf.CeilToInt((float)n_rooms / numColumns));

        var canvas = GameObject.Find("Canvas");
        RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();
        float screenHeight = canvasRectTransform.rect.height;
        float screenWidth = canvasRectTransform.rect.width;

        float tileWidth = screenWidth / numColumns;
        float tileHeight = screenHeight / numRows;

        for (int i = 0; i < n_rooms; i++)
        {
            int difficulty = config.rooms[i].difficulty;
            GameObject prefab = Resources.Load("Prefabs/" + difficulty + "_stars") as GameObject;
            GameObject tile = Instantiate(prefab);
            RectTransform tileRectTransform = tile.GetComponent<RectTransform>();
            tile.transform.SetParent(canvas.transform, false);

            tileRectTransform.sizeDelta = new Vector2(tileWidth, tileHeight);

            int row = i / numColumns;
            int column = i % numColumns;

            float xPosition = tileWidth * column - tileWidth * 0.5f;
            float yPosition = screenHeight - tileHeight * (row + 1) - tileHeight * 0.5f;

            tileRectTransform.anchoredPosition = new Vector2(xPosition, yPosition);

            GameObject text = tile.transform.Find("RoomNumber").gameObject;
            text.GetComponent<TMPro.TextMeshProUGUI>().text = "Room " + (i + 1).ToString();
            if (i != current_room)
            {
                Image image = tile.transform.Find("Image").gameObject.GetComponent<Image>();
                image.color = new Color(1f, 1f, 1f, 0.5f);
            }
        }
    }
}
