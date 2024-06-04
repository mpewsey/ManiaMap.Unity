using MPewsey.ManiaMap;
using MPewsey.ManiaMap.Samples;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Tests
{
    public class TestTemplateGroup
    {
        [Test]
        public void TestGetTemplateGroupEntries()
        {
            var templates = new List<RoomTemplate>
            {
                TemplateLibrary.Angles.Angle3x4(),
                TemplateLibrary.Squares.Square3x3Template(),
                TemplateLibrary.Rectangles.Rectangle2x4Template(),
            };

            var templateGroup = ScriptableObject.CreateInstance<TemplateGroup>();
            templateGroup.Name = "Test";

            foreach (var template in templates)
            {
                var room = ScriptableObject.CreateInstance<RoomTemplateObject>();
                room.Initialize(template);
                templateGroup.Entries.Add(new TemplateGroupEntry(room));
            }

            var copies = templateGroup.GetMMTemplateGroupEntries();

            for (int i = 0; i < templates.Count; i++)
            {
                Assert.IsTrue(RoomTemplate.ValuesAreEqual(templates[i], copies[i].Template));
            }
        }
    }
}