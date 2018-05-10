using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;

namespace Auxiliary.GUI
{
    /// <summary>
    /// Abstract base class to all Auxiliary GUI elements, such as Button or Textbox.
    /// </summary>
    public abstract class UIElement
    {
        /// <summary>
        /// Gets or sets the skin used by this control.
        /// </summary>
        public GuiSkin Skin { get; set; }
        /// <summary>
        /// Space occupied by this control.
        /// </summary>
        public Rectangle Rectangle;
        /// <summary>
        /// Space occupied by the inner part of this control, excluding the border.
        /// </summary>
        public Rectangle InnerRectangle
        {
            get
            {
                return new Rectangle(Rectangle.X + Skin.TotalBorderThickness, Rectangle.Y + Skin.TotalBorderThickness, Rectangle.Width - 2 * Skin.TotalBorderThickness, Rectangle.Height - 2 * Skin.TotalBorderThickness);
            }
        }
        /// <summary>
        /// ??? Is this the same thing as Rectangle? Why would I write this?
        /// </summary>
        public Rectangle InnerRectangleWithBorder
        {
            get
            {
                return new Rectangle(Rectangle.X + Skin.OuterBorderThickness + Skin.InnerBorderThickness, Rectangle.Y + Skin.OuterBorderThickness + Skin.InnerBorderThickness, Rectangle.Width - 2 * (Skin.OuterBorderThickness + Skin.InnerBorderThickness), Rectangle.Height - 2 * (Skin.OuterBorderThickness + Skin.InnerBorderThickness));

            }
        }
        /// <summary>
        /// Draws the control.
        /// </summary>
        public abstract void Draw();
        /// <summary>
        /// Update the control. The base method causes the control to become active and consumes the left-click if clicked.
        /// </summary>
        public virtual void Update()
        {
            if (Root.WasMouseLeftClick && Root.IsMouseOver(Rectangle))
            {
                Activate();
                Root.ConsumeLeftClick();
            }
        }
        /// <summary>
        /// Causes the control to become active (gain focus).
        /// </summary>
        public virtual void Activate()
        {
            Root.GuiActiveElement = this;
        }
        /// <summary>
        /// Deactivates all controls (causes them to lose focus).
        /// </summary>
        public virtual void Deactivate()
        {
            Root.GuiActiveElement = null;
        }
        /// <summary>
        /// Returns true if this control is active.
        /// </summary>
        public bool IsActive
        {
            get
            {
                return Root.GuiActiveElement == this;
            }
        }
        /// <summary>
        /// Sets the skin to the default skin.
        /// </summary>
        protected UIElement()
        {
            Skin = GuiSkin.DefaultSkin;
        }
    }
    /// <summary>
    /// Holds instruction on colors, sizes and fonts to use in Auxiliary GUI elements using this skin.
    /// </summary>
    public class GuiSkin
    {
        /// <summary>
        /// The color in the inner outline when mouse is over the control.
        /// </summary>
        public Color InnerBorderColorMouseOver { get; set; }
        /// <summary>
        /// The color in the inner outline when mouse is over the control and its left button is held.
        /// </summary>
        public Color InnerBorderColorMousePressed { get; set; }
        /// <summary>
        /// The color in the inner outline.
        /// </summary>
        public Color InnerBorderColor { get; set; }
        /// <summary>
        /// The size, in pixels, of the inner outline.
        /// </summary>
        public int InnerBorderThickness { get; set; }
        /// <summary>
        /// The color of the outer outline.
        /// </summary>
        public Color OuterBorderColor { get; set; }
        /// <summary>
        /// The color of the outer outline if mouse is over the control.
        /// </summary>
        public Color OuterBorderColorMouseOver { get; set; }
        /// <summary>
        /// The size, in pixels, of the outer outline.
        /// </summary>
        public int OuterBorderThickness { get; set; }
        /// <summary>
        /// Gets twice the outer outline width + the inner outline width.
        /// </summary>
        public int TotalBorderThickness
        {
            get
            {
                return 2 * OuterBorderThickness + InnerBorderThickness;
            }
        }
        /// <summary>
        /// The color of the background, if in Windows, it would be gray.
        /// </summary>
        public Color GreyBackgroundColor { get; set; }
        /// <summary>
        /// The color of the normally gray background when mouse is over the control.
        /// </summary>
        public Color GreyBackgroundColorMouseOver { get; set; }
        /// <summary>
        /// The background color of the form, if any.
        /// </summary>
        public Color DialogBackgroundColor { get; set; }
        /// <summary>
        /// The background color which would normally be white in Windows, such as in TextBox controls.
        /// </summary>
        public Color WhiteBackgroundColor { get; set; }
        /// <summary>
        /// The background color of a selected item in a listbox.
        /// </summary>
        public Color ItemSelectedBackgroundColor { get; set; }
        /// <summary>
        /// The background color of an item in a listbox, if mouse is over it.
        /// </summary>
        public Color ItemMouseOverBackgroundColor { get; set; }
        /// <summary>
        /// Font used by the control.
        /// </summary>
        public SpriteFont Font { get; set; }
        /// <summary>
        /// Color of the text of the control.
        /// </summary>
        public Color TextColor { get; set; }
        /// <summary>
        /// Color of the text if mouse is over the control.
        /// </summary>
        public Color TextColorMouseOver { get; set; }
        /// <summary>
        /// Height of items in a listbox.
        /// </summary>
        public int ListItemHeight { get; set; }

