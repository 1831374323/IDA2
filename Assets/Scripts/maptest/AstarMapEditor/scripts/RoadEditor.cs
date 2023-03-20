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
    Mapmanager script;//获取格子脚本
    private bool ifediting=false;//是否进入编辑模式
    public class BlockType
    {
        public string name;
        public Color color;
        public bool ifadded;
        public BlockType(Color co,string name1 = "输入想创建的名字", bool ifadded = false)
        {
            name = name1;
            color = co;
            this.ifadded = ifadded;
        }
    }
    private List<BlockType> blocks = new List<BlockType>();//创建保存每个块种类的数组
    //private bool enterplacingmode=false;
    private string mapName = "关卡名";
    private Color color;
    private string blockname = "默认文字";
    public void OnSceneGUI()
    {
        if (ifediting == false) return;
        if(Event.current.type == EventType.KeyDown)
        {
            if(Event.current.keyCode == KeyCode.C)
            {
                Event.current.Use();
                //进入单个改变模式
                Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);//射线铺路
                RaycastHit hitinfo;
                if (!Physics.Raycast(worldRay, out hitinfo)) { Debug.Log("无射线"); return; }
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
                    Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);//射线铺路
                    RaycastHit hitinfo;
                    if (!Physics.Raycast(worldRay, out hitinfo)) { Debug.Log("无射线"); return; }
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
        data.status = (data.status == sta) ? 0 : sta;//赋值
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

        GUILayout.Label("设置配置数据文件的名字");
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
            AssetDatabase.Refresh();//刷新资源视窗
            //ExportMapBitMap();
        }
        GUILayout.Label("设置要新增的块信息");
        blockname=EditorGUILayout.TextField("输入新增地图块的名字：",blockname);
        color=EditorGUILayout.ColorField("为新建地图块选择一个颜色：",color);
        BlockType blockType = new BlockType(color, blockname);//新建blockType用于赋值
        if (GUILayout.Button("Create new block type", GUILayout.Height(40))&&blockType!=null)
        {
            blocks.Add(blockType);//添加新的block类型
            blocks[blocks.Count-1].ifadded=true;
        }
        if (GUILayout.Button("Delete the last blocktype", GUILayout.Height(40))){
            if (blocks.Count != 0)
            {
                blocks.Remove(blocks[blocks.Count-1]);//删除尾部
            }
        }
        if (GUILayout.Button("导出地图", GUILayout.Height(40)))
        {
            ExportCSVmap();
            AssetDatabase.Refresh();//刷新资源视窗
        }
        for (int i = 0; i < blocks.Count; i++) {
            EditorGUILayout.LabelField("第"+(i+1).ToString()+"种额外地形的名字：", blocks[i].name);
            EditorGUILayout.LabelField("第" + (i + 1).ToString() + "种额外地形的颜色：", UnityEngine.ColorUtility.ToHtmlStringRGB(blocks[i].color));//这边需要用到类型转换，color类型转string
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
    private void ExportMapBitMap()//导出位图数据
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
    public static SceneView GetSceneView()//把view改到scene视窗
    {
        SceneView view = SceneView.lastActiveSceneView;
        if (view == null)
        {
            view=EditorWindow.GetWindow<SceneView>();
        }

        return view;
    }

    private void OnDisable()//不活跃时调用
    {
        //enterplacingmode = false;
        ifediting=false;
    }

}
