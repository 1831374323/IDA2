using UnityEngine;
using UnityEditor;
using System.Xml;
using System.Collections.Generic;

public class MapSpawn : EditorWindow
{
    public string levelID;

    private SerializedProperty cubePty;
    private SerializedObject serObj;

    [System.Serializable]
    public struct CubeStruct
    {
        public string id;
        public GameObject cube;
    }
    [SerializeField]
    public List<CubeStruct> cubeList = new List<CubeStruct>();


    [MenuItem("Map/MapSpawn")]
    private static void ShowWindow()
    {
        EditorWindow window = GetWindow(typeof(MapSpawn));
        window.titleContent = new GUIContent("MapGenerate");
        window.Show();
    }

    private void OnEnable()
    {
        serObj = new SerializedObject(this);
        cubePty = serObj.FindProperty("cubeList");
    }

    private void OnGUI()
    {
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        levelID = EditorGUILayout.TextField("level_ID", levelID);

        EditorGUILayout.PropertyField(cubePty);
        serObj.ApplyModifiedProperties();

        if (GUILayout.Button("Spwan"))
        {
            Spawn(levelID);
        }

    }

    #region  ----地图生成----

    private List<string[]> mapData = new List<string[]>();      //数组存储地图ID
    private Transform cubes;        //地图的父物体

    void Spawn(string _levelID)
    {
        ReadXml(_levelID);
        cubes = GameObject.Find("Cubes").transform;

        int x = 0;
        int y = 0;



        foreach (string[] row in mapData)
        {
            foreach (string cubeID in row)
            {
                SpawnCube(cubeID, x, mapData.Count - y - 1);
                x++;
            }
            x = 0;
            y++;
        }
    }

    void SpawnCube(string id, int x, int y)
    {
        Debug.Log(cubeList.Count);          //如何将list载入
        foreach (var cubeStruct in cubeList)
        {
            if (cubeStruct.id == id)
            {
                Debug.Log("X: " + x + "Y: " + y + "ID :" + cubeStruct.id);
                Instantiate(cubeStruct.cube, new Vector3(x, y, 0), Quaternion.identity,cubes);
            }
        }
    }
    void ReadXml(string _levelID)                               //将XML文件载入mapData数组
    {
        mapData.Clear();

        //加载xml文件
        XmlDocument xml = new XmlDocument();
        TextAsset xmlFile = Resources.Load<TextAsset>("MapData");
        xml.LoadXml(xmlFile.text);

        XmlNodeList levelNodes = xml.SelectSingleNode("root").ChildNodes;     //一级节点 关卡节点

        foreach (XmlElement levelNode in levelNodes)
        {
            if (levelNode.GetAttribute("id") == _levelID)                    //进入关卡节点
            {
                foreach (XmlElement element in levelNode.ChildNodes)
                {
                    if (element.Name == ("data"))                           //进入地图数据节点
                    {
                        int j = 0;
                        foreach (XmlElement row in element.ChildNodes)      //逐行处理
                        {
                            string str = row.InnerText;
                            string[] spilted = str.Split(",");
                            mapData.Add(spilted);
                            j++;
                        }
                    }
                }
            }
        }
    }

    #endregion
}


