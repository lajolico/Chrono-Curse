using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Prop", menuName = "ChronoCurse/Prop")]
public class Prop : ScriptableObject
{
    [Header("Prop data:")]
    public Sprite propSprite;
    public Vector2Int Size = Vector2Int.one;
    public bool hasCollider = true;
    public bool isTrigger = false;
    public GameObject prefab;

    [Space, Header("Placement type: ")]
    public bool NearWallUpper = false;
    public bool NearWallBottom = false;
    public bool NearWallRight = false;
    public bool NearWallLeft = false;
    public bool Inner = false;
    public bool OnTopWalls = false;

    [Space, Header("Room Type Options")]
    public bool roomTypeRestricted;
    public List<Room.RoomType> allowedRoomTypes;

    [Space, Header("Group placement: ")]
    public bool PlaceAsGroup = false;

    [Min(1)]
    public int GroupMinAmount = 1;
    [Min(1)]
    public int GroupMaxAmount = 1;

    [Space, Header("Placement Options: ")]
    [Tooltip("Random amount of props to place. ")]
    public bool PlaceRandomly = false;

    [Range(1, 20)]
    public int PlacementQuantity = 1;
}