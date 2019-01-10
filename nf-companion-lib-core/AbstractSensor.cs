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
using System.Threading;

namespace nanoFramework.Companion
{
    /// <summary>
    /// This abstract class contains implementation, that is common to
    /// all sensors.Every new sensor driver implementation should
    /// inherit from this class
    /// </summary>
    public abstract class AbstractSensor : ISensor
    {
        #region Implementation
        /// <summary>
        /// Is this sensor tracking changes
        /// </summary>
        protected  bool _isTrackingChanges = false;
        /// <summary>
        /// The thread that keeps a track of sensor value change
        /// </summary>
        protected Thread _changeTracker = null;

        /// <summary>
        /// Dispose support
        /// </summary>
        private bool _disposed = false; // To detect redundant calls
        #endregion

        #region Events/Delegates
        /// <summary>
        /// Delegate that defines method signature that will be called
        /// when sensor value change event happens
        /// </summary>
        public delegate void OnSensorChanged();
        /// <summary>
        /// Event that is called when the sensor value changes
        /// </summary>
        public event OnSensorChanged SensorValueChanged;
        #endregion

        #region IDispose Implementation
        /// <summary>
        /// Dispose the object
        /// </summary>
        public virtual void Dispose()
        {
            if (!_disposed)
            {
                DisposeSensor();
                _disposed = true;
            }
        }
        /// <summary>
        /// Dispose the sensor
        /// </summary>
        protected abstract void DisposeSensor();
        #endregion

        #region Change tracking
        /// <summary>
        /// Is this sensor capable of tracking changes
        /// </summary>
        /// <returns>bool</returns>
        public virtual bool CanTrackChanges()
        {
            return false;
        }
        /// <summary>
        /// Start to track the changes
        /// </summary>
        /// <param name="ms">Interval in milliseconds to track the changes to sensor values</param>
        public virtual void BeginTrackChanges(int ms)
        {
            if (_isTrackingChanges) throw new InvalidOperationException("Already tracking changes");
            if (ms < 50) throw new ArgumentOutOfRangeException("ms", "Minimum interval to track sensor changes is 50 milliseconds");
            if (SensorValueChanged == null) throw new NotSupportedException("Tracking not supported if SensorValueChanged event is not defined");

            _changeTracker = new Thread(() => {
                int divs = (int)(ms / 1000);
                
                while (_isTrackingChanges)
                {
                    if (ms > 1000)
                    {
                        divs = (int)(ms / 1000);
                        while (_isTrackingChanges && divs > 0)
                        {
                            Thread.Sleep(1000);
                            divs--;
                        }
                    }
                    else
                        Thread.Sleep(ms);
                    //now check for change
                    if (HasSensorValueChanged() && SensorValueChanged != null)
                    {
                        try { SensorValueChanged(); } catch {; ; /*do nothing..upto event handler to decide what to do*/ }
                    }
                }

            });
            _isTrackingChanges = true;
            _changeTracker.Start();
        }
        /// <summary>
        /// The sensor driver implementation should decide what is the meaning
        /// of change of a sensor value. We just check if the value of sensor has changed or not.
        /// Some sensor driver implementors may not want this automatic check and may want to have a 
        /// polling mechanism for the client applications. The client apps,can use this method in their own
        /// polling implementation and check for value changes
        /// </summary>
        /// <returns>If true, then sensor value has changed else false</returns>
        public abstract bool HasSensorValueChanged();
        /// <summary>
        /// Stop tracking changes
        /// </summary>
        public virtual void EndTrackChanges()
        {
            _isTrackingChanges = false;
            Thread.Sleep(3000);//see BeginChangeTracker to know why 3000 is chosen...3x of lowest wait time
            if (_changeTracker.IsAlive)
            {
                //force kill
                try { _changeTracker.Abort(); } finally { _changeTracker = null; }
            }
        }
        #endregion

        #region Calibration
        /// <summary>
        /// Apply calibration to the measured value..using 2-point calibration method
        /// Apply calibration formula to readings...see https://learn.adafruit.com/calibrating-sensors/two-point-calibration
        /// </summary>
        /// <param name="rawLow">Raw value low</param>
        /// <param name="rawRange">Raw value range</param>
        /// <param name="refLow">Reference low</param>
        /// <param name="refRange">Reference range</param>
        /// <param name="measuredVal">Value to calibrate</param>
        /// <returns>Calibrated value</returns>
        public float Apply2PointCalibration(float rawLow, float rawRange, float refLow, float refRange, float measuredVal)
        {
            if (refRange == 0) throw new ArgumentException();
            if (rawRange == 0) throw new ArgumentException();
            return (((measuredVal - rawLow) * refRange) / rawRange) + refLow;
        }
        #endregion

        #region Abstract Sensor Methods
        /// <summary>
        /// Initialize the sensor
        /// </summary>
        public abstract void Initialize();
        /// <summary>
        /// Prepare sensor for reading value
        /// </summary>
        public abstract void PrepareToRead();
        /// <summary>
        /// Read sensor value
        /// </summary>
        /// <returns>true if  read successful else false</returns>
        public abstract bool Read();
        /// <summary>
        /// Reset sensor
        /// </summary>
        public abstract void Reset();
        #endregion
        
    }
}
