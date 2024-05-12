using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace Endless.Inventory.Items
{
    public class ItemDataBase : MonoBehaviour
    {
        public static Dictionary<int, ItemObject> itemObjects = new Dictionary<int, ItemObject>();
        public static Dictionary<ItemObject, int> itemObjectsIndex = new Dictionary<ItemObject, int>();

        private static string path = "Assets/Resources/Definitive Inventory System";

        private void OnEnable()
        {
            //itemObjects.Clear();
            Save();
            Debug.Log("databaseCleared");
        }

        public static void AddItem(ItemObject itemObject)
        {
            //UnityEngine.Debug.Log("Item : ID - " + itemObject.data.Id + " Name - " + itemObject.name);
            if (!itemObjects.ContainsValue(itemObject))
            {
                itemObject.data.Id = itemObjects.Count;
                itemObjectsIndex.Add(itemObject, itemObjects.Count);
                itemObjects.Add(itemObjects.Count, itemObject);
                Debug.Log("Item : ID - " + itemObject.data.Id + " Name - " + itemObject.itemName);

                Save();

                return;
            }
            else if(itemObjects.ContainsValue(itemObject))
            {
                if (itemObject.data.Id == itemObjectsIndex[itemObject])
                {
                    Debug.Log("Item : ID - " + itemObject.data.Id + " Name - " + itemObject.itemName + "Updated");
                    itemObjects[itemObjectsIndex[itemObject]] = itemObject;
                    Save();
                }
            }
        }

        public static void UpdateItem(ItemObject itemObject)
        {
            if (itemObject.data.Id >= 0)
            {
                itemObjects[itemObject.data.Id] = itemObject;
                Save();
            }
            else if (itemObjectsIndex[itemObject] >= 0)
            {
                itemObject.data.Id = itemObjectsIndex[itemObject];
                itemObjects[itemObjectsIndex[itemObject]] = itemObject;
                Save();
            }
            else
            {
                Debug.Log("No se encontro el item: \n" + itemObject.Name +"\nID: "+ itemObject.data.Id + " \nNombre de archivo: " + itemObject.name + " \nen la base de datos");
                itemObject.data.Id = itemObjects.Count;
                itemObjectsIndex.Add(itemObject, itemObjects.Count);
                itemObjects.Add(itemObjects.Count, itemObject);
                Debug.Log("Ha sido añadido como: Item : ID - " + itemObject.data.Id + " Name - " + itemObject.itemName);

                Save();
            }
        }

        private class ItemDatabaseSaveObject
        {
            public Dictionary<int, ItemObject> itemObjectsDictionary = new Dictionary<int, ItemObject>();
            public Dictionary<ItemObject, int> itemObjectsIndexDictionary = new Dictionary<ItemObject, int>();

            public List<ItemObject> itemObjectsSaved = new List<ItemObject>();
            public List<int> itemObjectsIndexSaved = new List<int>();

            public ItemDatabaseSaveObject(Dictionary<int, ItemObject> itemObjectsToSave, Dictionary<ItemObject, int> itemObjectsIndexToSave)
            {
                itemObjectsDictionary = itemObjectsToSave;
                itemObjectsIndexDictionary = itemObjectsIndexToSave;

                for (int i = 0; i < itemObjectsDictionary.Count; i++)
                {
                    if (itemObjectsDictionary[i] is ItemObject and not null)
                    {
                        itemObjectsSaved.Add(itemObjectsDictionary[i]);
                        itemObjectsIndexSaved.Add(itemObjectsIndexDictionary[itemObjectsDictionary[i]]);
                    }
                }
            }
        }
        public static Dictionary<int, ItemObject> Reflect()
        {
            return itemObjects;
        }

        [InitializeOnLoadMethod]
        private static void OnEditorOpen()
        {
            if (!SessionState.GetBool("FirstInitDone", false))
            {
                Load();

                SessionState.SetBool("FirstInitDone", true);
            }
        }

        private static void Load()
        {
            if (Directory.Exists(path))
            {
                if (File.Exists(path + "/Database.txt"))
                {
                    // reads data from the json file into the jsonString
                    string jsonString = File.ReadAllText(path + "/Database.txt");
                    UnityEngine.Debug.Log("Loaded: " + jsonString);

                    // transforms the jsonString into an object
                    ItemDatabaseSaveObject data = JsonUtility.FromJson<ItemDatabaseSaveObject>(jsonString);

                    // sets the values of this object to those of the file

                    Dictionary<int, ItemObject> loadedObjects = new Dictionary<int, ItemObject>();
                    Dictionary<ItemObject, int> loadedObjectsIndex = new Dictionary<ItemObject, int>();

                    for (int i = 0; i < data.itemObjectsSaved.Count; i++)
                    {
                        loadedObjects.Add(data.itemObjectsIndexSaved[i], data.itemObjectsSaved[i]);
                        loadedObjectsIndex.Add(data.itemObjectsSaved[i], data.itemObjectsIndexSaved[i]);
                    }

                    itemObjects = loadedObjects;
                    itemObjectsIndex = loadedObjectsIndex;
                }
                else
                {
                    itemObjects = new Dictionary<int, ItemObject>();
                    itemObjectsIndex = new Dictionary<ItemObject, int>();
                }
            }
        }



        public static void Save()
        {
            ItemDatabaseSaveObject dataFile = new ItemDatabaseSaveObject(itemObjects, itemObjectsIndex);

            // transforms the data into text
            string jsonString = JsonUtility.ToJson(dataFile);

            // writes a file with all the data in it
            if(Directory.Exists(path))
                File.WriteAllText(path + "/Database.txt", jsonString);
            else
            {
                Directory.CreateDirectory(path);
                File.WriteAllText(path + "/Database.txt", jsonString);
            }

            Debug.Log("Saving to " + path + "/Database.txt");
        }
    }
}
