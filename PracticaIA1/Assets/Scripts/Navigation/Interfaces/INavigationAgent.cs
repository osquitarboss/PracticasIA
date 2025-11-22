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
using UnityEngine;

namespace Navigation.Interfaces
{
    /// <summary>
    /// This interface defines the methods and properties that a navigation agent must implement.
    /// </summary>
    public interface INavigationAgent
    {
        /// <summary>
        /// Current objective cell information. 
        /// </summary>
        public CellInfo CurrentObjective { get; }
        
        /// <summary>
        /// Current Destination position.
        /// </summary>
        public Vector3 CurrentDestination { get; }
        
        /// <summary>
        /// Number of destinations to visit.
        /// </summary>
        public int NumberOfDestinations { get; }

        /// <summary>
        /// Called to initialize the navigation agent with the world information and the navigation algorithm to use.
        /// </summary>
        /// <param name="worldInfo">World information</param>
        /// <param name="navigationAlgorithm">Navigation algorithm</param>
        public void Initialize(WorldInfo worldInfo, INavigationAlgorithm navigationAlgorithm);
        
        /// <summary>
        /// Called on every frame to update the object (the character).
        /// This method must return the next scene position to visit, starting at current position.
        /// </summary>
        /// <param name="currentPosition">Actual position</param>
        /// <returns>Vector3 position to visit</returns>
        public Vector3? GetNextDestination(Vector3 currentPosition);
    }
}