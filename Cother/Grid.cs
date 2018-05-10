using System.Collections;
using System.Collections.Generic;

namespace Cother
{
    /// <summary>
    /// A generic class that represent a 2D grid of tiles, useful for maps (in the cartographic sense, not the discrete mathematics sense).
    /// </summary>
    /// <typeparam name="T">Type of tiles.</typeparam>
    public class Grid<T> : IEnumerable<GridColumn<T>>
    {
        internal List<GridColumn<T>> InnerGrid; 
        /// <summary>
        /// Gets the width of the grid in tiles.
        /// </summary>
        public int Width { get; private set; }
        /// <summary>
        /// Gets the height of the grid in tiles.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Gets a column of tiles based on its 'X' coordinate.
        /// </summary>
        /// <param name="x">Specify the 'X' coordinate now.</param>
        public GridColumn<T> this[int x]
        {
            get { return InnerGrid[x]; }
        }

        /// <summary>
        /// Creates a 2D grid of tiles with the specified unmodifiable dimensions. All tiles will be initialized to null or their default value if not class instances.
        /// </summary>
        /// <param name="width">Width of the grid.</param>
        /// <param name="height">Height of the grid.</param>
        public Grid(int width, int height)
        {
            Width = width;
            Height = height;
            InnerGrid = new List<GridColumn<T>>();
            for (int x = 0; x <width;x++)
            {
                GridColumn<T> gridColumn = new GridColumn<T>(height);
                InnerGrid.Add(gridColumn);
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<GridColumn<T>> GetEnumerator()
        {
            return InnerGrid.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns true if the specified coordinate exists within the Grid (i.e. both coordinates are greater or equal to zero and less than the width or height, respectively.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        public bool Exists(int x, int y)
        {
            return x >= 0 && y >= 0 && x < Width && y < Height;
        }
    }
    /// <summary>
    /// This class represents a single column of tiles from the grid. Has internal constructor only.
    /// </summary>
    /// <typeparam name="T">Type of tiles.</typeparam>
    public sealed class GridColumn<T> : IEnumerable<T>
    {
        private readonly List<T> tiles; 
        internal GridColumn(int height)
        {
            tiles = new List<T>();
            for (int y =0 ;y<height;y++)
            {
                this.tiles.Add(default(T));
            }
        }

        /// <summary>
        /// Gets or set the tile at this X,Y position of the grid.
        /// </summary>
        /// <param name="index">Specify the 'Y' coordinate now.</param>
        public T this[int index]
        {
            get { return tiles[index]; }
            set { tiles[index] = value; }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<T> GetEnumerator()
        {
            return tiles.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
