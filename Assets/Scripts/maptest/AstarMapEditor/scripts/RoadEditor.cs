using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;
using System.Data;
using SY_Tool;

[CustomEditor(typeof(Mapmanager))]
public class RoadEditor : Editor
{
    Mapmanager script;//��ȡ���ӽű�
    private bool ifediting=false;//�Ƿ����༭ģʽ
    public class BlockType
    {
        public string name;
        public Color color;
        public bool ifadded;
        public BlockType(Color co,string name1 = "�����봴��������", bool ifadded = false)
        {
            name = name1;
            color = co;
            this.ifadded = ifadded;
        }
    }
    private List<BlockType> blocks = new List<BlockType>();//��������ÿ�������������
    //private bool enterplacingmode=false;
    private string mapName = "�ؿ���";
    private Color color;
    private string blockname = "Ĭ������";
    public void OnSceneGUI()
    {
        if (ifediting == false) return;
        if(Event.current.type == EventType.KeyDown)
        {
            if(Event.current.keyCode == KeyCode.C)
            {
                Event.current.Use();
                //���뵥���ı�ģʽ
                Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);//������·
                RaycastHit hitinfo;
                if (!Physics.Raycast(worldRay, out hitinfo)) { Debug.Log("������"); return; }
                if (hitinfo.collider.gameObject.name != "square") { return; }
                //Debug.Log("change");
                changeValue(ref hitinfo);
                return;
            }

            for(int i = 0; i < blocks.Count; i++)
            {
                if (Event.current.keyCode == (KeyCode)(i+49) && blocks[i] != null && blocks[i].ifadded == true)
                {
                    Event.current.Use();
                    Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);//������·
                    RaycastHit hitinfo;
                    if (!Physics.Raycast(worldRay, out hitinfo)) { Debug.Log("������"); return; }
                    if (hitinfo.collider.gameObject.name != "square") { return; }
                    changeExtraValue(ref hitinfo, blocks[i].color,i+2);
                }
            }
            
        }
    }

    

    void changeValue(ref RaycastHit hitinfo)
    {
        BlockData data =hitinfo.collider.GetComponent<BlockData>();
        //Debug.Log("test");
        data.status = (data.status == 1) ? 0 : 1;
        if(data.status ==1)
        {
            hitinfo.collider.gameObject.GetComponent<SpriteRenderer>().color=Color.white;
        }
        else
        {
            hitinfo.collider.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    void changeExtraValue(ref RaycastHit hitinfo,Color c,int sta)
    {
        BlockData data = hitinfo.collider.GetComponent<BlockData>();
        //Debug.Log("test");
        data.status = (data.status == sta) ? 0 : sta;//��ֵ
        if (data.status == sta)
        {
            hitinfo.collider.gameObject.GetComponent<SpriteRenderer>().color = c;

            Debug.Log(c.ToString());
        }
        else
        {
            hitinfo.collider.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        DrawDefaultInspector();

        GUILayout.Label("�������������ļ�������");
        mapName = EditorGUILayout.TextField(mapName);

        script = (Mapmanager)target;
        SceneView view = GetSceneView();

        if(!ifediting&&GUILayout.Button("Start Editing", GUILayout.Height(40)))
        {
            ifediting=true;
            view.Focus();
            
        }
        //GUI.backgroundColor = Color.yellow;
        if(ifediting&&GUILayout.Button("Finish Editing", GUILayout.Height(40))) 
        { 
            ifediting=false;
            SY_CSV.Write("10", Application.dataPath + "/Resources/Maps/"+mapName+".CSV", 5, 5);
            AssetDatabase.Refresh();//ˢ����Դ�Ӵ�
            //ExportMapBitMap();
        }
        GUILayout.Label("����Ҫ�����Ŀ���Ϣ");
        blockname=EditorGUILayout.TextField("����������ͼ������֣�",blockname);
        color=EditorGUILayout.ColorField("Ϊ�½���ͼ��ѡ��һ����ɫ��",color);
        BlockType blockType = new BlockType(color, blockname);//�½�blockType���ڸ�ֵ
        if (GUILayout.Button("Create new block type", GUILayout.Height(40))&&blockType!=null)
        {
            blocks.Add(blockType);//����µ�block����
            blocks[blocks.Count-1].ifadded=true;
        }
        if (GUILayout.Button("Delete the last blocktype", GUILayout.Height(40))){
            if (blocks.Count != 0)
            {
                blocks.Remove(blocks[blocks.Count-1]);//ɾ��β��
            }
        }
        if (GUILayout.Button("������ͼ", GUILayout.Height(40)))
        {
            ExportCSVmap();
            AssetDatabase.Refresh();//ˢ����Դ�Ӵ�
        }
        for (int i = 0; i < blocks.Count; i++) {
            EditorGUILayout.LabelField("��"+(i+1).ToString()+"�ֶ�����ε����֣�", blocks[i].name);
            EditorGUILayout.LabelField("��" + (i + 1).ToString() + "�ֶ�����ε���ɫ��", UnityEngine.ColorUtility.ToHtmlStringRGB(blocks[i].color));//�����Ҫ�õ�����ת����color����תstring
        }
    }

    private void ExportCSVmap()
    {
        for (int i = 0; i < script.gameObject.transform.childCount; i++)
        {
            BlockData blockData = script.gameObject.transform.GetChild(i).GetComponent<BlockData>();
            SY_CSV.Write(blockData.status.ToString(), Application.dataPath + "/Resources/Maps/Resources/" + mapName + ".CSV", blockData.mapY, blockData.mapX);
        }
    }
    private void ExportMapBitMap()//����λͼ����
    {
        Texture2D mapTex =new Texture2D(script.mapX, script.mapY, TextureFormat.ARGB32, false);
        byte[] rawData=mapTex.GetRawTextureData();
        for (int i = 0; i < rawData.Length; i++)
        {
            rawData[i] = 0;
        }

        for (int i = 0; i < script.gameObject.transform.childCount; i++)
        {
            BlockData blockData = script.gameObject.transform.GetChild(i).GetComponent<BlockData>();
            rawData[i] = (byte)((blockData.status == 0) ? 0 : 255);
        }

        mapTex.LoadRawTextureData(rawData);
        AssetDatabase.DeleteAsset(mapName);
        AssetDatabase.CreateAsset(mapTex, mapName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    public static SceneView GetSceneView()//��view�ĵ�scene�Ӵ�
    {
        SceneView view = SceneView.lastActiveSceneView;
        if (view == null)
        {
            view=EditorWindow.GetWindow<SceneView>();
        }

        return view;
    }

    private void OnDisable()//����Ծʱ����
    {
        //enterplacingmode = false;
        ifediting=false;
    }

}
