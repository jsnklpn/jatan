using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jatan.Core
{
    /// <summary>
    /// Class that returns the result of an action
    /// </summary>
    public class ActionResult
    {
        /// <summary>
        /// Indicates if the action succeeded.
        /// </summary>
        public bool Succeeded { get; protected set; }

        /// <summary>
        /// Indicates if the action failed.
        /// </summary>
        public bool Failed { get { return !Succeeded; } }

        /// <summary>
        /// A message to return. Usually it's the reason that the action failed.
        /// </summary>
        public string Message { get; protected set; }

        /// <summary>
        /// Creates a new action result.
        /// </summary>
        public ActionResult(bool succeeded, string message = "")
        {
            Succeeded = succeeded;
            Message = message;
        }

        /// <summary>
        /// Creates a new failed result.
        /// </summary>
        public static ActionResult CreateFailed(string message = "")
        {
            return new ActionResult(false, message);
        }

        /// <summary>
        /// Creates a new success result.
        /// </summary>
        /// <returns></returns>
        public static ActionResult CreateSuccess()
        {
            return new ActionResult(true);
        }

        /// <summary>
        /// Converts to a generic result.
        /// </summary>
        public ActionResult<T> ToGeneric<T>()
        {
            return new ActionResult<T>(default(T), Succeeded, Message);
        }
    }

    /// <summary>
    /// Class that returns the result of an action
    /// </summary>
    public class ActionResult<T> : ActionResult
    {
        /// <summary>
        /// The returned result of the action.
        /// </summary>
        public T Data { get; protected set; }

        /// <summary>
        /// Creates a new action result.
        /// </summary>
        public ActionResult(T data, bool success, string message = "") : base(success, message)
        {
            Data = data;
        }

        /// <summary>
        /// Creates a new success result.
        /// </summary>
        public new static ActionResult<T> CreateSuccess(T data)
        {
            return new ActionResult<T>(data, true, "");
        }

        /// <summary>
        /// Creates a new failed result.
        /// </summary>
        public new static ActionResult<T> CreateFailed(string message)
        {
            return new ActionResult<T>(default(T), false, message);
        }

        /// <summary>
        /// Creates a new result.
        /// </summary>
        public new static ActionResult<T> Create(ActionResult result)
        {
            return new ActionResult<T>(default(T), result.Succeeded, result.Message);
        }
    }
}
