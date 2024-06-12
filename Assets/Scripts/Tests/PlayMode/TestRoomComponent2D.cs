using MPewsey.ManiaMap;
using MPewsey.ManiaMapUnity.Generators;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.TestTools;

namespace MPewsey.ManiaMapUnity.Tests
{
    public class TestRoomComponent2D
    {
        private const int CellLayer = 20;
        private const int TriggeringLayer = 21;
        private const string TestScene = "TestRoomComponent2D";
        private RoomComponent Room { get; set; }
        private LayoutPack LayoutPack { get; set; }

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            var settings = ScriptableObject.CreateInstance<ManiaMapSettings>();
            settings.CellLayer = CellLayer;
            settings.TriggeringLayers = 1 << TriggeringLayer;

            yield return Addressables.LoadSceneAsync(TestScene);
            var handle = Addressables.LoadAssetAsync<RoomTemplateDatabase>("2DRoomTemplateDatabase");
            yield return handle;
            var database = handle.Result;
            Assert.IsTrue(database != null);
            var pipeline = Object.FindAnyObjectByType<GenerationPipeline>();
            Assert.IsTrue(pipeline != null);
            var results = pipeline.Run();
            Assert.IsTrue(results.Success);
            var layout = results.GetOutput<Layout>("Layout");
            Assert.IsNotNull(layout);
            var roomId = layout.Rooms.Keys.First();
            LayoutPack = new LayoutPack(layout, new LayoutState(layout), settings);
            Room = database.InstantiateRoom(roomId, LayoutPack);
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            yield return Addressables.LoadSceneAsync("EmptyScene");
        }

        [UnityTest]
        public IEnumerator TestOnCellAreaEnteredAndExited()
        {
            var area = new GameObject("Area Tester");
            area.transform.position = new Vector2(10000, 10000);
            area.layer = TriggeringLayer;
            area.AddComponent<Rigidbody2D>();
            var collider = area.AddComponent<BoxCollider2D>();
            collider.size = Vector2.one;

            var indexes = new List<Vector2Int>();
            Room.OnCellAreaEntered.AddListener((area, collision) => indexes.Add(new Vector2Int(area.Row, area.Column)));
            Room.OnCellAreaExited.AddListener((area, collision) => indexes.Remove(new Vector2Int(area.Row, area.Column)));

            // Test area should begin with no collisions
            yield return new WaitForFixedUpdate();
            Assert.AreEqual(0, indexes.Count);

            // Move test area to internal cell corners
            for (int i = 1; i < Room.Rows; i++)
            {
                for (int j = 1; j < Room.Columns; j++)
                {
                    area.transform.position = new Vector2(j * Room.CellSize.x, -i * Room.CellSize.y);
                    yield return new WaitForFixedUpdate();
                    Assert.AreEqual(4, indexes.Count);
                    Assert.IsTrue(indexes.Contains(new Vector2Int(i, j)));
                    Assert.IsTrue(indexes.Contains(new Vector2Int(i - 1, j)));
                    Assert.IsTrue(indexes.Contains(new Vector2Int(i, j - 1)));
                    Assert.IsTrue(indexes.Contains(new Vector2Int(i - 1, j - 1)));
                }
            }
        }

        [Test]
        public void TestCellIndexExists()
        {
            Assert.IsTrue(Room.CellIndexExists(0, 0));
            Assert.IsTrue(Room.CellIndexExists(1, 0));
            Assert.IsTrue(Room.CellIndexExists(0, 2));
            Assert.IsFalse(Room.CellIndexExists(-1, 0));
            Assert.IsFalse(Room.CellIndexExists(0, -1));
            Assert.IsFalse(Room.CellIndexExists(3, 0));
            Assert.IsFalse(Room.CellIndexExists(0, 3));
        }

        [Test]
        public void TestCellCenterLocalPosition()
        {
            Room.CellSize = new Vector3(100, 100, 100);
            Room.transform.position = new Vector3(543, 9084, 234);

            Assert.AreEqual(new Vector3(50, -50, -50), Room.CellCenterLocalPosition(0, 0));
            Assert.AreEqual(new Vector3(50, -150, -50), Room.CellCenterLocalPosition(1, 0));
            Assert.AreEqual(new Vector3(50, -250, -50), Room.CellCenterLocalPosition(2, 0));
            Assert.AreEqual(new Vector3(150, -50, -50), Room.CellCenterLocalPosition(0, 1));
            Assert.AreEqual(new Vector3(250, -50, -50), Room.CellCenterLocalPosition(0, 2));
            Assert.AreEqual(new Vector3(150, -150, -50), Room.CellCenterLocalPosition(1, 1));
            Assert.AreEqual(new Vector3(250, -250, -50), Room.CellCenterLocalPosition(2, 2));
        }

