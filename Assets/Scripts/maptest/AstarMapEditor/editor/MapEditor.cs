using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class MapEditor : EditorWindow
{
    private int mapX=12;//x方向上最多几个方块
    private int mapY=5;//y方向上最多几个方块
    private int blocksize = 3;
    [MenuItem("Tool/MapEditor")]
    static void run()//点击MapEditor即可执行的函数
    {
        EditorWindow.GetWindow<MapEditor>();//打开窗口
    }

    public void OnGUI()
    {
        GUILayout.Label("地图X方向");
        this.mapX=Convert.ToInt32(GUILayout.TextField(this.mapX.ToString()));

        GUILayout.Label("地图z方向");
        this.mapY = Convert.ToInt32(GUILayout.TextField(this.mapY.ToString()));

        GUILayout.Label("地图方块大小");
        this.blocksize = Convert.ToInt32(GUILayout.TextField(this.blocksize.ToString()));

        GUILayout.Label("选择地图原点");
        if(Selection.activeGameObject != null)
        {
            GUILayout.Label(Selection.activeGameObject.name);
        }
        else
        {
            GUILayout.Label("没有选中的UI节点，无法生成");
        }

        if (GUILayout.Button("在原点下生成地图块"))
        {
            if (Selection.activeGameObject != null)
            {
                Debug.Log("开始生成");
                this.CreateBlocksAt(Selection.activeGameObject);
                Debug.Log("生成结束");
            }
        }

        if (GUILayout.Button("重置地图块"))
        {
            if (Selection.activeGameObject != null)
            {
                this.ResetBlocksAt(Selection.activeGameObject);
            }
        }

        if (GUILayout.Button("清理地图块"))
        {
            if (Selection.activeGameObject != null)
            {
                this.ClearBlocksAt(Selection.activeGameObject);
            }
        }
    }

    private void ResetBlocksAt(GameObject gameobject)
    {
        int count = gameobject.transform.childCount;
        for (int i = 0; i < count; i++)
        {
            GameObject cube = gameobject.transform.GetChild(i).gameObject;
            cube.GetComponent<BlockData>().status = 0;
            cube.GetComponent<SpriteRenderer>().color = Color.red;
            //cube.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    private void CreateBlocksAt(GameObject gameObject)//创建方格
    {
        Mapmanager mapmanager=gameObject.GetComponent<Mapmanager>();
        if (!mapmanager)
        {
            mapmanager=gameObject.AddComponent<Mapmanager>();
        }
        mapmanager.mapX = mapX;
        mapmanager.mapY = mapY;
        mapmanager.blockSize = blocksize;
        ClearBlocksAt(gameObject);
        Vector3 startPos = new Vector3(this.blocksize * 0.5f,this.blocksize*0.5f,-2);//开始位置
        GameObject cubePre = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Perfabs/Mapeditor/square.prefab");//找到方格资源
        for (int i= 0; i < mapY; i++)
        {
            Vector3 pos = startPos;
            for(int j = 0; j < mapX; j++)
            {
                GameObject cube = PrefabUtility.InstantiatePrefab(cubePre) as GameObject;//实例化方格
                cube.transform.SetParent(gameObject.transform, false);
                cube.transform.localPosition = pos;
                cube.transform.localScale=new Vector3(blocksize,blocksize,1);
                BlockData block = cube.AddComponent<BlockData>();//为每一个块挂上脚本,并且实例化；
                block.mapX = j;
                block.mapY = i;
                block.status = 0;
                pos.x += this.blocksize;
            }
            startPos.y += this.blocksize;
        }
    }

    private void ClearBlocksAt(GameObject gameobject)//删除所有方格
    {
        int count = gameobject.transform.childCount;
        for(int i=0;i< count; i++)
        {
            GameObject cube = gameobject.transform.GetChild(0).gameObject;
            GameObject.DestroyImmediate(cube);
        }
    }
    private void OnSelectionChange()
    {
        this.Repaint();//加快刷新
    }


}
