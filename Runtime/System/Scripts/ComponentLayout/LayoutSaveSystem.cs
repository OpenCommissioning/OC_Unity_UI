using System;
using System.Collections.Generic;
using System.IO;
using OC.Components;
using OC.Communication;
using OC.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OC.UI.ComponentLayout
{
    [DefaultExecutionOrder(-400)]
    public class LayoutSaveSystem : MonoBehaviourSingleton<LayoutSaveSystem>
    {
        public bool IsDirty => _isDirty;

        private const string FOLDER_NAME = "Saved";
        private const string FILE_EXTENSION = ".xml";
        private const string FILE_EXTENSION_FILTER = "xml";

        private bool _isDirty;

        public void MarkDirty()
        {
            _isDirty = true;
        }

        public void ClearDirty()
        {
            _isDirty = false;
        }

        public string GetLayoutsDirectory()
        {
            return Path.Combine(Application.streamingAssetsPath, FOLDER_NAME);
        }

        public string BuildDefaultFileName()
        {
            var sceneName = SanitizeFileName(SceneManager.GetActiveScene().name);
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            return $"{sceneName}_{timestamp}{FILE_EXTENSION}";
        }

        public void Save(Action<bool> onComplete = null)
        {
            EnsureLayoutsDirectory();
            var directory = GetLayoutsDirectory();
            var suggestedFileName = BuildDefaultFileName();

            var path = FileBrowser.SaveFilePanel(
                "Save Component Layout",
                directory,
                suggestedFileName,
                FILE_EXTENSION_FILTER);

            if (string.IsNullOrEmpty(path))
            {
                onComplete?.Invoke(false);
                return;
            }

            var success = SaveToFile(path);
            onComplete?.Invoke(success);
        }

        public void Load()
        {
            OpenAndImport();
        }

        public void OpenAndImport(Action<bool> onComplete = null)
        {
            EnsureLayoutsDirectory();
            var directory = GetLayoutsDirectory();

            var paths = FileBrowser.OpenFilePanel(
                "Load Component Layout",
                directory,
                FILE_EXTENSION_FILTER,
                multiselect: false);

            if (paths == null || paths.Length == 0)
            {
                onComplete?.Invoke(false);
                return;
            }

            var success = ImportFromFile(paths[0]);
            onComplete?.Invoke(success);
        }

        public bool SaveToFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return false;
            }

            if (!filePath.EndsWith(FILE_EXTENSION, StringComparison.OrdinalIgnoreCase))
            {
                filePath += FILE_EXTENSION;
            }

            try
            {
                var data = CollectLayoutData();
                ComponentLayoutXmlSerializer.Write(filePath, data);
                ClearDirty();
                Debug.Log($"[ComponentLayout] Saved {data.Entries.Count} entries to {filePath}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[ComponentLayout] Save failed: {e.Message}");
                return false;
            }
        }

        public bool ImportFromFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                Debug.LogWarning($"[ComponentLayout] File not found: {filePath}");
                return false;
            }

            if (!filePath.EndsWith(FILE_EXTENSION, StringComparison.OrdinalIgnoreCase))
            {
                Debug.LogWarning($"[ComponentLayout] Expected {FILE_EXTENSION} file: {filePath}");
                return false;
            }

            try
            {
                var data = ComponentLayoutXmlSerializer.Read(filePath);
                var activeSceneName = SceneManager.GetActiveScene().name;
                if (!string.IsNullOrEmpty(data.SceneName) && data.SceneName != activeSceneName)
                {
                    Debug.LogWarning(
                        $"[ComponentLayout] Layout scene '{data.SceneName}' differs from active scene '{activeSceneName}'.");
                }

                var lookup = BuildSceneLookup();
                var applied = 0;
                var skipped = 0;

                foreach (var entry in data.Entries)
                {
                    if (string.IsNullOrEmpty(entry.Id) || !lookup.TryGetValue(entry.Id, out var transform))
                    {
                        skipped++;
                        continue;
                    }

                    if (entry.LocalPosition != null)
                    {
                        transform.localPosition = entry.LocalPosition.ToVector3();
                    }

                    if (entry.LocalRotation != null)
                    {
                        transform.localRotation = entry.LocalRotation.ToQuaternion();
                    }

                    applied++;
                }

                ClearDirty();
                Debug.Log($"[ComponentLayout] Imported from {filePath}: applied={applied}, skipped={skipped}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[ComponentLayout] Import failed: {e.Message}");
                return false;
            }
        }

        public void TryQuit()
        {
            if (!_isDirty)
            {
                Application.Quit();
                return;
            }

            if (AppUI.Instance != null)
            {
                AppUI.Instance.ShowSaveLayoutPrompt(quitOnSuccess: true);
            }
            else
            {
                Application.Quit();
            }
        }

        private ComponentLayoutData CollectLayoutData()
        {
            var data = new ComponentLayoutData
            {
                Version = 1,
                SceneName = SceneManager.GetActiveScene().name
            };

            foreach (var sensor in FindObjectsByType<SensorBinary>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                AddEntry(data, GetEntryId(sensor.Link, sensor.name), nameof(SensorBinary), sensor.transform);
            }

            foreach (var sensor in FindObjectsByType<SensorAnalog>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                AddEntry(data, GetEntryId(sensor.Link, sensor.name), nameof(SensorAnalog), sensor.transform);
            }

            foreach (var cylinder in FindObjectsByType<Cylinder>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                AddEntry(data, GetEntryId(cylinder.Link, cylinder.name), nameof(Cylinder), cylinder.transform);
            }

            return data;
        }

        private static void AddEntry(ComponentLayoutData data, string id, string type, Transform transform)
        {
            if (string.IsNullOrEmpty(id))
            {
                return;
            }

            data.Entries.Add(new ComponentLayoutEntry
            {
                Id = id,
                Type = type,
                LocalPosition = XmlVector3.From(transform.localPosition),
                LocalRotation = XmlQuaternion.From(transform.localRotation)
            });
        }

        private static string GetEntryId(Link link, string fallbackName)
        {
            var path = link?.ClientPath;
            if (!string.IsNullOrEmpty(path))
            {
                return path;
            }

            if (!string.IsNullOrEmpty(fallbackName))
            {
                Debug.LogWarning($"[ComponentLayout] Component '{fallbackName}' has no Link.ClientPath; using GameObject name as id.");
                return fallbackName;
            }

            return null;
        }

        private static Dictionary<string, Transform> BuildSceneLookup()
        {
            var lookup = new Dictionary<string, Transform>();

            void Add(Link link, string name, Transform transform)
            {
                var id = GetEntryId(link, name);
                if (!string.IsNullOrEmpty(id) && !lookup.ContainsKey(id))
                {
                    lookup[id] = transform;
                }
            }

            foreach (var c in FindObjectsByType<SensorBinary>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                Add(c.Link, c.name, c.transform);
            }

            foreach (var c in FindObjectsByType<SensorAnalog>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                Add(c.Link, c.name, c.transform);
            }

            foreach (var c in FindObjectsByType<Cylinder>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                Add(c.Link, c.name, c.transform);
            }

            return lookup;
        }

        private void EnsureLayoutsDirectory()
        {
            var directory = GetLayoutsDirectory();
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        private static string SanitizeFileName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return "Scene";
            }

            foreach (var c in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c, '_');
            }

            return name;
        }
    }
}