        [Test]
        public void TestCellCenterGlobalPosition()
        {
            var offset = new Vector3(543, 9084, 234);
            Room.CellSize = new Vector3(100, 100, 100);
            Room.transform.position = offset;

            Assert.AreEqual(new Vector3(50, -50, -50) + offset, Room.CellCenterGlobalPosition(0, 0));
            Assert.AreEqual(new Vector3(50, -150, -50) + offset, Room.CellCenterGlobalPosition(1, 0));
            Assert.AreEqual(new Vector3(50, -250, -50) + offset, Room.CellCenterGlobalPosition(2, 0));
            Assert.AreEqual(new Vector3(150, -50, -50) + offset, Room.CellCenterGlobalPosition(0, 1));
            Assert.AreEqual(new Vector3(250, -50, -50) + offset, Room.CellCenterGlobalPosition(0, 2));
            Assert.AreEqual(new Vector3(150, -150, -50) + offset, Room.CellCenterGlobalPosition(1, 1));
            Assert.AreEqual(new Vector3(250, -250, -50) + offset, Room.CellCenterGlobalPosition(2, 2));
        }

        [Test]
        public void TestGlobalPositionToCellIndex()
        {
            var offset = new Vector3(543, 9084, 234);
            Room.CellSize = new Vector3(100, 100, 100);
            Room.transform.position = offset;

            for (int i = 0; i < Room.Rows; i++)
            {
                for (int j = 0; j < Room.Columns; j++)
                {
                    var position = new Vector3(j * Room.CellSize.x, -i * Room.CellSize.y, 0)
                        + offset + 0.5f * new Vector3(Room.CellSize.x, -Room.CellSize.y, Room.CellSize.x);
                    Assert.AreEqual(new Vector2Int(i, j), Room.GlobalPositionToCellIndex(position));
                }
            }

            Assert.AreEqual(new Vector2Int(-1, -1), Room.GlobalPositionToCellIndex(Vector3.zero));
        }

        [Test]
        public void TestLocalPositionToCellIndex()
        {
            var offset = new Vector3(543, 9084, 234);
            Room.CellSize = new Vector3(100, 100, 100);
            Room.transform.position = offset;

            for (int i = 0; i < Room.Rows; i++)
            {
                for (int j = 0; j < Room.Columns; j++)
                {
                    var position = new Vector3(j * Room.CellSize.x, -i * Room.CellSize.y, 0)
                        + 0.5f * new Vector3(Room.CellSize.x, -Room.CellSize.y, Room.CellSize.x);
                    Assert.AreEqual(new Vector2Int(i, j), Room.LocalPositionToCellIndex(position));
                }
            }

            Assert.AreEqual(new Vector2Int(-1, -1), Room.LocalPositionToCellIndex(new Vector3(-100, -100, -100)));
        }

        [Test]
        public void TestFindClosestDoorDirection()
        {
            Room.CellSize = new Vector3(100, 100, 100);

            Assert.AreEqual(DoorDirection.North, Room.FindClosestDoorDirection(0, 0, new Vector3(50, 0, 50)));
            Assert.AreEqual(DoorDirection.West, Room.FindClosestDoorDirection(0, 0, new Vector3(0, -50, 50)));
            Assert.AreEqual(DoorDirection.South, Room.FindClosestDoorDirection(0, 0, new Vector3(50, -100, 50)));
            Assert.AreEqual(DoorDirection.East, Room.FindClosestDoorDirection(0, 0, new Vector3(100, -50, 50)));
        }

        [Test]
        public void TestFindClosestActiveCellIndex()
        {
            Room.CellSize = new Vector3(100, 100, 100);
            Room.SetCellActivities(new Vector2Int(1, 1), new Vector2Int(2, 2), CellActivity.Deactivate);

            Assert.AreEqual(Vector2Int.zero, Room.FindClosestActiveCellIndex(Vector3.zero));
            Assert.AreEqual(new Vector2Int(0, 1), Room.FindClosestActiveCellIndex(new Vector3(125, -50, 1000)));
            Assert.AreEqual(new Vector2Int(1, 0), Room.FindClosestActiveCellIndex(new Vector3(125, -150, -1000)));
            Assert.AreEqual(new Vector2Int(0, 1), Room.FindClosestActiveCellIndex(new Vector3(150, -125, 0)));
        }

        [Test]
        public void TestGetCellActivityThrowsOutOfRangeException()
        {
            Assert.Throws<System.IndexOutOfRangeException>(() => Room.GetCellActivity(-1, 1));
            Assert.Throws<System.IndexOutOfRangeException>(() => Room.GetCellActivity(1, -1));
        }

