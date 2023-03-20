using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System;

public class MapManager : MonoBehaviour
{
    int mapX;
    int mapY;
    public List<string[]> mapData = new List<string[]>();
    public string levelID;
    void Start()
    {
        ReadXml("0");
        // foreach (var str in mapData)
        // {
        //     foreach (var item in str)
        //     {
        //         Debug.Log(item);
        //     }
        // }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void ReadXml(string levelID)
    {
        //加载xml文件
        XmlDocument xml = new XmlDocument();
        TextAsset xmlFile = Resources.Load<TextAsset>("MapData");
        xml.LoadXml(xmlFile.text);

        XmlNodeList levelNodes = xml.SelectSingleNode("root").ChildNodes;     //一级节点 关卡节点

        foreach (XmlElement levelNode in levelNodes)
        {
            if (levelNode.GetAttribute("id") == levelID)                    //进入关卡节点
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
}
