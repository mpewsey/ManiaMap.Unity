using MPewsey.Common.Mathematics;
using MPewsey.Common.Random;
using MPewsey.ManiaMap;
using MPewsey.ManiaMap.Graphs;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace MPewsey.ManiaMapUnity.Tests
{
    public class TestRoomComponent
    {
        [SetUp]
        public void SetUp()
        {
            Assets.DestroyAllGameObjects();
        }

        [Test]
        public void TestGetMMRoomTemplate()
        {
            var room = Assets.InstantiatePrefab<RoomComponent>(Assets.Angle3x4RoomPath);
            var template = room.GetMMRoomTemplate();
            Object.DestroyImmediate(room.gameObject);
            Assert.IsNotNull(template);
        }

        [Test]
        public void TestInitialize()
        {
            var seed = new RandomSeed(12345);
            var room = Assets.InstantiatePrefab<RoomComponent>(Assets.Angle3x4RoomPath);
            var template = room.GetMMRoomTemplate();

            // Create fake layout.
            var layout = new Layout(1, "Test", seed.Seed);
            var node = new LayoutNode(1);
            var roomLayout = new Room(node, Vector2DInt.Zero, template, seed.Next());
            layout.Rooms.Add(roomLayout.Id, roomLayout);
            var layoutState = new LayoutState(layout);
            var roomState = layoutState.RoomStates[roomLayout.Id];
            var doorConnections = new List<DoorConnection>();

            room.Initialize(layout, layoutState, roomLayout, roomState, doorConnections, RoomPositionOption.LayoutPosition);
            Assert.IsTrue(room.IsInitialized);
        }

        [Test]
        public void TestIntantiateRoom()
        {
            var seed = new RandomSeed(12345);
            var prefab = Assets.InstantiatePrefab<RoomComponent>(Assets.Angle3x4RoomPath);
            var template = prefab.GetMMRoomTemplate();

            // Create fake layout.
            var layout = new Layout(1, "Test", seed.Seed);
            var node = new LayoutNode(1);
            var roomLayout = new Room(node, Vector2DInt.Zero, template, seed.Next());
            layout.Rooms.Add(roomLayout.Id, roomLayout);
            ManiaMapManager.Current.Initialize(layout, new LayoutState(layout));

            var room = RoomComponent.InstantiateRoom(roomLayout.Id, prefab.gameObject, null, RoomPositionOption.LayoutPosition);
            Object.DestroyImmediate(prefab.gameObject);
            Assert.IsNotNull(room);
        }
    }
}
