using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;


public class PropManager : MonoBehaviour
{
    public static PropManager Instance { get; private set; }

    private static RoomManager roomManager = RoomManager.Instance;

    [SerializeField]
    private List<Prop> propsToPlace;

    [SerializeField]
    private GameObject propPrefabParent;

    //All objects that have been spawned
    [SerializeField]
    private List<GameObject> allProps = new List<GameObject>();

    private PropManager() { }

    //Make sure we can find our roomManager, because it has all of our floors/rooms data
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public UnityEvent FinishedGeneration;

    /// <summary>
    /// Process our props and check with the different room tiles, what we need to place down props in our dungeon
    /// </summary>
    public void InitProps()
    {
        roomManager = FindObjectOfType<RoomManager>();

        foreach (Room room in roomManager.Rooms) 
        {
            //Place props ON top of upper walls
            List<Prop> wallProps = propsToPlace.Where(x => x.OnTopWalls).OrderByDescending(x => x.Size.x * x.Size.y).ToList();
            PlaceWallProps(room, wallProps, room.WallsTop, PlacementOrigin.BottomLeft);

            //Place props near LEFT wall
            List<Prop> leftWallProps = propsToPlace.Where(x => x.NearWallLeft).OrderByDescending(x => x.Size.x * x.Size.y).ToList();
            InitPropsPlacement(room, leftWallProps, room.TilesNearLeftSide, PlacementOrigin.BottomLeft);

            //Place props near RIGHT wall
            List<Prop> rightWallProps = propsToPlace.Where(x => x.NearWallRight).OrderByDescending(x => x.Size.x * x.Size.y).ToList();
            InitPropsPlacement(room, rightWallProps, room.TilesNearRightSide, PlacementOrigin.TopRight);

            //Place props near UP wall
            List<Prop> topWallProps = propsToPlace.Where(x => x.NearWallUpper).OrderByDescending(x => x.Size.x * x.Size.y).ToList();
            InitPropsPlacement(room, topWallProps, room.TilesNearUpperSide, PlacementOrigin.TopRight);

            //Place props near DOWN wall
            List<Prop> downWallProps = propsToPlace.Where(x => x.NearWallBottom).OrderByDescending(x => x.Size.x * x.Size.y).ToList();
            InitPropsPlacement(room, downWallProps, room.TilesNearBottomSide, PlacementOrigin.BottomLeft);

            //Place props on inner floor tiles, not touching walls
            List<Prop> innerProps = propsToPlace.Where(x => x.Inner).OrderByDescending(x => x.Size.x * x.Size.y).ToList();
            InitPropsPlacement(room, innerProps, room.TilesInsideRoom, PlacementOrigin.BottomLeft);

        }

        Invoke("RunEvent", 1);
    }

    public void RunEvent()
    {
        FinishedGeneration?.Invoke();
    }

    /// <summary>
    /// Helper function that assists with placing our props in each room.
    /// </summary>
    private void InitPropsPlacement(Room room, List<Prop> props, HashSet<Vector2Int> freeTiles, PlacementOrigin placement)
    {
        HashSet<Vector2Int> tempPositions = new HashSet<Vector2Int>(freeTiles);

        //Ensure we have a free path to place items, that does not get in the way of our players and enemies
        tempPositions.ExceptWith(roomManager.Corridors);

        //Make sure we are not placing at already placed props
        tempPositions.ExceptWith(room.PropPositions);

        //loop through each of our Scriptable objects of props and set a random range to place them down inside our dungeon
        foreach (Prop propToPlace in props)
        {
            int quantity = new int();

            if(propToPlace.PlaceRandomly)
            {
                quantity = Random.Range(0, propToPlace.PlacementQuantity);
            }
            else
            {
                quantity = propToPlace.PlacementQuantity;
            }

            //loop through our varied amount of props
            for (int i = 0; i < quantity; i++)
            {
                // Check if the prop should be placed in specific room types
                if (propToPlace.roomTypeRestricted && !propToPlace.allowedRoomTypes.Contains(room.roomType))
                {
                    continue; // Skip this prop if the room type is not allowed
                }

                //Order our new list by shuffling and convert to a list
                List<Vector2Int> possiblePositions = tempPositions.OrderBy(x => Guid.NewGuid()).ToList();

                if (!PlaceProp(room, propToPlace, possiblePositions, placement))
                    break;
            }
        
        }

    }

