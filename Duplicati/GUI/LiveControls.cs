﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Duplicati.GUI
{
    /// <summary>
    /// This class keeps track of the users modifications regarding
    /// throttling and pause/resume
    /// </summary>
    public class LiveControls
    {
        /// <summary>
        /// An event that is activated when the pause state changes
        /// </summary>
        public event EventHandler StateChanged;

        /// <summary>
        /// An event that is activated when the thread priority changes
        /// </summary>
        public event EventHandler ThreadPriorityChanged;

        /// <summary>
        /// An event that is activated when the throttle speed changes
        /// </summary>
        public event EventHandler ThrottleSpeedChanged;

        /// <summary>
        /// The possible states for the live control
        /// </summary>
        public enum LiveControlState
        {
            /// <summary>
            /// Indicates that the backups are running
            /// </summary>
            Running,
            /// <summary>
            /// Indicates that the backups are currently suspended
            /// </summary>
            Paused
        }

        /// <summary>
        /// The current control state
        /// </summary>
        private LiveControlState m_state;

        /// <summary>
        /// Gets the current state for the control
        /// </summary>
        public LiveControlState State { get { return m_state; } }

        /// <summary>
        /// The internal variable that tracks the the priority
        /// </summary>
        private System.Threading.ThreadPriority? m_priority;

        /// <summary>
        /// The internal variable that tracks the upload limit
        /// </summary>
        private long? m_uploadLimit;

        /// <summary>
        /// The internal variable that tracks the download limit
        /// </summary>
        private long? m_downloadLimit;

        /// <summary>
        /// Gets the current overridden thread priority
        /// </summary>
        public System.Threading.ThreadPriority? ThreadPriority 
        { 
            get { return m_priority; }
            set
            {
                if (m_priority != value)
                {
                    m_priority = value;
                    if (ThreadPriorityChanged != null)
                        ThreadPriorityChanged(this, null);
                }
            }
        }

        /// <summary>
        /// Gets the current upload limit in bps
        /// </summary>
        public long? UploadLimit 
        { 
            get { return m_uploadLimit; }
            set
            {
                if (m_uploadLimit != value)
                {
                    m_uploadLimit = value;
                    if (ThrottleSpeedChanged != null)
                        ThrottleSpeedChanged(this, null);
                }
            }
        }

        /// <summary>
        /// Gets the download limit in bps
        /// </summary>
        public long? DownloadLimit 
        { 
            get { return m_downloadLimit; }
            set
            {
                if (m_downloadLimit != value)
                {
                    m_downloadLimit = value;
                    if (ThrottleSpeedChanged != null)
                        ThrottleSpeedChanged(this, null);
                }
            }
        }

        /// <summary>
        /// The timer that is activated after a pause period.
        /// We use a Windows.Forms timer to ensure that the event is raised
        /// in the correct thread (the UI thread).
        /// </summary>
        private System.Windows.Forms.Timer m_waitTimer;

        /// <summary>
        /// Constructs a new instance of the LiveControl
        /// </summary>
        /// <param name="initialTimeout">The duration that the backups should be initially suspended</param>
        public LiveControls(Datamodel.ApplicationSettings settings)
        {
            m_state = LiveControlState.Running;
            m_waitTimer = new System.Windows.Forms.Timer();
            m_waitTimer.Tick += new EventHandler(m_waitTimer_Tick);
            if (!string.IsNullOrEmpty(settings.StartupDelayDuration) && settings.StartupDelayDuration != "0")
            {
                m_waitTimer.Interval = (int)Duplicati.Library.Core.Timeparser.ParseTimeSpan(settings.StartupDelayDuration).TotalMilliseconds;
                m_waitTimer.Enabled = true;
                m_state = LiveControlState.Paused;
            }

            m_priority = settings.ThreadPriorityOverride;
            if (!string.IsNullOrEmpty(settings.DownloadSpeedLimit))
                m_downloadLimit = Library.Core.Sizeparser.ParseSize(settings.DownloadSpeedLimit, "kb");
            if (!string.IsNullOrEmpty(settings.UploadSpeedLimit))
                m_uploadLimit = Library.Core.Sizeparser.ParseSize(settings.UploadSpeedLimit, "kb");
        }

        /// <summary>
        /// Event that occurs when the timeout duration is exceeded
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">An unused event argument</param>
        private void  m_waitTimer_Tick(object sender, EventArgs e)
        {
            Resume();
        }

        /// <summary>
        /// Internal helper to reset the timeout timer
        /// </summary>
        /// <param name="timeout">The time to wait</param>
        private void ResetTimer(string timeout)
        {
            m_waitTimer.Enabled = false;
            if (!string.IsNullOrEmpty(timeout))
            {
                m_waitTimer.Interval = ((int)Duplicati.Library.Core.Timeparser.ParseTimeSpan(timeout).TotalMilliseconds);
                m_waitTimer.Enabled = true;
            }
        }

        /// <summary>
        /// Pauses the backups until resumed
        /// </summary>
        public void Pause()
        {
            if (m_state == LiveControlState.Running)
            {
                m_state = LiveControlState.Paused;
                if (StateChanged != null)
                    StateChanged(this, null);
            }
        }

        /// <summary>
        /// Resumes a backups to the running state
        /// </summary>
        public void Resume()
        {
            if (m_state == LiveControlState.Paused)
            {
                //Make sure that the timer is cleared
                m_waitTimer.Enabled = false;

                m_state = LiveControlState.Running;
                if (StateChanged != null)
                    StateChanged(this, null);
            }
        }

        /// <summary>
        /// Suspends the backups for a given period
        /// </summary>
        /// <param name="timeout">The duration to wait</param>
        public void Pause(string timeout)
        {
            if (m_state == LiveControlState.Running)
            {
                Pause();
                m_waitTimer.Enabled = false;
                m_waitTimer.Interval = ((int)Duplicati.Library.Core.Timeparser.ParseTimeSpan(timeout).TotalMilliseconds);
                m_waitTimer.Enabled = true;
            }
        }
    }
}