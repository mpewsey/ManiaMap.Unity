# ManiaMap.Unity

[![Docs](https://github.com/mpewsey/ManiaMap.Unity/workflows/Docs/badge.svg?event=push)](https://mpewsey.github.io/ManiaMap.Unity)
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

The subsections outline how to generate a basic layout using the `GenerationPipeline`.

### Step 1: Create Room Templates

The generator creates rooms by pulling from user-defined rooms. To the generator is room is essentially a collection of cells in a grid, with information, such as door connections, assigned to them.

#### Creating a Room

1. Create a new `Room` by selecting `GameObject > Mania Map > Room`.
2. In the inspector, provide a name for the room, along with its width and height in terms of cells.
4. Click the `Update Room` button to generate the cells.

![Generated Cells](https://user-images.githubusercontent.com/23442063/175812812-5871bc8c-84e0-4b2f-ae04-0073c9aaed61.png)

5. In the scene view, select any empty cells and set the `Is Empty` flag in the inspector. The cell Gizmos will show crossed out for empty cells.

![Empty Cells](https://user-images.githubusercontent.com/23442063/175812984-1e05c096-9a93-4855-9b34-fb566083ddcc.png)

7. Next, add all possible door connections and collectable spots to the room per the following sections.

#### Creating Doors

1. Create a new `Door` by selecting the Room Game Object then `GameObject > Mania Map > Door`. The door should be added as a child of the room.
2. At this point, you should probably create a prefab for the door with art supporting the particular direction, along with all applicable events. However, those specifics are beyond the scope of this example.
3. Position the door at the desired location within the room and assign its direction and connection constraints.
4. To auto assign the cell and direction, make sure the applicable flags are selected and click the `Auto Assign` button. Note that the `Update Room` button on the Room component will also perform this operation on all doors in the room.

![Doors](https://user-images.githubusercontent.com/23442063/175813945-080a6eab-f333-4036-8816-90a2746401d7.png)
