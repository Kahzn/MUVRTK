namespace MUVRTK
{
    using Photon.Pun;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using VRTK;
    /// <summary>
    /// Simple inventory based on VRTK's PanelMenuSaucerGrid.
    /// The object this script is on needs to have a InventoryPanelMenu-Object attached to it and alle references set accordingly.
    /// When picked up (grabbed), this script toggles the display of the menu by pressing "up" on the Touchpad.
    /// <para> Created by Katharina Ziolkowski, 2019.02.04</para>
    /// </summary>
    public class MUVRTK_Inventory : MonoBehaviour
    {
        #region Public Fields

        public bool debug;

        public GridLayoutGroup gridLayoutGroup;
        public MeshRenderer changeObject;
        public VRTK_PanelMenuItemController panelMenuController;
        public Color[] colours = new Color[0];
        public MUVRTK_SimpleSpawner simpleSpawner;
        public MUVRTK_SpeechBubble speechBubble;

        #endregion

        #region Protected Fields

        protected int currentIndex = 0;
        protected readonly Color colorDefault = Color.white;
        protected readonly Color colorSelected = Color.green;
        protected readonly float colorAlpha = 0.25f;

        #endregion

        #region enums

        protected enum Direction
        {
            None,
            Up,
            Down,
            Left,
            Right
        }
        #endregion

        #region Monobehaviour Callbacks

        protected virtual void OnEnable()
        {
            if (panelMenuController != null)
            {
                panelMenuController.PanelMenuItemSwipeTop += PanelMenuItemSwipeTop;
                panelMenuController.PanelMenuItemSwipeBottom += PanelMenuItemSwipeBottom;
                panelMenuController.PanelMenuItemSwipeLeft += PanelMenuItemSwipeLeft;
                panelMenuController.PanelMenuItemSwipeRight += PanelMenuItemSwipeRight;
                panelMenuController.PanelMenuItemTriggerPressed += PanelMenuItemTriggerPressed;
            }

            SetGridLayoutItemSelectedState(currentIndex);
        }

        #endregion

        #region VRTK methods

        protected virtual void PanelMenuItemTriggerPressed(object sender, PanelMenuItemControllerEventArgs e)
        {
            if (currentIndex < colours.Length && changeObject != null)
            {
                changeObject.material.color = colours[currentIndex];
            }

            Transform selected = gridLayoutGroup.transform.GetChild(currentIndex);

            if (selected != null)
            {
                string s = selected.GetComponentInChildren<Text>().text.ToString();
                PhotonView.Get(this).RPC("SetSpeechBubbleText", RpcTarget.All, s);

                if(debug)
                Debug.Log(name + " : the Text of the current item is:  " + selected.GetComponentInChildren<Text>().text.ToString());
            }
            else
            {
                if(debug)
                Debug.Log("No Selected Item!");
            }

            simpleSpawner.Spawn(speechBubble.gameObject, new Vector3(e.interactableObject.transform.position.x , e.interactableObject.transform.position.y + 0.5f , e.interactableObject.transform.position.z  ), transform.rotation);
        }

        protected virtual void PanelMenuItemSwipeRight(object sender, PanelMenuItemControllerEventArgs e)
        {
            MoveSelectGridLayoutItem(Direction.Right);
        }

        protected virtual void PanelMenuItemSwipeLeft(object sender, PanelMenuItemControllerEventArgs e)
        {
            MoveSelectGridLayoutItem(Direction.Left);
        }

        protected virtual void PanelMenuItemSwipeBottom(object sender, PanelMenuItemControllerEventArgs e)
        {
            MoveSelectGridLayoutItem(Direction.Down);
        }

        protected virtual void PanelMenuItemSwipeTop(object sender, PanelMenuItemControllerEventArgs e)
        {
            MoveSelectGridLayoutItem(Direction.Up);
        }

        protected virtual void SetGridLayoutItemSelectedState(int index)
        {
            foreach (Transform childTransform in gridLayoutGroup.transform)
            {
                GameObject child = childTransform.gameObject;
                if (child != null)
                {
                    Color color = colorDefault;
                    color.a = colorAlpha;
                    child.GetComponent<Image>().color = color;
                }
            }

            Transform selected = gridLayoutGroup.transform.GetChild(index);
            if (selected != null)
            {
                Color color = colorSelected;
                color.a = colorAlpha;
                selected.GetComponent<Image>().color = color;
            }
        }

        protected virtual bool MoveSelectGridLayoutItem(Direction direction)
        {
            int newIndex = FindNextItemBasedOnMoveDirection(direction);
            if (newIndex != currentIndex)
            {
                SetGridLayoutItemSelectedState(newIndex);
                currentIndex = newIndex;
            }
            return true;
        }

        protected virtual int FindNextItemBasedOnMoveDirection(Direction direction)
        {
            float width = gridLayoutGroup.preferredWidth;
            float cellWidth = gridLayoutGroup.cellSize.x;
            float spacing = gridLayoutGroup.spacing.x;
            int cellsAccross = (int)Mathf.Floor(width / (cellWidth + (spacing / 2))); // quick / dirty
            int childCount = gridLayoutGroup.transform.childCount;

            switch (direction)
            {
                case Direction.Up:
                    int nextUp = currentIndex - cellsAccross;
                    return (nextUp >= 0) ? nextUp : currentIndex;
                case Direction.Down:
                    int nextDown = currentIndex + cellsAccross;
                    return (nextDown < childCount) ? nextDown : currentIndex;
                case Direction.Left:
                    int nextLeft = currentIndex - 1;
                    return (nextLeft >= 0) ? nextLeft : currentIndex;
                case Direction.Right:
                    int nextRight = currentIndex + 1;
                    return (nextRight < childCount) ? nextRight : currentIndex;
                default:
                    return currentIndex;
            }
        }

        #endregion

        #region PUN RPCS

        [PunRPC]
        public void SetSpeechBubbleText(string s)
        {
            speechBubble.content.text = s;
        }

        #endregion

    }
}


