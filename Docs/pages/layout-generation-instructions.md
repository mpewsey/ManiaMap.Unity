# Generating a Layout

The following subsections outline how to procedurally generate an arrangement of rooms, called a `Layout`.

## Step 1: Create Rooms and Room Templates

The procedural generator creates layouts by pulling from user-defined room templates. To the generator, a room is a collection of cells in a grid, with information, such as the available door connections, assigned to them.

### Creating a Room

1. In the Unity scene heirarchy, right-click then select `Mania Map > Room`.
2. In the inspector, specify the room's type, whether you intend to work with 2D or 3D physics.
3. Specify the number of cell rows and columns, along with the cell size to create the bounding shape of the room.
4. When the room node is selected, a toolbar will appear in the Godot main window (Circled in red in the screenshot below). The options on this toolbar may be used to edit the cell activities to further define the shape of the room. For instance, selecting the toggle edit mode button, then clicking or dragging over cells in the view will toggle the targeted cells on or off.
5. Save the room as a prefab.

In the below screenshot, the cells for an angle shaped room have been created, with the active cells shown in blue and inactive cells shown in red and crossed-out. The active cells are the regions where we intend to build our scene.

![Screenshot 2024-06-14 182659](https://github.com/mpewsey/ManiaMap.Unity/assets/23442063/f4248cb5-54f0-4db3-8b66-955a54efe5b6)

### Creating Doors

Doors define the locations where two rooms can be connected. At least one door must be added to a room.

1. Right-click on the room in the scene heirarchy and select `Mania Map > Door`. To save time, you may wish to save your doors as prefabs with any related components, such as sprites or collision. These prefabs can then be referenced in your room prefab.
2. Position the door within the room and assign its direction and connection constraints.
4. To auto assign the closest cell and direction to the door, make sure the applicable flags are selected in the inspector and click the `Auto Assign` button on the room toolbar. The assigned door direction will be based on its location relative to the center of its assigned cell.

Additional room child nodes such as `CollectableSpotComponent`, `Feature`, and `RoomFlag` can also be added to the room if you wish them to be included.

The below screenshot shows a square 3x3 room with doors around its entire perimeter. Each of the door direction (north, west, east, and south) use a different prefab since the tiles depicting the doors are different for each.

![Screenshot 2024-06-14 184508](https://github.com/mpewsey/ManiaMap.Unity/assets/23442063/7cd8e3dc-7723-4d7d-bfbf-37eb6d1b210e)

### Exporting Room Templates

The procedural generator uses one or more `RoomTemplateResource` exported from a room to generate layouts.

1. With at least one room saved as a prefab in your project, from the top toolbar, select `Mania Map > Batch Update Room Templates`.
2. This operation will perform auto assignment on all room prefabs throughout the project and save `RoomTemplateResource` Scriptable Objects with the `.room_template.asset` extension to the prefab paths.

## Step 2: Create Room Template Groups

One or more `TemplateGroup` are used by the procedural generator to determine which rooms can be assigned to a given position in a layout.

1. To create a template group, right-click in the Unity project explorer and select `Create > Mania Map > Template Group`.
2. Click on the newly created template group and, in the inspector, assign a unique name to the group.
3. Drag and drop one or more `RoomTemplateResource` into the rectangular region to add them to the group.

> **Note:** Locking the inspector and searching `t:RoomTemplateResource` in the project explorer can make adding room templates to a group easier.

## Step 3: Create a Layout Graph

The procedural generator uses a `LayoutGraphResource` as a base for generating layouts. This allows you to design high level relationships between rooms while still making the resulting layout appear random.

1. To create a layout graph, right-click in the Unity project explorer and select `Create > Mania Map > Layout Graph`.
2. Double click on the created object to open the graph editor window.
3. In the graph editor window, right click in an open area, then select `Create Node` to add a node to the graph. Each node will serve as a room location.
4. To add edges, serving as connections between rooms, to the graph, right-click on the starting node and select `Add Edge`, then click on a second node to make the connection.
5. Selecting nodes and/or edges will allow you to edit their properties in the inspector. Each node must have a `TemplateGroup` assigned; though it is optional of edges.

![Screenshot 2024-06-14 191811](https://github.com/mpewsey/ManiaMap.Unity/assets/23442063/b3b44803-7173-4a48-af0b-05d034b6b2e7)

## Step 4: Create a Generation Pipeline

The `GenerationPipeline` takes various inputs and feeds them through a series of operational steps to generate one or more outputs. In the context of this package, the output is most notably a room `Layout`.

1. To create a pipeline, right-click in the scene heirarchy and select `Mania Map > Generation Pipeline`. A pipeline with the default inputs and steps will be created.
2. On the `Inputs` Game Object, add one or more `LayoutGraphResource` to the `LayoutGraphsInput` component.
3. Optionally, add one or more `CollectableGroup` to the `CollectableGroupsInput` component.
4. Create a script that references the `GenerationPipeline` you wish to run, and call the `Run`, `RunAsync`, or `RunAttemptsAsync` methods to generate a layout. For example:

```ExampleGenerationPipelineRunner.cs
using MPewsey.ManiaMap;
using MPewsey.ManiaMapGodot.Generators;

public class ExampleGenerationPipelineRunner : MonoBehaviour
{
    [SerializeField] public GenerationPipeline _pipeline;

    public void RunPipeline()
    {
        var results = _pipeline.Run();
        var layout = results.GetOutput<Layout>("Layout");

        // At this point, you will probably want to do something with the layout...
        //
        // * You could save it to file using the JsonSerialization or XmlSerialization static classes.
        // * You could use a RoomDatabase or RoomTemplateGroupDatabase to instantiate the rooms in the layout.
        // * You could use it with the LayoutTileMap or LayoutTileMapBook components to generate maps.
        //
        // See the project samples for example implementations.
    }
}
```

> **Note:** Depending on your room templates and layout graph, it may not be possible to generate a layout for all (if any) random seeds. If you are encountering issues with successfully generating a layout, you may need to reconsider the constraints imposed on its generation. For example, you may need to add more doors to ensure that your rooms have a better chance of connecting. Even then, you may still encounter some isolated failures, in which case the generation pipeline `RunAttemptsAsync` method can help by automatically falling back to other seeds.
