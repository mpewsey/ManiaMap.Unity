using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Editor
{
    public static class TemplateGroupEditor
    {
        [MenuItem("Mania Map/Save Sample Templates", priority = 0)]
        public static void SaveTemplates()
        {
            var templates = new List<RoomTemplate>()
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
                    var room = new RoomTemplate(id, variation.Name, variation.Cells);
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