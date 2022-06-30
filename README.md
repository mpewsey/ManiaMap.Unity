# ManiaMap.Unity

[![Docs](https://github.com/mpewsey/ManiaMap.Unity/workflows/Docs/badge.svg?event=push)](https://mpewsey.github.io/ManiaMap.Unity)
![Unity ≥ 2021.2](https://img.shields.io/badge/Unity-%E2%89%A5%202021.2-blue)
![Version](https://img.shields.io/github/v/tag/mpewsey/ManiaMap.Unity?label=Version)

## About

This package provides components for interfacing the [ManiaMap](https://github.com/mpewsey/ManiaMap) procedural layout generator with Unity.

![BigMap](https://user-images.githubusercontent.com/23442063/158001876-cb3962a8-9826-44e9-bb19-a5779e3f99d6.png)

## Installation

To add the package to a project, in Unity, select `Window > Package Manager`.

![HexagonalUI](https://user-images.githubusercontent.com/23442063/163601100-191d8699-f4fd-42cc-96d4-f6aa5a8ae29b.png)

Select `Add package from git URL...` and paste the following URL:

```
https://github.com/mpewsey/ManiaMap.Unity.git
```

RECOMMENDED: To lock into a specific version, append `#{VERSION_TAG}` to the end of the URL. For example:

```
https://github.com/mpewsey/ManiaMap.Unity.git#v1.0.0
```

## Example

The following subsections outline how to procedurally generate a layout.

### Step 1: Create Room Templates

The generator creates rooms by pulling from user-defined rooms. To the generator a room is essentially a collection of cells in a grid, with information, such as door connections, assigned to them.

#### Creating a Room

1. Create a new `Room` by selecting `GameObject > Mania Map > Room`. Create a prefab for this Game Object that may be utilized later.
2. In the inspector, provide a name for the room, along with its width and height in terms of cells.
3. Click the `Update Room` button to generate the cells.

![Generated Cells](https://user-images.githubusercontent.com/23442063/175812812-5871bc8c-84e0-4b2f-ae04-0073c9aaed61.png)

4. In the scene view, select any empty cells and set the `Is Empty` flag in the inspector. The cell Gizmos will show crossed out for empty cells.

![Empty Cells](https://user-images.githubusercontent.com/23442063/175812984-1e05c096-9a93-4855-9b34-fb566083ddcc.png)

5. Next, add all possible doors and collectable spots to the room per the following sections.

#### Creating Doors

1. Create a new `Door` by selecting the Room Game Object then `GameObject > Mania Map > Door`. The door should be added as a child of the room.
2. At this point, you should probably create a prefab for the door with art supporting the particular direction, along with all applicable events. However, those specifics are beyond the scope of this example.
3. Position the door at the desired location within the room and assign its direction and connection constraints.
4. To auto assign the closest cell and direction to the door, make sure the applicable flags are selected and click the `Auto Assign` button. Note that the `Update Room` button on the Room component will also perform this operation on all doors in the room.

![Doors](https://user-images.githubusercontent.com/23442063/175813945-080a6eab-f333-4036-8816-90a2746401d7.png)

#### Creating Collectable Spots

1. Create a new `CollectableSpot` by selecting the Room Game Object then `GameObject > Mania Map > Collectable Spot`. The collectable spot should be added as a child of the room.
2. At this point, you should probably create a prefab for the collectable spot with art (treasure chest, orb, sparkle, etc.) and all applicable events. However, those specifics are beyond the scope of this example.
3. Position the collectable spot at the desired location within the room and assign a collectable group to it (`Assets > Mania Map > CollectableGroup`).
4. To auto assign the closest cell to the collectable spot, make sure the applicable flag is selected and click the `Auto Assign` button. Note that the `Update Room` button on the Room component will also perform this operation on all collectable spots in the room.

![Collectable Spot](https://user-images.githubusercontent.com/23442063/175827419-9639dd11-18ad-4c99-97b0-571984efab97.png)

#### Saving Room Templates

1. Select the Room and click the `Save Template` button. The data for the room template will be saved in the `ManiaMap > RoomTemplates` directory of the project. Alternately, use the `Mania Map > Batch Save Templates` command to save room templates for all prefabs in the project with a Room component at its root.

### Step 2: Create Room Template Groups

1. Create a `TemplateGroup` by selecting `Assets > Mania Map > Template Group`.
2. In the inspector, provide a unique name to the group and add any of the generated room templates you wish to associate with the group to the list.

![Template Group](https://user-images.githubusercontent.com/23442063/175827410-a61dcbc4-2275-4217-bd83-4fff0f048e5d.png)

### Step 3: Create Layout Graphs

1. Create a `LayoutGraph` by selecting `Assets > Mania Map > Layout Graph`.
2. Double click on the asset to open the graph editor.
3. Add rooms (nodes) to the graph by right-clicking in the graph area and selecting `Add Node`.
4. In the inspector, add a template group to the selected nodes to associate the room templates that may be used at those locations.
5. Add edges to the graph by right-clicking a node, selecting `Add Edge`, then selecting another node.
6. To edit the edge attributes, ensure that `View > Toggle Edge Display` is enabled, then select the edges in the graph area.

![Layout Graph](https://user-images.githubusercontent.com/23442063/175828288-b47a1e3d-ac81-4a2a-b436-0f22b7da3a6c.png)

### Step 4: Create Generation Pipeline

1. Create a `GenerationPipeline` by selecting `GameObject > Mania Map > Generation Pipeline`.
2. Select the child `<Inputs>` Game Object and add any layout graphs and collectable groups you wish to associate with the generator to the inputs.

![Generation Pipeline](https://user-images.githubusercontent.com/23442063/175828818-3a0a09da-69d8-4d60-a48c-b69d4ceac374.png)

3. To run the pipeline and generate a layout, run the `Generate` method from a script.

```Generate.cs
int seed = 12345;
var results = Pipeline.Generate(seed);

// Retrieve the generated layout data from the ouput results
var layout = (Layout)results.Outputs["Layout"];
```
