using NUnit.Framework;
using System.IO;

namespace MPewsey.ManiaMap.Unity.Tests
{
    public class TestSerialization
    {
        [SetUp]
        public void SetUp()
        {
            Assets.DestroyAllGameObjects();
            Directory.CreateDirectory("Tests");
        }

        [Test]
        public void TestSaveAndLoadLayout()
        {
            var pipeline = Assets.InstantiatePrefab<GenerationPipeline>(Assets.BigLayoutPath);
            var results = pipeline.Generate();
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
            var path = "Tests/Layout.xml";
            Serialization.SaveXml(path, layout);
            var copy = Serialization.LoadXml<Layout>(path);
            Assert.AreEqual(layout.Id, copy.Id);
        }

        [Test]
        public void TestSaveAndLoadLayoutState()
        {
            var pipeline = Assets.InstantiatePrefab<GenerationPipeline>(Assets.BigLayoutPath);
            var results = pipeline.Generate();
            var layout = (Layout)results.Outputs["Layout"];
            Assert.IsNotNull(layout);
            var state = new LayoutState(layout);
            var path = "Tests/LayoutState.xml";
            Serialization.SaveXml(path, state);
            var copy = Serialization.LoadXml<LayoutState>(path);
            Assert.AreEqual(state.Id, copy.Id);
        }
    }
}