        /// <summary>
        /// Creates a deep copy of this skin.
        /// </summary>
        public GuiSkin Clone()
        {
            GuiSkin newSkin = new GuiSkin();
            Type skinType = this.GetType();
            FieldInfo[] fields = skinType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo fi in fields)
            {
                fi.SetValue(newSkin, fi.GetValue(this));
            }
            return newSkin;
        }

        /// <summary>
        /// Holds the default skin used by Auxiliary controls (blue).
        /// </summary>
        public static GuiSkin DefaultSkin;
        /// <summary>
        /// Holds the simplistic skin, which has a thin inner border.
        /// </summary>
        public static GuiSkin SimplisticSkin;
        static GuiSkin()
        {
            DefaultSkin = new GuiSkin
                              {
                                  InnerBorderColorMouseOver = Color.Aquamarine,
                                  InnerBorderColor = Color.LightBlue,
                                  InnerBorderColorMousePressed = Color.DarkBlue,
                                  InnerBorderThickness = 3,
                                  OuterBorderColor = Color.Black,
                                  OuterBorderColorMouseOver = Color.Black,
                                  OuterBorderThickness = 1,
                                  Font = Library.FontVerdana,
                                  TextColor = Color.Black,
                                  TextColorMouseOver = Color.Black,
                                  GreyBackgroundColor =
                                      Color.FromNonPremultiplied(Color.CornflowerBlue.R + 20, Color.CornflowerBlue.G + 20,
                                                                 Color.CornflowerBlue.B + 20, 255),
                                  GreyBackgroundColorMouseOver =
                                      Color.FromNonPremultiplied(Color.CornflowerBlue.R + 26, Color.CornflowerBlue.G + 26,
                                                                 Color.CornflowerBlue.B + 26, 255),
                                  WhiteBackgroundColor = Color.White,
                                  DialogBackgroundColor =
                                      Color.FromNonPremultiplied(Color.CornflowerBlue.R + 45, Color.CornflowerBlue.G + 45,
                                                                 Color.CornflowerBlue.B + 45, 255),
                                  ListItemHeight = 30,
                                  ItemSelectedBackgroundColor = Color.PowderBlue,
                                  ItemMouseOverBackgroundColor = Color.LightYellow
                              };

            SimplisticSkin = DefaultSkin.Clone();
            SimplisticSkin.InnerBorderColor = Color.MediumAquamarine;
            SimplisticSkin.InnerBorderThickness = 1;
        }
    }
}