    /// <summary>
    /// Main placer method that returns true or false back to PlaceProps, a sanity checker in a sense
    /// </summary>
    /// <param name="room">Pass the room we want to check</param>
    /// <param name="propToPlace">the prop we want to place</param>
    /// <param name="possiblePositions">Locations that are possible for the prop to be placed</param>
    /// <param name="placement">Where to place it depending on it's origin</param>
    /// <returns>true or false</returns>    
    private bool PlaceProp(Room room, Prop propToPlace, List<Vector2Int> possiblePositions, PlacementOrigin placement)
    {
        for (int i = 0; i < possiblePositions.Count; i++)
        {
            Vector2Int position = possiblePositions[i];
            if (room.PropPositions.Contains(position))
                return false;
             
            //What are the positions around our prop, can we place more than one prop or just one
            //TryToFit's main purpose is to make that our prop fits
            List<Vector2Int> positionsAroundProp = DoesPropFit(propToPlace, possiblePositions, position, placement);

            //Based on the count of our positions, will there be enough space to place our prop?
            if(positionsAroundProp.Count == propToPlace.Size.x * propToPlace.Size.y)
            {
                PlaceGameObject(room, position, propToPlace);

                foreach (Vector2Int pos in positionsAroundProp)
                {
                    room.PropPositions.Add(pos);
                }

                return true;
            }
        }
        return false;
    }


    /// <summary>
    /// Method checker that checks, if the prop will fit depending on its possible location, position and placement method
    /// </summary>
    /// <param name="prop">Our prop</param>
    /// <param name="possiblePositions"></param>
    /// <param name="originalPosition"></param>
    /// <param name="placement"></param>
    /// <returns></returns>
    private List<Vector2Int> DoesPropFit(Prop prop, List<Vector2Int> possiblePositions, Vector2Int originalPosition, PlacementOrigin placement)
    {
        //List of positions around our prop, we are trying to place, that we return back.
        List<Vector2Int> positions = new();

        //Based on our placement argument select one of the cases below and perform our 
        //algorithm of looking to find possible positions around our prop.
        switch (placement)
        {
            case PlacementOrigin.BottomLeft:
                {
                    for (int xOffset = 0; xOffset < prop.Size.x; xOffset++)
                    {
                        for (int yOffset = 0; yOffset < prop.Size.y; yOffset++)
                        {
                            Vector2Int tempPos = originalPosition + new Vector2Int(xOffset, yOffset);
                            if (possiblePositions.Contains(tempPos))
                                positions.Add(tempPos);
                        }
                    }
                    break;
                }

            case PlacementOrigin.BottomRight: 
                {
                    for (int xOffset = -prop.Size.x + 1; xOffset <= 0; xOffset++)
                    {
                        for (int yOffset = 0; yOffset < prop.Size.y; yOffset++)
                        {
                            Vector2Int tempPos = originalPosition + new Vector2Int(xOffset, yOffset);
                            if (possiblePositions.Contains(tempPos))
                                positions.Add(tempPos);
                        }
                    }
                    break;
                }

            case PlacementOrigin.TopLeft: 
                {
                    for (int xOffset = 0; xOffset < prop.Size.x; xOffset++)
                    {
                        for (int yOffset = -prop.Size.y + 1; yOffset <= 0; yOffset++)
                        {
                            Vector2Int tempPos = originalPosition + new Vector2Int(xOffset, yOffset);
                            if (possiblePositions.Contains(tempPos))
                                positions.Add(tempPos);
                        }
                    }

                    break;
                }

            case PlacementOrigin.TopRight: 
                {
                    for (int xOffset = -prop.Size.x + 1; xOffset <= 0; xOffset++)
                    {
                        for (int yOffset = -prop.Size.y + 1; yOffset <= 0; yOffset++)
                        {
                            Vector2Int tempPos = originalPosition + new Vector2Int(xOffset, yOffset);
                            if (possiblePositions.Contains(tempPos))
                                positions.Add(tempPos);
                        }
                    }

                    break;
                }
        }

        return positions;
    }

    private void PlaceWallProps(Room room, List<Prop> props, HashSet<Vector2Int> positions, PlacementOrigin origin)
    {
        int quantity = new int();

        HashSet<Vector2Int> tempPositions = new HashSet<Vector2Int>(positions);

        List<Vector2Int> possiblePositions = tempPositions.OrderBy(x => Guid.NewGuid()).ToList();

        foreach (Prop prop in props)
        {
            if(prop.PlaceRandomly)
            {
                quantity = Random.Range(0, prop.PlacementQuantity);
            }else
            {
                quantity = prop.PlacementQuantity;
            }

            for (int i = 0; i < quantity; i++)
            {

                // Check if the prop should be placed in specific room types
                if (prop.roomTypeRestricted && prop.allowedRoomTypes.Contains(room.roomType))
                {
                    continue; // Skip this prop if the room type is not allowed
                }

                if (!PlaceProp(room, prop, possiblePositions, origin))
                    break;
            }
            
        }
    }

