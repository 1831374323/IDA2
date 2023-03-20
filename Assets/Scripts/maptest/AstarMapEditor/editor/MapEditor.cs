using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class MapEditor : EditorWindow
{
    private int mapX=12;//x��������༸������
    private int mapY=5;//y��������༸������
    private int blocksize = 3;
    [MenuItem("Tool/MapEditor")]
    static void run()//���MapEditor����ִ�еĺ���
    {
        EditorWindow.GetWindow<MapEditor>();//�򿪴���
    }

    public void OnGUI()
    {
        GUILayout.Label("��ͼX����");
        this.mapX=Convert.ToInt32(GUILayout.TextField(this.mapX.ToString()));

        GUILayout.Label("��ͼz����");
        this.mapY = Convert.ToInt32(GUILayout.TextField(this.mapY.ToString()));

        GUILayout.Label("��ͼ�����С");
        this.blocksize = Convert.ToInt32(GUILayout.TextField(this.blocksize.ToString()));

        GUILayout.Label("ѡ���ͼԭ��");
        if(Selection.activeGameObject != null)
        {
            GUILayout.Label(Selection.activeGameObject.name);
        }
        else
        {
            GUILayout.Label("û��ѡ�е�UI�ڵ㣬�޷�����");
        }

        if (GUILayout.Button("��ԭ�������ɵ�ͼ��"))
        {
            if (Selection.activeGameObject != null)
            {
                Debug.Log("��ʼ����");
                this.CreateBlocksAt(Selection.activeGameObject);
                Debug.Log("���ɽ���");
            }
        }

        if (GUILayout.Button("���õ�ͼ��"))
        {
            if (Selection.activeGameObject != null)
            {
                this.ResetBlocksAt(Selection.activeGameObject);
            }
        }

        if (GUILayout.Button("�����ͼ��"))
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

    private void CreateBlocksAt(GameObject gameObject)//��������
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
        Vector3 startPos = new Vector3(this.blocksize * 0.5f,this.blocksize*0.5f,-2);//��ʼλ��
        GameObject cubePre = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Perfabs/Mapeditor/square.prefab");//�ҵ�������Դ
        for (int i= 0; i < mapY; i++)
        {
            Vector3 pos = startPos;
            for(int j = 0; j < mapX; j++)
            {
                GameObject cube = PrefabUtility.InstantiatePrefab(cubePre) as GameObject;//ʵ��������
                cube.transform.SetParent(gameObject.transform, false);
                cube.transform.localPosition = pos;
                cube.transform.localScale=new Vector3(blocksize,blocksize,1);
                BlockData block = cube.AddComponent<BlockData>();//Ϊÿһ������Ͻű�,����ʵ������
                block.mapX = j;
                block.mapY = i;
                block.status = 0;
                pos.x += this.blocksize;
            }
            startPos.y += this.blocksize;
        }
    }

    private void ClearBlocksAt(GameObject gameobject)//ɾ�����з���
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
        this.Repaint();//�ӿ�ˢ��
    }


}
