﻿using GalaSoft.MvvmLight.Ioc;
using SCMSClient.Models;
using SCMSClient.Services.Interfaces;
using SCMSClient.ToastNotification;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SCMSClient.ViewModel
{
    public class CardholdersVM : CollectionsVMWithTwoCommands<Cardholder>
    {
        private IEmployeeService empService;
        private ITenantService tenantService;

        #region Default Constructor

        public CardholdersVM(ICardholderService _cardholderService, IEmployeeService _empService,
            ITenantService _tenantService) : base(_service: _cardholderService)
        {
            empService = _empService;
            tenantService = _tenantService;
        }

        #endregion


        #region Tests

        /// <summary>
        /// This is a method thar runs on the initialization of the class
        /// to load all objects using the service class
        /// </summary>
        /// <returns></returns>
        protected override async Task LoadAll()
        {
            try
            {
                await RunMethodAsync(() =>
                {
                    if (AllObjects?.Count > 0)
                        AllObjects.Clear();

                    if (FilteredCollection?.Count > 0)
                        FilteredCollection.Clear();

                    var allObjects = empService.GetAll().Cast<Cardholder>().ToList() ??
                    new List<Employee>().Cast<Cardholder>().ToList();
                    AllObjects = FilteredCollection = new ObservableCollection<Cardholder>(allObjects);
                }, () => IsBusy);
            }
            catch (Exception e)
            {
                toaster.ShowErrorToast(Toaster.ErrorTitle, e.Message);
            }
        }

        #endregion


        #region Private Methods

        protected override bool SearchFilter(object obj)
        {
            var cardholder = obj as Cardholder;

            if (cardholder?.FullName?.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0
                || cardholder?.IdentificationNo?.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }

            return false;
        }

        #endregion


        #region Command Methods

        protected override void Process()
        {
            var detailsVm = SimpleIoc.Default.GetInstance<CardholderDetailsVM>("new");
            detailsVm.SelectedItem = SelectedObject;

            var mainWindowVm = SimpleIoc.Default.GetInstance<MainWindowVM>();
            mainWindowVm.ActivePage = new Uri("/Views/CardholderDetails.xaml", UriKind.RelativeOrAbsolute);
        }

        protected override void FilterCollections(object obj)
        {
            var filter = obj as string;

            var cardholders = AllObjects
                .Where(ch => ch.Cards
                                .Any(c => c.CardType.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0))
                                .ToList();

            FilteredCollection = new ObservableCollection<Cardholder>(cardholders);
        }

        #endregion
    }
}
