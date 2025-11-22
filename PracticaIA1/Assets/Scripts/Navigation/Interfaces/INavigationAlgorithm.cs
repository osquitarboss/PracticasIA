#region Copyright
// MIT License
// 
// Copyright (c) 2023 David María Arribas
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion

using Navigation.World;

namespace Navigation.Interfaces
{
    /// <summary>
    /// This interface defines the methods and properties that a navigation algorithm must implement.
    /// A navigation algorithm calculates the path between two points in the world.
    /// </summary>
    public interface INavigationAlgorithm
    {
        /// <summary>
        /// Called to initialize the navigation algorithm with the world information.
        /// </summary>
        /// <param name="worldInfo">World information</param>
        public void Initialize(WorldInfo worldInfo);
        
        /// <summary>
        /// Called to obtain the path between two points in the world.
        /// </summary>
        /// <param name="startNode">Starting point</param>
        /// <param name="targetNode">Ending point</param>
        /// <returns></returns>
        public CellInfo[] GetPath(CellInfo startNode, CellInfo targetNode);
    }
}