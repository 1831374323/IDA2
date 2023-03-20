using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using SY_Tool;
using System.Security.Cryptography;
using UnityEditor;
using System;

public class MapGenerator : MonoBehaviour
{
    public int a;
    public int b;
    public int blocksize;
    DataTable datatable1;
    
    // Start is called before the first frame update
    void Start()
    {
        datatable1=SY_CSV.ReadFromResources("�ؿ�3",false);
        CreateBlocksAt(gameObject);
        //Debug.Log(datatable1.Rows.Count.ToString());
        //Debug.Log(Convert.ToInt32(datatable1.Rows[2][3]));
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private void ChangeMapValue(int _id,GameObject gameObject2)//�ı�ؿ��sprite,δ�����
    {
        if (Resources.Load<Sprite>("MapBlocks/Test_" + _id) != null)
        {
            Debug.Log("success");
            gameObject2.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("MapBlocks/Test_" + _id);
        }
        else
        {
            //Debug.Log("success");
            gameObject2.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("MapBlocks/Test_" + 0);
        }
    }

    private void CreateBlocksAt(GameObject gameObject)//��������
    {
        int mapx=datatable1.Rows.Count-1;
        int mapy=datatable1.Columns.Count;
        Vector3 startPos = new Vector3(this.blocksize * 0.5f, this.blocksize * 0.5f, -2);//��ʼλ��
        GameObject cubePre = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Perfabs/Mapeditor/square.prefab");//�ҵ�������Դ
        for (int i = 1; i <= mapx; i++)
        {
            Vector3 pos = startPos;
            for (int j = 0; j < mapy; j++)
            {
                GameObject cube = PrefabUtility.InstantiatePrefab(cubePre) as GameObject;//ʵ��������
                cube.transform.SetParent(gameObject.transform, false);
                cube.transform.localPosition = pos;
                cube.transform.localScale = new Vector3(blocksize, blocksize, 1);
                BlockData block = cube.AddComponent<BlockData>();//Ϊÿһ������Ͻű�,����ʵ����,�������ػ�����νű�
                ChangeMapValue(Convert.ToInt32(datatable1.Rows[i][j]),cube);
                pos.x += this.blocksize;
            }
            startPos.y += this.blocksize;
        }
    }
}
