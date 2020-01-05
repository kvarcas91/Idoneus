using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Core.ViewModels.Base
{
    public class ParameterizedRelayCommand<T> : ICommand
    {
        #region Private Members

        /// <summary>
        /// The action to run
        /// </summary>
        private readonly Action<T> mAction;

        #endregion

        #region Public Events

        /// <summary>
        /// The event thats fired when the <see cref="CanExecute(object)"/> value has changed
        /// </summary>
        public event EventHandler CanExecuteChanged = (sender, e) => { };

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public ParameterizedRelayCommand(Action<T> action)
        {
            mAction = action;
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// A relay command can always execute
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Executes the commands Action
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            mAction((T)parameter);
        }


        #endregion
    }
}
