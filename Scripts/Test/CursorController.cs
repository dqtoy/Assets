﻿using Generic.CustomInput;
using Generic.Singleton;
using UI;
using UnityEngine;
using UnityEngine.Events;
using static NodeManagerFactory;

public class CursorController : MonoBehaviour
{
    private ResourceManager resourceManager;
    private NestedCondition selectConditions;
    private UnityAction<Vector3Int> selectedCallback;

    private HexMap mapIns;
    private CrossInput crossInput;
    private UnityEventSystem eventSystem;
    private Popup popupIns;

    private RangeWayPointManager towerPositions;
    private NodeManagerFactory managerFactory;

    public CursorPos Cursor;
    public CameraController CameraController;

    public event UnityAction<Vector3Int> SelectedCallback
    {
        add { selectedCallback += value; }
        remove { selectedCallback -= value; }
    }
    public Vector3Int SelectedPosition
    {
        get; private set;
    }

    public HexMap MapIns
    {
        get
        {
            return mapIns ?? (mapIns = Singleton.Instance<HexMap>());
        }
    }
    public Popup PopupIns
    {
        get
        {
            return popupIns ?? (popupIns = Singleton.Instance<Popup>());
        }
    }
    public CrossInput CrossInput
    {
        get
        {
            return crossInput ?? (crossInput = EventSystem?.CrossInput);
        }
    }
    public Camera CameraRaycaster
    {
        get
        {
            return CameraController.TargetCamera;
        }
    }
    public UnityEventSystem EventSystem
    {
        get
        {
            return eventSystem ?? (eventSystem = eventSystem = Singleton.Instance<UnityEventSystem>());
        }
    }

    public ResourceManager ResourceManager
    {
        get
        {
            return resourceManager ?? (resourceManager = Singleton.Instance<ResourceManager>());
        }
    }

    public NodeManagerFactory ManagerFactory
    {
        get
        {
            return managerFactory ?? (managerFactory = Singleton.Instance<NodeManagerFactory>());
        }
    }

    public RangeWayPointManager ConstructNodeManager
    {
        get
        {
            return towerPositions ?? (towerPositions = 
                ManagerFactory.GetManager<ConstructWayPoint>(NodeType.Range) as RangeWayPointManager);
        }
    }

    private void Awake()
    {
        selectConditions = new NestedCondition();
        selectConditions.Conditions +=
            delegate
            {
                return CrossInput.IsTouch && !EventSystem.IsPointerDownOverUI;
            };
        selectConditions.Conditions +=
            delegate
            {
                return CameraController.Gesture == EnumCollect.CameraGesture.None;
            };

        SelectedCallback += UpdateCursor;
        SelectedCallback += DetermineSelectedOnRSS;
    }

    private void Update()
    {
        if (selectConditions.Evaluate())
        {
            Vector3 mousePos = Input.mousePosition;

            bool raycastHitted = Physics.Raycast(
                CameraRaycaster.ScreenPointToRay(mousePos),
                out RaycastHit hitInfo,
                CameraRaycaster.farClipPlane);

            if (raycastHitted)
            {
                SelectedPosition = MapIns.WorldToCell(hitInfo.point).ZToZero();

                DetermineSelectedOnTower();
                // DetermineSelectedOnRSS(hitInfo);

                selectedCallback?.Invoke(SelectedPosition);
            }
        }
    }

    private void UpdateCursor(Vector3Int position)
    {
        Cursor.PositionCursor.SetPosTxt(position.x.ToString(), position.y.ToString());
        Cursor.updateCursor(MapIns.CellToWorld(position));
    }

    private void DetermineSelectedOnTower()
    {
        if (ConstructNodeManager.GetInfo(SelectedPosition, out NodeInfo result))
        {
            SelectedPosition = MapIns.WorldToCell(result.GameObject.transform.position).ZToZero();
        }
    }

    private void DetermineSelectedOnRSS(Vector3Int position)
    {
        bool isRss = ResourceManager.IsRssAtPosition(position, out int id);
        if (isRss)
        {
            ResourceManager[id].OpenPopup(PopupIns);
        }
    }

    public Vector3Int Position;
    [ContextMenu("Set Position")]
    public void SetPosition()
    {
        Cursor.updateCursor(MapIns.CellToWorld(Position));
    }

    //private void DetermineSelectedOnRSS(RaycastHit hitInfo)
    //{
    //    bool isRss = LayerMask.NameToLayer("RSS") == hitInfo.transform.gameObject.layer;
    //    if (isRss)
    //    {
    //        hitInfo.transform.GetComponent<NaturalResource>()?.OpenPopup(PopupIns);
    //    }
    //}

}
