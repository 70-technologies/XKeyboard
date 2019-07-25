/// XConfig - Application Configuration Management
/// XConfig uses a class database to store any TYPE of data. The class is then serialized either by using XSerializer (a serializer
/// based on XmlSerialization) or BSerializer (Binary serializer). 

/* 
Copyright (c) 2019, All rights are reserved by AB, 70 Technologies. 
https://www.Tech70.cf
This program is licensed under the Apache License, Version 2.0 (the "License");
you may not download, install or use this program except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
*/ 

using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;

namespace XConfig
{
    public class ConfigManager
    {
        /// <summary>
        /// Indicates whether to serialize the database class after setting a new value or removing.
        /// </summary>
        public bool AutoSave { get; set; }
        /// <summary>
        /// The name of the output file. 
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// The Serializer to use. 
        /// </summary>
        public ISerializer Serializer { get; private set; }
        /// <summary>
        /// The underlying database class used to store data.
        /// </summary>
        public ConfigDatabase Database { get; private set; }
        /// <summary>
        /// Creates a new instance of ConfigManager with specified output file and serializer.
        /// </summary>
        /// <param name="fileName">Fullname of output file. </param>
        /// <param name="serializer">The serializer to use. </param>
        /// <param name="autoSave">Indicates whether to save automatically on adding or removing new data. </param>
        public ConfigManager(string fileName, ISerializer serializer, bool autoSave = true)
        {
            this.AutoSave = autoSave;
            this.FileName = fileName;
            this.Serializer = serializer;

            Init();
        }
        private void Init()
        {
            //If File exists, deserialize it. Otherwise create new instance. 
            if (File.Exists(this.FileName))
            {
                //Deserialize the file. 
                this.Database = this.Serializer.Deserialize<ConfigDatabase>(this.FileName);
                if (this.Database == null)
                    this.Database = new ConfigDatabase();
            }
            else
                this.Database = new ConfigDatabase();
        }
        /// <summary>
        /// Returns a ConfigObject from database using specified key. To get the object data, use ConfigManager.Get() instead. 
        /// </summary>
        /// <param name="key">The key of the config object.</param>
        /// <returns></returns>
        public ConfigObject this[string key]
        {
            get { return this.Database.Get(key); }
        }
        /// <summary>
        /// Returns the object data of a config object with speicied key. 
        /// </summary>
        /// <typeparam name="T">The type of the object. </typeparam>
        /// <param name="key">The key of the object in database.</param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            if (Database.Contains(key))
                return (T)(Database.Get(key).Object);
            return default(T);
        }
        /// <summary>
        /// Sets or adds a an object in the database with specified key and value. 
        /// </summary>
        /// <typeparam name="T">The type of the object. </typeparam>
        /// <param name="key">THe key for the object. Must be unique.</param>
        /// <param name="value">Value</param>
        public void Set<T>(string key, T value)
        {
            ConfigObject co = new ConfigObject() { Key = key, Object = value, Type = typeof(T) };
            this.Database.Set(co, true);
            if (AutoSave) Save();
        }
        /// <summary>
        /// Returns true if the database contains any object with specified key. 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Contains(string key) => this.Database.Contains(key);
        /// <summary>
        /// Removes an object from the database. 
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            this.Database.Remove(key);
            if (AutoSave) Save();
        }
        /// <summary>
        /// Saves (Serializes) the database. 
        /// </summary>
        public void Save()
        {
            this.Serializer.Serialize<ConfigDatabase>(this.Database, this.FileName);
        }
    }
    [Serializable]
    public class ConfigDatabase
    {
        public List<ConfigObject> Objects;
        public ConfigDatabase()
        {
            this.Objects = new List<ConfigObject>();
        }
        public ConfigObject Get(string key)
        {
            //Return object from objects, if it doesn't exist, throw error. 
            foreach (var obj in Objects)
            {
                if (obj.Key == key)
                    return obj;
            }
            throw new KeyNotFoundException("The specified key doesn't exist in the database. ");
        }
        public void Set(ConfigObject obj, bool overWrite = false)
        {
            //Store the object in list. If it already exists, throw error if overWrite is false.
            if (Contains(obj.Key) && overWrite == false)
                throw new Exception("An object with same key already exists. ");
            if (Contains(obj.Key) && overWrite)
                Remove(obj.Key);
            Objects.Add(obj);
        }
        public bool Contains(string key)
        {
            //Check if the list contains object with specified key. If it does, assign it to obj param and return true.
            foreach (var item in Objects)
            {
                if (item.Key == key)
                {
                    return true;
                }
            }
            return false;
        }
        public void Remove(string key)
        {
            //Remove an object from the list with specified key. 
            if (Contains(key))
            {
                for (int i = 0; i < Objects.Count; i++)
                {
                    if (Objects[i].Key == key)
                        Objects.RemoveAt(i);
                }
            }
        }
    }
    [Serializable]
    public class ConfigObject
    {
        public string Key;
        public object Object;
        public Type Type;
    }
    public interface ISerializer
    {
        /// <summary>
        /// Serializes an object of type T using specified Serializer. 
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize. </typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="outputFile">The output file.</param>
        void Serialize<T>(T obj, string outputFile);
        /// <summary>
        /// Deserializes a file into a .NET Object, casts it to specified type and returns. 
        /// </summary>
        /// <typeparam name="T">The expected type of the object to deserialize. </typeparam>
        /// <param name="inputFile">The input file. </param>
        T Deserialize<T>(string inputFile);
        /// <summary>
        /// Tries to deserialize a file. If any error occures, returns false. Otherwise sets outputObject to the deserialized object. 
        /// </summary>
        /// <typeparam name="T">The expected type of the object to deserialize.</typeparam>
        /// <param name="inputFile">The input file. </param>
        /// <param name="outputObject">Deserialized object. </param>
        /// <returns></returns>
        bool TryDeserialize<T>(string inputFile, out T outputObject) where T : class;
    }
    /// <summary>
    /// Used to serialize any ISerializable class using XmlSerialization.
    /// </summary>
    public class XSerializer : ISerializer
    {
        public void Serialize<T>(T obj, string outputFile)
        {
            XmlSerializer xz = new XmlSerializer(typeof(T), "XKeyboard");
            XmlWriter xw = XmlWriter.Create(outputFile, new XmlWriterSettings()
            {
                CloseOutput = true,
                Encoding = Encoding.Unicode,
                Indent = true,
                IndentChars = "    ",
                NewLineHandling = NewLineHandling.Entitize,
                WriteEndDocumentOnClose = true
            });
            xz.Serialize(xw, obj);
            xw.Close();
        }
        public T Deserialize<T>(string inputFile)
        {
            if (File.Exists(inputFile) == false)
                throw new FileNotFoundException(inputFile);
            XmlSerializer xz = new XmlSerializer(typeof(T), "XKeyboard");
            XmlReader xr = XmlReader.Create(inputFile);
            var x = (T)xz.Deserialize(xr);
            xr.Close();
            return x;
        }
        public bool TryDeserialize<T>(string inputFile, out T outputObject) where T : class
        {
            outputObject = null;
            try
            {
                outputObject = Deserialize<T>(inputFile);
                return true;
            }
            catch (Exception)
            {
            }
            return false;
        }
    }
    /// <summary>
    /// Used to serialize any ISerializable class using BinaryFormatter. 
    /// </summary>
    public class BSerializer : ISerializer
    {
        public void Serialize<T>(T obj, string outputFile)
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
            bf.TypeFormat = System.Runtime.Serialization.Formatters.FormatterTypeStyle.TypesAlways;

            using (FileStream fs = new FileStream(outputFile, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                bf.Serialize(fs, obj);
            }
        }
        public T Deserialize<T>(string inputFile)
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
            bf.TypeFormat = System.Runtime.Serialization.Formatters.FormatterTypeStyle.TypesAlways;

            using (FileStream fs = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
            {
                return (T)bf.Deserialize(fs);
            }
        }
        public bool TryDeserialize<T>(string inputFile, out T outputObject) where T : class
        {
            outputObject = null;
            try
            {
                outputObject = (T)Deserialize<T>(inputFile);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
    /// <summary>
    /// Thrown when the specified key doesn't exist in the dictionary. 
    /// </summary>
    public class KeyNotFoundException : Exception
    {
        public KeyNotFoundException(string msg) : base(msg)
        {

        }
    }
}