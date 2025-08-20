using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [Tooltip("Drop a TextAsset that contains a single LevelData JSON object.")]
    public TextAsset levelJson;

    public LevelData Load()
    {
        if (levelJson == null)
        {
            Debug.LogWarning("LevelLoader: No JSON assigned; using a 3x3 sample.");
            var sample = "{\"bugCount\":0,\"wordCount\":2,\"timeSec\":0,\"totalScore\":0," +
                         "\"gridSize\":{\"x\":3,\"y\":3},\"gridData\":[" +
                         "{\"tileType\":0,\"letter\":\"W\"},{\"tileType\":0,\"letter\":\"O\"},{\"tileType\":0,\"letter\":\"R\"}," +
                         "{\"tileType\":0,\"letter\":\"A\"},{\"tileType\":0,\"letter\":\"N\"},{\"tileType\":0,\"letter\":\"D\"}," +
                         "{\"tileType\":0,\"letter\":\"F\"},{\"tileType\":0,\"letter\":\"U\"},{\"tileType\":0,\"letter\":\"N\"}]}";
            return JsonUtility.FromJson<LevelData>(sample);
        }
        return JsonUtility.FromJson<LevelData>(levelJson.text);
    }
}