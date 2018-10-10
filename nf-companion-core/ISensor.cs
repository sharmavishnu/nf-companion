/**
Copyright 2018 Vishnu Sharma , (Twitter : @getvishnu)

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION 
WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
using System;

namespace nanoFramework.Companion
{
    /// <summary>
    /// This interface defines the sensor contract. Every sensor driver must implement this interface
    /// </summary>
    public interface ISensor : IDisposable
    {
        /// <summary>
        /// Initialize the sensor
        /// </summary>
        void Initialize();
        /// <summary>
        /// Prepare to read sensor
        /// </summary>
        void PrepareToRead();

        /// <summary>
        /// Read the sensor
        /// </summary>
        /// <returns>true if read successful, else false</returns>
        bool Read();

        /// <summary>
        /// Reset the sensor
        /// </summary>
        void Reset();
        /// <summary>
        /// If this sensor can track changes
        /// </summary>
        /// <returns>bool</returns>
        bool CanTrackChanges();
        /// <summary>
        /// Start tracking changes
        /// </summary>
        /// <param name="ms">Track changes to the sensor in how many milliseconds interval</param>
        void BeginTrackChanges(int ms);
        /// <summary>
        /// Check if sensor value has changed or not
        /// </summary>
        /// <returns>bool</returns>
        bool HasSensorValueChanged();
        /// <summary>
        /// Stop tracking changes
        /// </summary>
        void EndTrackChanges();        
    }
}
