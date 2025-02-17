
using UnityEngine;

namespace Weapon_System.GameplayObjects.ItemsSystem
{
    /// <summary>
    /// Any item in the game, must need to have a name and a reference to the ItemDataSO
    /// </summary>
    public interface IItemInfo
    {
        public string Name { get; }
        public ItemDataSO ItemData { get; }
    }

    /// <summary>
    /// Interface for the collectable items
    /// </summary>
    public interface ICollectable
    {
        public bool IsCollected { get; }

        public ItemDataSO ItemData { get; }

        /// <summary>
        /// Collect the item.
        /// If some items need to be attached to the collector's hand, then the collector's transform is also passed
        /// </summary>
        /// <param name="hand"></param>
        /// <returns></returns>
        public bool TryCollect(ItemUserHand hand);
    }


    /// <summary>
    /// Interface for the droppable items
    /// </summary>
    public interface IDroppable
    {
        public bool Drop();
    }


    /// <summary>
    /// Interface for the storable items
    /// eg: Ammo, Heals, Attachments, etc.
    /// An IStorable item must need to be ICollectable and IDroppable
    /// </summary>
    public interface IStorable : ICollectable, IDroppable
    {
        public bool TryStoreAndCollect(ItemUserHand hand);
        public bool TryRemoveAndDrop();

        /// <summary>
        /// Store the item in the inventory
        /// </summary>
        /// <returns></returns>
        public bool TryStore(ItemUserHand hand);

        public bool TryRemove();
    }


    /// <summary>
    /// Interface for the usable items - primary use of the item
    /// Fire, Punch, Throw, etc.
    /// </summary>
    public interface  IP_Usable
    {
        public bool PrimaryUseStarted();

        public bool PrimaryUseCanceled();
    }


    /// <summary>
    /// Interface for the usable items - secondary use of the item
    /// eg: ADS
    /// </summary>
    public interface IS_Usable
    {
        public bool SecondaryUseStarted();

        public bool SecondaryUseCanceled();
    }


    /// <summary>
    /// Interface for the items that can be held in hand
    /// 
    /// ------------REMARKS--------------------
    /// An item that can be hold by the hand, 
    /// must need to be put away also at some point of time.
    /// eg: Guns, Melee weapons, Throwable items, etc.
    /// </summary>
    public interface IHoldable
    {
        public bool IsInHand { get; }

        /// <summary>
        /// We pass the hand to hold the item in
        /// </summary>
        public bool Hold();

        /// <summary>
        /// This method is used to either put away the item in the inventory
        /// or drop it on the ground
        /// </summary>
        /// <returns></returns>
        public bool TryPutAway();
    }


    /// <summary>
    /// Interface for the weapon attachment items
    /// Considering that item can be attached as well as detached from the weapon
    /// eg: Muzzle attachments, Magazine attachments, Sight attachments, Foregrip attachments, Stock attachments etc.
    /// </summary>
    public interface IWeaponAttachment
    {
        public ItemDataSO ItemData { get; }
        public bool IsWeaponCompatible(WeaponDataSO weaponData);

        /// <summary>
        /// This method is used to attach the attachment to the weapon
        /// </summary>
        /// <param name="weaponItem">the gun on which the attachment will be attached</param>
        /// <returns></returns>
        public bool AttachToWeapon(WeaponItem weaponItem);
        public bool DetachFromWeapon();
    }
}