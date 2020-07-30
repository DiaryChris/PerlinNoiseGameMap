using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class CodeLinesCalc : MonoBehaviour
{
    //在菜单中添加选项
    [MenuItem("Tools/View All Codes")]
    static void ViewAllCodes()
    {
        //将所有代码添加至code中
        List<string> code = new List<string>();
        string path = string.Format("{0}",
        @"D:\Unity workplace\Border V1.0\Assets\Scripts");
        //如果这个路径存在
        if (Directory.Exists(path))
        {
            //获取所有文件
            DirectoryInfo direction = new DirectoryInfo(path);
            FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
            
            for (int i = 0; i < files.Length; i++)
            {
                //如果是.meta文件，跳出循环
                if (files[i].Name.EndsWith(".meta"))
                {
                    continue;
                }
                //不是，则加入code中
                code.AddRange(File.ReadAllLines(files[i].FullName));
            }
        }
        Debug.Log("此项目代码共" + code.Count + "行");
    }
}
