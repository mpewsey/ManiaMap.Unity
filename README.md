# ManiaMap.Unity

[![Docs](https://github.com/mpewsey/ManiaMap.Unity/workflows/Docs/badge.svg?event=push)](https://mpewsey.github.io/ManiaMap.Unity)
![Unity ≥ 2022.1](https://img.shields.io/badge/Unity-%E2%89%A5%202022.1-blue)
![Version](https://img.shields.io/github/v/tag/mpewsey/ManiaMap.Unity?label=Version)

## About

This package provides components for interfacing the [ManiaMap](https://github.com/mpewsey/ManiaMap) procedural layout generator with Unity.

![BigMap](https://user-images.githubusercontent.com/23442063/158001876-cb3962a8-9826-44e9-bb19-a5779e3f99d6.png)

## Installation

In Unity, select `Window > Package Manager`.

![Unity Package Manager](https://user-images.githubusercontent.com/23442063/163601100-191d8699-f4fd-42cc-96d4-f6aa5a8ae29b.png)

Select `Add package from git URL...` and paste the following URL:

```
https://github.com/mpewsey/ManiaMap.Unity.git?path=Packages/ManiaMap.Unity
```

To lock into a specific version, append `#{VERSION_TAG}` to the end of the URL. For example:

```
https://github.com/mpewsey/ManiaMap.Unity.git?path=Packages/ManiaMap.Unity#v1.0.0
```

## Documentation

For information on the scripting API, as well as instructions related to procedurally generating a room layout, see the [documentation](https://mpewsey.github.io/ManiaMap.Unity/md_pages_layout_generation_instructions.html).


## Samples

Sample scenes are available in the project `Assets` directory. To explore them, clone this repository and open the project in Unity.

<table style='text-align: center'>
    <tr>
        <td width='50%' align='center'>
            <img width=450 alt='2d layout' src='https://github.com/mpewsey/ManiaMap.Unity/assets/23442063/e170235f-df4f-475f-bd2c-bbdc90b464e7'><br>
            2D layout generation sample.
        </td>
        <td width='50%' align='center'>
            <img width=450 alt='3d layout' src='https://github.com/mpewsey/ManiaMap.Unity/assets/23442063/04453eaa-6d5f-4aa1-964c-b69b6c307ad8'><br>
            3D layout generation sample.
        </td>
    </tr>
    <tr>
        <td width='50%' align='center'>
            <img width=450 alt='layout map book' src='https://github.com/mpewsey/ManiaMap.Unity/assets/23442063/866d377b-0887-42cf-b9fb-1204e3ebce61'><br>
            Layout tile map book sample with onionskin map display.
        </td>
        <td width='50%' align='center'>
            <img width=450 alt='layout map' src='https://github.com/mpewsey/ManiaMap.Unity/assets/23442063/9891dc70-662c-47c8-93c7-3bdbddbd58b1'><br>
            Layout tile map sample.
        </td>
    </tr>
    <tr>
        <td width='50%' align='center'>
            <img width=450 alt='3d layout' src='https://github.com/mpewsey/ManiaMap.Unity/assets/23442063/05fff504-f15d-4c2e-869c-ef59e2e9e4b1'><br>
            Layout map sample.
        </td>
        <td width='50%' align='center'>
        </td>
    </tr>
</table>
