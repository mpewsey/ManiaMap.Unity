using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MPewsey.ManiaMap.Unity.Tests
{
    public class TestTemplateGroup
    {
        [Test]
        public void TestGetTemplateGroupEntries()
        {
            var templates = new List<ManiaMap.RoomTemplate>
            {
                Samples.TemplateLibrary.Angles.Angle3x4(),
                Samples.TemplateLibrary.Squares.Square3x3Template(),
                Samples.TemplateLibrary.Rectangles.Rectangle2x4Template(),
            };

            var templateGroup = ScriptableObject.CreateInstance<TemplateGroup>();
            templateGroup.Name = "Test";

            foreach (var template in templates)
            {
                var room = ScriptableObject.CreateInstance<RoomTemplate>();
                room.Initialize(template);
                templateGroup.Entries.Add(new TemplateGroup.Entry(room));
            }

            var copies = templateGroup.GetEntries().ToList();

            for (int i = 0; i < templates.Count; i++)
            {
                Assert.IsTrue(ManiaMap.RoomTemplate.ValuesAreEqual(templates[i], copies[i].Template));
            }
        }
    }
}