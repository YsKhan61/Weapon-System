using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Weapon_System.GameplayObjects.ItemsSystem;


namespace Weapon_System.GameplayObjects.UI
{

    /// <summary>
    /// Manages the UIs of the guns in the inventory. [ Drag drop, swap, remove, add etc ]
    /// </summary>
    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public class WeaponItemUI : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        [SerializeField]
        Image m_Icon;

        [SerializeField]
        TextMeshProUGUI m_NameText;

        [SerializeField]
        WeaponUIMediator m_WeaponUIMediator;
        public WeaponUIMediator WeaponUIMediator => m_WeaponUIMediator;

        [SerializeField]
        int m_SlotIndex;
        public int SlotIndex => m_SlotIndex;

        private RectTransform m_RectTransform;
        private Canvas m_Canvas;
        private CanvasGroup m_CanvasGroup;

        private Transform m_LastParent;
        public Transform LastParent => m_LastParent;

        private Vector2 m_lastAnchoredPosition;
        
        private WeaponItem m_StoredWeapontem;
        /// <summary>
        /// This stores the GunItem data of this ItemUI, from the Inventory
        /// </summary>
        public WeaponItem StoredGunItem => m_StoredWeapontem;

        [HideInInspector]
        public bool IsDragSuccess;

        private void Awake()
        {
            m_Canvas = GetComponentInParent<Canvas>();
            if (m_Canvas == null)
            {
                Debug.LogError("No Canvas found in parent of " + gameObject.name);
                enabled = false;
                return;
            }

            m_CanvasGroup = GetComponent<CanvasGroup>();
            m_RectTransform = GetComponent<RectTransform>();

            ///<remarks>
            /// Hide the item by default in awake,
            /// as WeaponInventoryUI's ToggleInventoryUI(false) is called in Start
            /// So if Hide() is called in start,
            /// It will call Hide() after Show() in SetItemData
            /// </remarks>
            Hide();     
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // Right click to drop item
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                // Check if WeaponItem is present in the ItemUI
                if (m_StoredWeapontem == null)
                    return;

                m_WeaponUIMediator.TryRemoveWeaponItemFromInventory(m_StoredWeapontem, m_SlotIndex);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            m_lastAnchoredPosition = m_RectTransform.anchoredPosition;
            m_LastParent = m_RectTransform.parent;

            IsDragSuccess = false;

            m_CanvasGroup.blocksRaycasts = false;
            // Set the parent to the canvas so that the UI is not clipped by the parent
            transform.SetParent(WeaponUIMediator.CanvasTransform);
            m_CanvasGroup.alpha = 0.6f;
        }

        public void OnDrag(PointerEventData eventData)
        {
            m_RectTransform.anchoredPosition += eventData.delta * m_Canvas.scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            m_CanvasGroup.blocksRaycasts = true;
            m_CanvasGroup.alpha = 1f;

            if (!IsDragSuccess)
            {
                m_RectTransform.SetParent(m_LastParent);
                m_RectTransform.anchoredPosition = m_lastAnchoredPosition;
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (eventData.pointerDrag == null)
                return;

            if (eventData.pointerDrag.TryGetComponent(out WeaponItemUI droppedWeaponItemUI))
            {
                // If there is already a Stored WeaponItem in this ItemUI ,
                // Check if the GunItem Type of dropped WeaponItemUI is same as this WeaponItemUI
                if (StoredGunItem != null &&
                    droppedWeaponItemUI.StoredGunItem.ItemData.ItemTag == StoredGunItem.ItemData.ItemTag)
                {
                    return;
                }

                m_WeaponUIMediator.SwapWeaponItemsInInventory(droppedWeaponItemUI.SlotIndex, m_SlotIndex);
            }
            else if (eventData.pointerDrag.TryGetComponent(out ItemUI droppedItemUI))
            {
                if (droppedItemUI.StoredItem is not WeaponItem)
                    return;

                if (droppedItemUI.StoredItem.ItemData.UITag == m_WeaponUIMediator.ItemUITag)
                {
                    // This is an ItemUI of a Weapon Item,
                    m_WeaponUIMediator.TryAddWeaponAndDestroyItemUI(droppedItemUI);
                }
            }
        }

        public void SetSlotIndex(int index)
        {
            m_SlotIndex = index;
        }

        public void SetItemDataAndShow(WeaponItem item)
        {
            m_StoredWeapontem = item;
            m_Icon.sprite = StoredGunItem.ItemData.IconSprite;
            m_NameText.text = StoredGunItem.ItemData.name;

            Show();
        }

        public void ResetDataAndHideGunItemUI()
        {
            m_StoredWeapontem = null;
            m_Icon.sprite = null;
            m_NameText.text = "";

            Hide();
        }

        private void Show()
        {
            m_CanvasGroup.alpha = 1;
        }

        private void Hide()
        {
            m_CanvasGroup.alpha = 0;
        }
    }
}
