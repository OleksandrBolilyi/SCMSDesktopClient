﻿using SCMSClient.Modals;
using SCMSClient.Models;
using SCMSClient.Services.Interfaces;
using SCMSClient.ToastNotification;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace SCMSClient.ViewModel
{
    /// <summary>
    /// ViewModel Logic for the CardRequest's Page
    /// </summary>
    public class CardRequestsVM : CollectionsVMWithOneCommand<SOACardRequest>
    {
        private readonly ICardRequestService service;

        #region Default Constructor

        public CardRequestsVM(ICardRequestService _service)
        {
            service = _service;

            LoadAll().ConfigureAwait(false);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Implementation of the Logic to Load all <see cref="SOACardRequest"/>
        /// </summary>
        protected override async Task LoadAll()
        {
            try
            {
                await Task.Run(() =>
                {
                    var allrequests = service.GetAll() ?? new List<SOACardRequest>();
                    AllObjects = new ObservableCollection<SOACardRequest>(allrequests);
                });

            }
            catch (Exception e)
            {
                toaster.ShowErrorToast(Toaster.ErrorTitle, e.Message);
            }
        }

        /// <summary>
        /// Implementaton of the Logic to filter the Collection
        /// </summary>
        /// <param name="obj">
        /// represents an object in the collection
        /// </param>
        /// <returns>
        /// returns true if the filter condition passes or
        /// false if the filter condition fails
        /// </returns>
        protected override bool SearchFilter(object obj)
        {
            var request = obj as SOACardRequest;

            return request?.RequestedBy?.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0
                || request?.CardType.Name?.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0
                || request?.RequestId?.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0
                || request?.BusinessUnit?.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        #endregion


        #region Command Methods

        /// <summary>
        /// Implementation of the Logic to process the
        /// Selected <see cref="SOACardRequest"/> Object
        /// </summary>
        protected override void Process()
        {
            var modal = new DistributeCards(SelectedObject);

            MessengerInstance.Send<UIElement>(modal);
        }

        #endregion
    }
}
