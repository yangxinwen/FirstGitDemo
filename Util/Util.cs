﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace Util
{
    public class Util
    {
      

        /// <summary>
        /// 对象转换成json串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ObjectToJson<T>(T obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            MemoryStream stream = new MemoryStream();
            serializer.WriteObject(stream, obj);
            byte[] dataBytes = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(dataBytes, 0, (int)stream.Length);
            return Encoding.UTF8.GetString(dataBytes);
        }
        /// <summary>
        /// 从一个Json串生成对象信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static T JsonToObject<T>(string jsonString)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            MemoryStream mStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            return (T)serializer.ReadObject(mStream);
        }

        /// <summary>
        /// 保存一个对象到文件中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool SaveData<T>(T obj, string path)
        {
            try
            {
                string str = ObjectToJson<T>(obj);
                string dirPath = Path.GetDirectoryName(path);
                if (!string.IsNullOrWhiteSpace(dirPath) && !Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);

                var sw = new StreamWriter(path);
                sw.Write(str);
                sw.Close();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;

        }
        /// <summary>
        /// 从文件中读取对象内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T ReadData<T>(string path)
        {
            try
            {
                string str = string.Empty;
                var sr = new StreamReader(path);
                str = sr.ReadToEnd();
                sr.Close();

                return JsonToObject<T>(str);
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public static T FindVisualChildByName<T>(DependencyObject parent, string name) where T : DependencyObject
        {
            if (parent != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                {
                    var child = VisualTreeHelper.GetChild(parent, i) as DependencyObject;
                    string controlName = child.GetValue(Control.NameProperty) as string;
                    if (controlName == name)
                    {
                        return child as T;
                    }
                    else
                    {
                        T result = FindVisualChildByName<T>(child, name);
                        if (result != null)
                            return result;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 通过名称查找某子元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T GetChildObject<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            DependencyObject child = null;
            T grandChild = null;

            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);

                if (child is T && (((T)child).Name == name | string.IsNullOrEmpty(name)))
                {
                    return (T)child;
                }
                else
                {
                    grandChild = GetChildObject<T>(child, name);
                    if (grandChild != null)
                        return grandChild;
                }
            }
            return null;
        }

    }
}
