using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UnityEngine.Networking;
using System.IO;

//2020-02-13
public class GoogleDocument : EditorWindow
{
    string folderName;
    string fileName;
    string urlString;

    List<string> fileinformationList = new List<string>();
    
    List<Color> colorlist = new List<Color>();

    public Vector2 scrollPosition = Vector2.zero;

    [MenuItem("GoogleDocument/GoogleDocumentManager")]
    static void Init()
    {
        GoogleDocument googleDocument = (GoogleDocument)EditorWindow.GetWindow(typeof(GoogleDocument));
        googleDocument.Show();
    }

    private void Awake()
    {
        LoadInformationFile();
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        GUILayout.ExpandWidth(false);
        GUILayout.BeginVertical();
        GUILayout.Label("----------------------------------------------------------Add File-------------------------------------------------------------------",EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();
        GUILayout.Label("FileName", GUILayout.ExpandWidth(false)); GUILayout.Space(77);
        GUILayout.Label("FolderName", GUILayout.ExpandWidth(false)); GUILayout.Space(33);
        GUILayout.Label("URL", GUILayout.ExpandWidth(false));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        fileName = EditorGUILayout.TextField("", fileName, GUILayout.Width(130));
        GUILayout.Space(3);

        folderName = EditorGUILayout.TextField("", folderName, GUILayout.Width(100));
        GUILayout.Space(3);

        urlString = EditorGUILayout.TextField("", urlString, GUILayout.Width(475));

        if (GUILayout.Button("추가", GUILayout.Width(70)))
        {
            if (string.IsNullOrEmpty(fileName) == true)
            {
                return;
            }
            if (string.IsNullOrEmpty(urlString) == true)
            {
                return;
            }

            string saveString = string.Format("{0},{1},{2}" ,fileName ,folderName ,urlString);

            folderName = null;
            fileName = null;
            urlString = null;

            fileinformationList.Add(saveString);
            colorlist.Add(Color.white);
            SaveInformationFile();
        }

        GUILayout.EndHorizontal();
        GUILayout.Space(15);
        GUILayout.Label("Download Path : Assets/Resources/ + FolderName");
        GUILayout.Space(15);
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        GUILayout.Label("----------------------------------------------------------File List--------------------------------------------------------------------", EditorStyles.boldLabel);
        
        int docCount = fileinformationList.Count;

        GUILayout.BeginHorizontal();
        GUILayout.Space(88);
        GUILayout.Label("FileName", GUILayout.ExpandWidth(false)); GUILayout.Space(77);
        GUILayout.Label("FolderName", GUILayout.ExpandWidth(false)); GUILayout.Space(33);
        GUILayout.Label("URL", GUILayout.ExpandWidth(false));
        GUILayout.EndHorizontal();

        for (int index = 0; index < docCount; index++)
        {
            string[] splitString = fileinformationList[index].Split(',');

            string inst_filename = splitString[0];
            string inst_foldername = splitString[1];
            string inst_url = splitString[2];

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Download", GUILayout.Width(70)))
            {
                DownloadFileOnURL(index);
                return;
            }

            EditorGUILayout.TextField("", inst_filename, GUILayout.Width(130));
            GUILayout.Space(3);

            EditorGUILayout.TextField("", inst_foldername, GUILayout.Width(100));
            GUILayout.Space(3);

            EditorGUILayout.TextField("", inst_url, GUILayout.Width(400));

            GUI.backgroundColor = colorlist[index];

            if (GUILayout.Button("Delete",GUILayout.Width(70)))
            {                
                if(colorlist[index] == Color.white)
                {
                    colorlist[index] = Color.red;
                }
                else
                {
                    colorlist.RemoveAt(index);
                    fileinformationList.RemoveAt(index);
                    SaveInformationFile();
                    break;
                }
            }
            GUI.backgroundColor = Color.white;

            GUILayout.EndHorizontal();
            GUILayout.Space(5);
        }
        GUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
    }

    void SaveInformationFile()
    {
        string InformationFilePath = Path.Combine(Application.dataPath + "/Editor/", "GoogleDocumentList.txt");
        StreamWriter writer = new StreamWriter(InformationFilePath, false);

        int docCount = fileinformationList.Count;

        for(int index = 0; index < docCount; index++)
        {
            writer.Write(fileinformationList[index]+';');
        }
        writer.Close();

        AssetDatabase.Refresh();
    }
    void LoadInformationFile()
    {
        string InformationFilePath = Path.Combine(Application.dataPath + "/Editor/", "GoogleDocumentList.txt");
       
        try
        {       
            using (StreamReader reader = new StreamReader(InformationFilePath))
            {
                string[] fileinformationArray = reader.ReadToEnd().Split(';');

                fileinformationList.Clear();

                int docCount = fileinformationArray.Length - 1;
                for (int index = 0; index < docCount; index++)
                {
                    colorlist.Add(Color.white);
                    fileinformationList.Add(fileinformationArray[index]);
                }
                reader.Close();
            }
        }     
        catch(System.Exception e)
        {
            Debug.LogException(e);
        }        
    }
    
    void DownloadFileOnURL(int index)
    {
        string[] splitString = fileinformationList[index].Split(',');
        string inst_filename = splitString[0];
        string inst_foldername = splitString[1];
        if(string.IsNullOrEmpty(inst_foldername) == false)
        {
            inst_foldername += "/";
        }
        string inst_url = splitString[2];
        
        UnityWebRequest request = new UnityWebRequest(inst_url, UnityWebRequest.kHttpVerbGET);
        string path = Path.Combine(Application.dataPath + "/Resources/", inst_foldername + inst_filename);

        request.downloadHandler = new DownloadHandlerFile(path);
        request.SendWebRequest();

        while (true)
        {
            if (request.isDone == true)
            {
                break;
            }
        }

        if (request.error != null)
        {
            Debug.LogError(request.error);
        }
        else
        {
            Debug.Log("파일 다운로드 완료 : " + path);
            AssetDatabase.Refresh();
        }
    }
}