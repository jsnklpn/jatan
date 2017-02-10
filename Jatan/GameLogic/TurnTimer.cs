using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Jatan.GameLogic
{
    /// <summary>
    /// Timer class to notify when a player's turn should be skipped.
    /// </summary>
    internal class TurnTimer : IDisposable
    {
        private Timer _internalTimer;
        private int _timeLimitSeconds;
        private int _currentPlayerId;
        private DateTime _lastStarted;
        private object _timerLock = new object();

        /// <summary>
        /// Event which fires when the time limit expires. The integer data is the playerId
        /// </summary>
        public event EventHandler<TimeLimitElapsedArgs> TimeLimitElapsed;

        /// <summary>
        /// Creates a new turn timer instance.
        /// </summary>
        /// <param name="timeLimit">The time limit in seconds.</param>
        public TurnTimer(int timeLimit)
        {
            _timeLimitSeconds = timeLimit;
            _lastStarted = DateTime.MinValue;

            if (_timeLimitSeconds <= 0)
            {
                // If the time limit is set to zero, completely disable the timer.
                _internalTimer = null;
            }
            else
            {
                _internalTimer = new Timer(_timeLimitSeconds * 1000);
                _internalTimer.Elapsed += InternalTimerOnElapsed;
                _internalTimer.AutoReset = false; // Fire event only once.    
            }
        }

        private void InternalTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            OnTimeLimitElapsed();
            _lastStarted = DateTime.MinValue;
        }

        protected void OnTimeLimitElapsed()
        {
            var handler = TimeLimitElapsed;
            if (handler != null)
            {
                handler(this, new TimeLimitElapsedArgs(_currentPlayerId));
            }
        }

        /// <summary>
        /// Starts the turn timer for the specified player.
        /// </summary>
        public void Start(int playerId)
        {
            lock (_timerLock)
            {
                if (_internalTimer == null) return;
                _currentPlayerId = playerId;
                _internalTimer.Stop();
                _internalTimer.Start();
                _lastStarted = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Stops the timer.
        /// </summary>
        public void Stop()
        {
            lock (_timerLock)
            {
                if (_internalTimer == null) return;
                _internalTimer.Stop();
                _lastStarted = DateTime.MinValue;
            }
        }

        /// <summary>
        /// Returns the UTC DateTime when the timer will expire next.
        /// Returns DateTime.MinValue if there timer is not running.
        /// </summary>
        public DateTime GetExpirationDateTime()
        {
            if (_timeLimitSeconds <= 0 || _lastStarted == DateTime.MinValue)
                return DateTime.MinValue;

            return _lastStarted.AddSeconds(_timeLimitSeconds);
        }

        /// <summary>
        /// Disposes the internal timer.
        /// </summary>
        public void Dispose()
        {
            lock (_timerLock)
            {
                if (_internalTimer != null)
                {
                    try
                    { _internalTimer.Dispose(); }
                    catch
                    { }
                    finally
                    { _internalTimer = null; }
                }
            }
        }
    }

    public class TimeLimitElapsedArgs : EventArgs
    {
        /// <summary>
        /// The Id of the player whose turn time limit expired.
        /// </summary>
        public int PlayerId { get; private set; }

        /// <summary>
        /// Creates a new instance of turn time limit event args.
        /// </summary>
        /// <param name="playerId">The Id of the current player.</param>
        public TimeLimitElapsedArgs(int playerId)
        {
            this.PlayerId = playerId;
        }
    }
}
