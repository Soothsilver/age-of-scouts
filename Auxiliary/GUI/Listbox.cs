using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Auxiliary.GUI
{
    /// <summary>
    /// Represents an Auxiliary list box control.
    /// </summary>
    /// <typeparam name="T">This type should override ToString().</typeparam>
    public class Listbox<T> : UIElement
    {
    
        /// <summary>
        /// Triggers whenever an item is selected. It is guaranteed the selected item will not be null when this is called.
        /// </summary>
        public event Action<Listbox<T>, object> ItemSelected;
        /// <summary>
        /// Calls the ItemSelected event.
        /// </summary>
        protected virtual void OnItemSelected(Listbox<T> listbox, object item)
        {
            if (ItemSelected != null)
                ItemSelected(listbox, item);
        }
        /// <summary>
        /// Triggers whenever an item is clicked or is selected and the Enter key is pressed.
        /// </summary>
        public event Action<Listbox<T>, object> ItemConfirmed;
        /// <summary>
        /// Calls the ItemConfirmed event.
        /// </summary>
        protected virtual void OnItemConfirmed(Listbox<T> listbox, object item)
        {
            if (ItemConfirmed != null)
                ItemConfirmed(listbox, item);
        }
        /// <summary>
        /// The collection of objects in this listbox.
        /// </summary>
        public List<T> Items = new List<T>();
        private int selectedIndex = -1;
        private int mouseOverIndex = -1;

        /// <summary>
        /// Gets or sets the index of the selected item. Returns -1 when no item is selected. Deselects anything if set to an invalid index.
        /// </summary>
        public int SelectedIndex
        {
            get { if (selectedIndex >= Items.Count) return -1; else return selectedIndex; }
            set
            {
                if (value >= Items.Count || value < -1)
                    selectedIndex = -1;
                else
                {
                    selectedIndex = value;
                    OnItemSelected(this, SelectedItem);
                }
            }
        }
        /// <summary>
        /// Gets or sets the selected item. Returns null if no item is selected. Throws exception of set to an object not in the listbox.
        /// </summary>
        public T SelectedItem
        {
            get { if (selectedIndex == -1) return default(T); else return Items[selectedIndex]; }
            set
            {
                if (value is T)
                {
                    T input = (T)value;
                    if (Items.Contains(input))
                    {
                        SelectedIndex = Items.IndexOf(input);
                    }
                    else throw new Exception("This item is not in the listbox.");
                }
                else throw new Exception("This item is not of the type accepted by this Listbox<T>.");
            }
        }

        /// <summary>
        /// Update the control. The base method causes the control to become active and consumes the left-click if clicked.
        /// </summary>
        public override void Update()
        {
            if (Root.WasMouseLeftClick)
            {
                if (Root.IsMouseOver(Rectangle))
                {
                    Root.ConsumeLeftClick();
                    if (mouseOverIndex != -1)
                    {
                        if (mouseOverIndex == SelectedIndex)
                            OnItemConfirmed(this, SelectedItem);
                        else
                        {
                            SelectedIndex = mouseOverIndex;
                        }
                    }
                    else SelectedIndex = -1;
                }
            }
            if (Root.WasKeyPressed(Keys.Down))
                if (SelectedIndex < Items.Count - 1) SelectedIndex++;
            if (Root.WasKeyPressed(Keys.Up))
                if (SelectedIndex > 0) SelectedIndex--;
            if (Root.WasKeyPressed(Keys.Home))
                if (Items.Count > 0) SelectedIndex = 0;
            if (Root.WasKeyPressed(Keys.End))
                SelectedIndex = Items.Count - 1;
            if (Root.WasKeyPressed(Keys.Enter))
            {
                if (SelectedIndex != -1) OnItemConfirmed(this, SelectedItem);
            }
            base.Update();
        }

        /// <summary>
        /// Draws the control.
        /// </summary>
        public override void Draw()
        {
            mouseOverIndex = -1;
            Color outerBorderColor = Skin.OuterBorderColor;
            Color innerBorderColor = Skin.InnerBorderColor;
            Color innerButtonColor = Skin.WhiteBackgroundColor;
            Primitives.FillRectangle(Rectangle, innerBorderColor);
            Primitives.DrawRectangle(Rectangle, outerBorderColor, Skin.OuterBorderThickness);
            Primitives.DrawAndFillRectangle(InnerRectangleWithBorder, innerButtonColor, outerBorderColor, Skin.OuterBorderThickness);
            for (int i = TopOfList; i < Items.Count; i++)
            {
                Rectangle rectItem = new Rectangle(InnerRectangle.X + 1, InnerRectangle.Y + Skin.ListItemHeight * (i - TopOfList) + 1, InnerRectangle.Width - 2, Skin.ListItemHeight);
                if (Root.IsMouseOver(rectItem))
                    mouseOverIndex = i;

                if (selectedIndex == i)
                    Primitives.FillRectangle(rectItem, Skin.ItemSelectedBackgroundColor);
                else if (mouseOverIndex == i)
                    Primitives.FillRectangle(rectItem, Skin.ItemMouseOverBackgroundColor);
                Primitives.DrawSingleLineText(Items[i].ToString(), new Vector2(InnerRectangle.X + 5, InnerRectangle.Y + 2 + Skin.ListItemHeight * (i - TopOfList)), Skin.TextColor, Skin.Font);
                Primitives.DrawLine(new Vector2(InnerRectangle.X, InnerRectangle.Y + Skin.ListItemHeight * (i - TopOfList + 1)),
                                    new Vector2(InnerRectangle.Right, InnerRectangle.Y + Skin.ListItemHeight * (i - TopOfList + 1)),
                                    outerBorderColor, Skin.OuterBorderThickness);
            }
        }

        private const int TopOfList = 0;

        /// <summary>
        /// Creates a new Auxiliary listbox.
        /// </summary>
        /// <param name="rect">Space occupied by the listbox.</param>
        public Listbox(Rectangle rect)
        {
            Rectangle = rect;
        }
    }
}