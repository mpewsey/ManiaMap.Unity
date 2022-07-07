using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    /// <summary>
    /// The TemplateGroup editor.
    /// </summary>
    public static class TemplateGroupEditor
    {
        /// <summary>
        /// Saves the sample room templates from the Mania Map library to the
        /// ManiaMap/RoomTemplates/Samples directory of the project.
        /// </summary>
        [MenuItem("Mania Map/Save Sample Templates", priority = 0)]
        public static void SaveTemplates()
        {
            var templates = new List<ManiaMap.RoomTemplate>()
            {
                ManiaMap.Samples.TemplateLibrary.Angles.Angle3x4(),
                ManiaMap.Samples.TemplateLibrary.Squares.Square2x2Template(),
                ManiaMap.Samples.TemplateLibrary.Squares.Square1x1Template(),
                ManiaMap.Samples.TemplateLibrary.Squares.Square3x3Template(),
                ManiaMap.Samples.TemplateLibrary.Rectangles.Rectangle2x4Template(),
                ManiaMap.Samples.TemplateLibrary.Rectangles.Rectangle1x2Template(),
                ManiaMap.Samples.TemplateLibrary.Rectangles.Rectangle1x4Template(),
                ManiaMap.Samples.TemplateLibrary.Rectangles.Rectangle2x3Template(),
                ManiaMap.Samples.TemplateLibrary.Rectangles.Rectangle1x3Template(),
                ManiaMap.Samples.TemplateLibrary.Miscellaneous.RingTemplate(),
                ManiaMap.Samples.TemplateLibrary.Miscellaneous.LTemplate(),
                ManiaMap.Samples.TemplateLibrary.Miscellaneous.SquareTemplate(),
                ManiaMap.Samples.TemplateLibrary.Miscellaneous.HyperSquareTemplate(),
                ManiaMap.Samples.TemplateLibrary.Miscellaneous.PlusTemplate(),
            };

            Random.InitState(12345);
            var root = Path.Combine("Assets", "ManiaMap", "RoomTemplates", "Samples");
            FileUtility.CreateDirectory(root);
            int k = 0;

            foreach (var template in templates)
            {
                foreach (var variation in template.UniqueVariations())
                {
                    var id = Random.Range(1, int.MaxValue);
                    var room = new ManiaMap.RoomTemplate(id, variation.Name, variation.Cells);
                    var path = Path.Combine(root, $"{room.Name}_{room.Id}.xml");

                    foreach (var cell in room.Cells.Array)
                    {
                        if (cell != null)
                            cell.AddCollectableSpot(k++, "Default");
                    }

                    Serialization.SavePrettyXml(path, room);
                }
            }

            AssetDatabase.Refresh();
        }
    }
}