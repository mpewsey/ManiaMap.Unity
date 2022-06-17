using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Tests
{
    public class TestTemplateGroup
    {
        [Test]
        public void TestLoadTemplates()
        {
            var templates = new List<RoomTemplate>
            {
                ManiaMap.Samples.TemplateLibrary.Angles.Angle3x4(),
                ManiaMap.Samples.TemplateLibrary.Squares.Square3x3Template(),
                ManiaMap.Samples.TemplateLibrary.Rectangles.Rectangle2x4Template(),
            };

            var path = Path.GetTempFileName();
            var templateGroup = ScriptableObject.CreateInstance<TemplateGroup>();
            templateGroup.Name = "Test";

            foreach (var template in templates)
            {
                Serialization.SaveXml(path, template);
                var text = new TextAsset(File.ReadAllText(path));
                templateGroup.Templates.Add(text);
            }

            File.Delete(path);
            var copies = templateGroup.LoadTemplates().ToList();

            for (int i = 0; i < templates.Count; i++)
            {
                Assert.IsTrue(RoomTemplate.ValuesAreEqual(templates[i], copies[i]));
            }
        }
    }
}
