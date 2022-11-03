using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    [CreateAssetMenu(menuName = "Mania Map/Settings/Sample Save Settings", fileName = "SampleSaveSettings")]
    public class SampleSaveSettings : ScriptableObject
    {
        [SerializeField]
        private string _savePath = "Assets/ManiaMap/RoomTemplates/Samples";
        public string SavePath { get => _savePath; set => _savePath = value; }

        // [MenuItem("Mania Map/Save Sample Templates", priority = 1)]
        public static void SaveSampleTemplates()
        {
            GetSettings().CreateSampleTemplates();
        }

        private static SampleSaveSettings GetSettings()
        {
            var settings = Resources.Load<SampleSaveSettings>("ManiaMap/SampleSaveSettings");

            if (settings != null)
                return settings;

            settings = CreateInstance<SampleSaveSettings>();
            FileUtility.CreateDirectory("Assets/Resources/ManiaMap");
            var path = "Assets/Resources/ManiaMap/SampleSaveSettings.asset";
            AssetDatabase.CreateAsset(settings, path);
            Debug.Log($"Sample save settings created at: {path}");
            return settings;
        }

        public void CreateSampleTemplates()
        {
            CreateSamplesDirectory();

            foreach (var template in SampleVariations())
            {
                CreateRoomTemplate(template);
            }

            Log.Success("Saved sample room templates.");
        }

        private void CreateRoomTemplate(ManiaMap.RoomTemplate template)
        {
            var path = TemplateSavePath(template);
            var asset = AssetDatabase.LoadAssetAtPath<RoomTemplate>(path);

            if (asset == null)
            {
                asset = CreateInstance<RoomTemplate>();
                asset.Initialize(template);
                AssetDatabase.CreateAsset(asset, path);
            }
            else
            {
                asset.Initialize(template);
                EditorUtility.SetDirty(asset);
                // AssetDatabase.SaveAssetIfDirty(asset);
            }
        }

        private string TemplateSavePath(ManiaMap.RoomTemplate template)
        {
            var path = FileUtility.ReplaceInvalidFileNameCharacters($"{template.Name} [{template.Id:x}].asset");
            return Path.Combine(SavePath, path);
        }

        private void CreateSamplesDirectory()
        {
            FileUtility.CreateDirectory(SavePath);
        }

        /// <summary>
        /// Returns a new list of sample templates.
        /// </summary>
        private static List<ManiaMap.RoomTemplate> SampleTemplates()
        {
            return new List<ManiaMap.RoomTemplate>()
            {
                Samples.TemplateLibrary.Angles.Angle3x4(),
                Samples.TemplateLibrary.Squares.Square2x2Template(),
                Samples.TemplateLibrary.Squares.Square1x1Template(),
                Samples.TemplateLibrary.Squares.Square3x3Template(),
                Samples.TemplateLibrary.Rectangles.Rectangle2x4Template(),
                Samples.TemplateLibrary.Rectangles.Rectangle1x2Template(),
                Samples.TemplateLibrary.Rectangles.Rectangle1x4Template(),
                Samples.TemplateLibrary.Rectangles.Rectangle2x3Template(),
                Samples.TemplateLibrary.Rectangles.Rectangle1x3Template(),
                Samples.TemplateLibrary.Miscellaneous.RingTemplate(),
                Samples.TemplateLibrary.Miscellaneous.LTemplate(),
                Samples.TemplateLibrary.Miscellaneous.SquareTemplate(),
                Samples.TemplateLibrary.Miscellaneous.HyperSquareTemplate(),
                Samples.TemplateLibrary.Miscellaneous.PlusTemplate(),
                Samples.TemplateLibrary.Squares.Square1x1SavePointTemplate(),
            };
        }

        /// <summary>
        /// Returns a list of sample template unique variations.
        /// </summary>
        private static List<ManiaMap.RoomTemplate> SampleVariations()
        {
            var result = new List<ManiaMap.RoomTemplate>();
            var seed = new RandomSeed(12345);
            int templateId = 1;
            int collectableId = 1;

            foreach (var template in SampleTemplates())
            {
                foreach (var variation in template.UniqueVariations())
                {
                    var copy = new ManiaMap.RoomTemplate(templateId++, variation.Name, variation.Cells);

                    foreach (var cell in copy.Cells.Array)
                    {
                        if (seed.NextDouble() <= 0.5)
                            cell?.AddCollectableSpot(collectableId++, "Default");
                    }

                    result.Add(copy);
                }
            }

            return result;
        }
    }
}