        [Test]
        public void TestSetCellActivityByBoolean()
        {
            Assert.IsTrue(Room.GetCellActivity(0, 0));
            Room.SetCellActivity(0, 0, false);
            Assert.IsFalse(Room.GetCellActivity(0, 0));
        }

        [Test]
        public void TestSetCellActivity()
        {
            Assert.IsTrue(Room.GetCellActivity(0, 0));
            Room.SetCellActivity(0, 0, CellActivity.Deactivate);
            Assert.IsFalse(Room.GetCellActivity(0, 0));
            Room.SetCellActivity(0, 0, CellActivity.Activate);
            Assert.IsTrue(Room.GetCellActivity(0, 0));
            Room.SetCellActivity(0, 0, CellActivity.Toggle);
            Assert.IsFalse(Room.GetCellActivity(0, 0));
            Room.SetCellActivity(0, 0, CellActivity.Toggle);
            Assert.IsTrue(Room.GetCellActivity(0, 0));
            Room.SetCellActivity(0, 0, CellActivity.None);
            Assert.IsTrue(Room.GetCellActivity(0, 0));
        }

        [Test]
        public void TestSetCellActivityByBooleanThrowsOutOfRangeException()
        {
            Assert.Throws<System.IndexOutOfRangeException>(() => Room.SetCellActivity(-1, 1, true));
            Assert.Throws<System.IndexOutOfRangeException>(() => Room.SetCellActivity(1, -1, true));
        }

        [Test]
        public void TestSetCellActivityThrowsOutOfRangeException()
        {
            Assert.Throws<System.IndexOutOfRangeException>(() => Room.SetCellActivity(-1, 1, CellActivity.Deactivate));
            Assert.Throws<System.IndexOutOfRangeException>(() => Room.SetCellActivity(1, -1, CellActivity.Deactivate));
        }

        [Test]
        public void TestSetCellActivities()
        {
            Room.SetCellActivities(new Vector2Int(1, 1), new Vector2Int(2, 2), CellActivity.Deactivate);
            Assert.IsFalse(Room.GetCellActivity(1, 1));
            Assert.IsFalse(Room.GetCellActivity(2, 1));
            Assert.IsFalse(Room.GetCellActivity(1, 2));
            Assert.IsFalse(Room.GetCellActivity(2, 2));
            Room.SetCellActivities(new Vector2Int(2, 2), new Vector2Int(1, 1), CellActivity.Activate);
            Assert.IsTrue(Room.GetCellActivity(1, 1));
            Assert.IsTrue(Room.GetCellActivity(2, 1));
            Assert.IsTrue(Room.GetCellActivity(1, 2));
            Assert.IsTrue(Room.GetCellActivity(2, 2));
        }

        [Test]
        public void TestSetCellActivitiesOutOfRangeDoesNothing()
        {
            Room.SetCellActivities(new Vector2Int(-1, -1), new Vector2Int(2, 2), CellActivity.Deactivate);
            Assert.IsTrue(Room.GetCellActivity(1, 1));
            Assert.IsTrue(Room.GetCellActivity(2, 1));
            Assert.IsTrue(Room.GetCellActivity(1, 2));
            Assert.IsTrue(Room.GetCellActivity(2, 2));
            Room.SetCellActivities(new Vector2Int(2, 2), new Vector2Int(-1, -1), CellActivity.Deactivate);
            Assert.IsTrue(Room.GetCellActivity(1, 1));
            Assert.IsTrue(Room.GetCellActivity(2, 1));
            Assert.IsTrue(Room.GetCellActivity(1, 2));
            Assert.IsTrue(Room.GetCellActivity(2, 2));
        }

        [Test]
        public void TestGlobalPositionToCellIndexFromCellCenterGlobalPosition()
        {
            Room.transform.position = new Vector3(543, 9084, 234);

            for (int i = 0; i < Room.Rows; i++)
            {
                for (int j = 0; j < Room.Columns; j++)
                {
                    var center = Room.CellCenterGlobalPosition(i, j);
                    var index = Room.GlobalPositionToCellIndex(center);
                    Assert.AreEqual(new Vector2Int(i, j), index);
                }
            }
        }

        [Test]
        public void TestLocalPositionToCellIndexFromCellCenterLocalPosition()
        {
            Room.transform.position = new Vector3(543, 9084, 234);

            for (int i = 0; i < Room.Rows; i++)
            {
                for (int j = 0; j < Room.Columns; j++)
                {
                    var center = Room.CellCenterLocalPosition(i, j);
                    var index = Room.LocalPositionToCellIndex(center);
                    Assert.AreEqual(new Vector2Int(i, j), index);
                }
            }
        }
    }
}