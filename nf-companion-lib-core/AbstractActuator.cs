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
using System.Text;

namespace nanoFramework.Companion
{
    /// <summary>
    /// This abstract actuator class implements basic/common actuator functions.
    /// Ideally, all actuator implementations should inherit from this class
    /// </summary>
    public abstract class AbstractActuator : IActuator
    {
        #region Implementation
        /// <summary>
        /// Dispose support
        /// </summary>
        private bool _disposed = false; // To detect redundant calls
        #endregion

        #region IDispose Implementation
        /// <summary>
        /// Dispose the object
        /// </summary>
        public virtual void Dispose()
        {
            if (!_disposed)
            {
                DisposeActuator();
                _disposed = true;
            }
        }
        /// <summary>
        /// Dispose the actuator
        /// </summary>
        protected abstract void DisposeActuator();
        #endregion

        #region Actuator Driver Methods
        /// <summary>
        /// Initialize the actuator
        /// </summary>
        public abstract void Initialize();
        /// <summary>
        /// Prepare to write to actuator
        /// </summary>
        public abstract void PrepareToWrite();
        /// <summary>
        /// Write value to actuator
        /// </summary>
        public abstract void Write();
        /// <summary>
        /// Reset the actuator
        /// </summary>
        public abstract void Reset();

        #endregion
    }
}
