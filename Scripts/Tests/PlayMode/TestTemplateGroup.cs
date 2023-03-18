using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Tests
{
    public class TestTemplateGroup
    {
        [Test]
        public void TestGetTemplateGroupEntries()
        {
            var templates = new List<RoomTemplate>
            {
                Samples.TemplateLibrary.Angles.Angle3x4(),
                Samples.TemplateLibrary.Squares.Square3x3Template(),
                Samples.TemplateLibrary.Rectangles.Rectangle2x4Template(),
            };

            var templateGroup = ScriptableObject.CreateInstance<TemplateGroup>();
            templateGroup.Name = "Test";

            foreach (var template in templates)
            {
                var room = ScriptableObject.CreateInstance<RoomTemplateObject>();
                room.Initialize(template);
                templateGroup.Entries.Add(new TemplateGroupEntry(room));
            }

            var copies = templateGroup.CreateData();

            for (int i = 0; i < templates.Count; i++)
            {
                Assert.IsTrue(RoomTemplate.ValuesAreEqual(templates[i], copies[i].Template));
            }
        }
    }
}