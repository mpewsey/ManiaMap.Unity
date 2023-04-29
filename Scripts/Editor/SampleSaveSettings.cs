using MPewsey.Common.Mathematics;
using MPewsey.Common.Random;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    /// <summary>
    /// Contains settings related to saving the samples room templates from the Mania Map library to the project.
    /// </summary>
    [CreateAssetMenu(menuName = "Mania Map/Settings/Sample Save Settings", fileName = "SampleSaveSettings")]
    public class SampleSaveSettings : ScriptableObject
    {
        [SerializeField]
        private string _savePath = "Assets/ManiaMap/RoomTemplates/Samples";
        /// <summary>
        /// The path where the sample templates will be saved.
        /// </summary>
        public string SavePath { get => _savePath; set => _savePath = value; }

        /// <summary>
        /// Creates or overwrites the existing sample templates within the project.
        /// </summary>
        public void CreateSampleTemplates()
        {
            CreateSamplesDirectory();

            foreach (var template in SampleVariations())
            {
                CreateRoomTemplate(template);
            }

            Log.Success("Saved sample room templates.");
        }

        /// <summary>
        /// Creates or overwrites the existing room template asset with the data for the specified
        /// generation template.
        /// </summary>
        /// <param name="template">The generation room template.</param>
        private void CreateRoomTemplate(RoomTemplate template)
        {
            var path = TemplateSavePath(template);
            var asset = AssetDatabase.LoadAssetAtPath<RoomTemplateObject>(path);

            if (asset == null)
            {
                asset = CreateInstance<RoomTemplateObject>();
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

        /// <summary>
        /// Returns the save path for the specified room template.
        /// </summary>
        /// <param name="template">The generation room template.</param>
        private string TemplateSavePath(RoomTemplate template)
        {
            var path = FileUtility.ReplaceInvalidFileNameCharacters($"{template.Name} [{template.Id:x}].asset");
            return Path.Combine(SavePath, path);
        }

        /// <summary>
        /// Creates the sample directory within the project.
        /// </summary>
        private void CreateSamplesDirectory()
        {
            FileUtility.CreateDirectory(SavePath);
        }

        /// <summary>
        /// Returns a new list of sample templates.
        /// </summary>
        private static List<RoomTemplate> SampleTemplates()
        {
            return new List<RoomTemplate>()
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
        private static List<RoomTemplate> SampleVariations()
        {
            var result = new List<RoomTemplate>();
            var seed = new RandomSeed(12345);
            int templateId = 1;
            int collectableId = 1;

            foreach (var template in SampleTemplates())
            {
                foreach (var variation in template.UniqueVariations())
                {
                    var copy = new RoomTemplate(templateId++, variation.Name, variation.Cells);

                    for (int i = 0; i < variation.Cells.Rows; i++)
                    {
                        for (int j = 0; j < variation.Cells.Columns; j++)
                        {
                            if (seed.ChanceSatisfied(0.5))
                                copy.AddCollectableSpot(collectableId++, new Vector2DInt(i, j), "Default");
                        }
                    }

                    result.Add(copy);
                }
            }

            return result;
        }
    }
}