using System;
using UnityEngine;

public static class ExperimentSerialization
{

    [Serializable]
    public class RoomConfiguration
    {
        public int seed = 42;

        public float width = 60f;
        public float height = 40f;

        public float durationSec = 10;
        public int numDistractors = 0;
        public float[] path = { .0f, .5f, 1f, .5f };
        public float[] timepos = { 0f, 0f, 1f, 1f };
        public bool timeposInterpolation = false;
        public float[] occlusionStartStop = { };
    }

    [Serializable]
    public class ExperimentConfiguration
    {
        public RoomConfiguration[] rooms = { new RoomConfiguration() };
    }

    static string ReadTextFile(string path)
    {
        return Resources.Load<TextAsset>(path).text;
    }

    public static ExperimentConfiguration LoadDefault()
    {
        return JsonUtility.FromJson<ExperimentConfiguration>(ReadTextFile("test"));
    }

    //Debug.Log(JsonUtility.FromJson<ExperimentConfiguration>(ReadTextFile("test")));
    //Debug.Log(JsonUtility.ToJson(new ExperimentConfiguration(), true));
  
}
