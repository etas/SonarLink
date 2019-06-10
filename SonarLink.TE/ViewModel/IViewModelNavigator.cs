// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

namespace SonarLink.TE.ViewModel
{
    /// <summary>
    /// Switches views and the associated view-models
    /// </summary>
    public interface IViewModelNavigator
    {
        /// <summary>
        /// Switches the view model to the requested one (if available)
        /// </summary>
        /// <param name="name">view-model identifier</param>
        void ChangeViewModel(string name);
    }
}