    private void PlaceGameObject(Room room, Vector2Int position, Prop propToPlace)
    {
        GameObject propHolder;

        if( propToPlace.prefab != null)
        {
            propHolder = Instantiate(propToPlace.prefab);

        }else
        {
            //Our Prop itself
            propHolder = Instantiate(propPrefabParent);
        }

        SpriteRenderer propSpriteRenderer = propHolder.GetComponentInChildren<SpriteRenderer>();

        propSpriteRenderer.sprite = propToPlace.propSprite;

        if(propToPlace.hasCollider)
        {
            CapsuleCollider2D collider = propSpriteRenderer.gameObject.AddComponent<CapsuleCollider2D>();

            //Reset at Vector2(0,0)
            collider.offset = Vector2.zero;

            if (propToPlace.Size.x > propToPlace.Size.y)
            {
                collider.direction = CapsuleDirection2D.Horizontal;
            }

            Vector2 size = new Vector2(propToPlace.Size.x * 0.8f, propToPlace.Size.y * 0.8f);
            collider.size = size;
        }

        if (propToPlace.isTrigger)
        {
            CircleCollider2D triggerCollider = propHolder.AddComponent<CircleCollider2D>();
            triggerCollider.offset = new Vector2(0.5f, 0.5f);
            triggerCollider.isTrigger = true;

            float size = Mathf.Max(propToPlace.Size.x, propToPlace.Size.y) / 2f;
            triggerCollider.radius = size + 0.5f;
        }

        propHolder.transform.localPosition = (Vector2)position;

        //Ensure that we are size properly based on our propholds position and size.
        propSpriteRenderer.transform.localPosition = (Vector2)propToPlace.Size * 0.5f;

        //Save the prop in the room data (so in the dunbgeon data)

        room.PropPositions.Add(position);
        
        allProps.Add(propHolder);
    }

    /// <summary>
    /// Where to start placing the prop ex. start at BottomLeft corner and search.
    /// If there are free space to the Right and Up in case of placing a biggex prop.
    /// </summary>
    public enum PlacementOrigin
    {
        BottomLeft,
        BottomRight,
        TopLeft,
        TopRight
    }

    public void Reset()
    {
       foreach(GameObject prop in allProps)
        {
            Destroy(prop);
        }
    }

    public PropData GetPropData()
    {
        PropData propData = new PropData();

        foreach(GameObject prop in allProps)
        {
            PropInfo saveData = new PropInfo();
            saveData.prefabName = prop.name;
            saveData.position = prop.transform.localPosition;
            saveData.rotation = prop.transform.localRotation;

            // Get the sprite from the child object's sprite renderer
            saveData.sprite = prop.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;

            saveData.spritePosition = prop.GetComponentInChildren<SpriteRenderer>().transform.localPosition;

            // Check if the prop has a collider
            CapsuleCollider2D collider = prop.GetComponentInChildren<CapsuleCollider2D>();
            if (collider != null)
            {
                saveData.hasCollider = true;
                saveData.ColliderSize = collider.size;
            }
            else
            {
                saveData.hasCollider = false;
            }

            CircleCollider2D triggerCollider = prop.GetComponent<CircleCollider2D>();
            if(triggerCollider != null)
            {
                saveData.hasTrigger = true;
                saveData.triggerOffset = triggerCollider.offset;
                saveData.triggerRadius = triggerCollider.radius;
            }

            propData.propSaveData.Add(saveData);
        }

        return propData;
    }

    internal void LoadPropData(PropData propData)
    {

        foreach (PropInfo saveData in propData.propSaveData)
        {
            GameObject prefab = null;

            if (saveData.prefabName.Contains("Chest"))
            {
                prefab = Resources.Load<GameObject>("GameObjects/Objects/Props/ChestProp");

            }else
            {
                prefab = Resources.Load<GameObject>("GameObjects/Objects/Props/StaticProp");
            }

            if(prefab == null)
            {
                Debug.LogError("Failed to load prefab.");
                return;
            }

            GameObject newPropParent = Instantiate(prefab);
            newPropParent.transform.localPosition = saveData.position;
            newPropParent.transform.localRotation = saveData.rotation;

            SpriteRenderer propSpriteRenderer = newPropParent.GetComponentInChildren<SpriteRenderer>();
            propSpriteRenderer.GetComponent<SpriteRenderer>().sprite = saveData.sprite;
            propSpriteRenderer.transform.localPosition = saveData.spritePosition;

            // Add a collider if the saved data indicated that it had one
            if (saveData.hasCollider)
            {
                CapsuleCollider2D collider = propSpriteRenderer.gameObject.AddComponent<CapsuleCollider2D>();

                //Reset at Vector2(0,0)
                collider.offset = Vector2.zero;

                if (propSpriteRenderer.sprite.bounds.size.x > propSpriteRenderer.sprite.bounds.size.y)
                {
                    collider.direction = CapsuleDirection2D.Horizontal;
                }

                Vector2 size = saveData.ColliderSize;
                collider.size = size;
            }

            if(saveData.hasTrigger)
            {
                CircleCollider2D triggerCollider = newPropParent.AddComponent<CircleCollider2D>();
                triggerCollider.isTrigger = true;
                triggerCollider.radius = saveData.triggerRadius;
                triggerCollider.offset = saveData.triggerOffset;
            }

            allProps.Add(newPropParent);
        }
    }
}
