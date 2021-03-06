﻿using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Ioc;
using SCMSClient.Models;
using SCMSClient.Services.Interfaces;
using SCMSClient.ToastNotification;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SCMSClient.ViewModel
{
    public class CardholdersVM : CollectionsVMWithTwoCommands<Cardholder>
    {
        private readonly IEmployeeService empService;
        private readonly ITenantService tenantService;

        #region Default Constructor

        public CardholdersVM(ICardholderService _cardholderService, IEmployeeService _empService,
            ITenantService _tenantService, IDinkeyDongleService _dongleService) : base(_service: _cardholderService, _dongleService: _dongleService)
        {
            empService = _empService;
            tenantService = _tenantService;

            AddCardholderCommand = new RelayCommand(OpenCardholderRegistrationPage);
        }

        #endregion Default Constructor

        public ICommand AddCardholderCommand { get; set; }

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

        #endregion Private Methods

        #region Command Methods

        protected async override void Process()
        {
            try
            {
                var detailsVm = SimpleIoc.Default.GetInstance<CardholderDetailsVM>();
                detailsVm.SelectedItem = await LoadCardholderInfo();

                var mainWindowVm = SimpleIoc.Default.GetInstance<MainWindowVM>();
                mainWindowVm.ActivePage = new Uri("/Views/CardholderDetails.xaml", UriKind.RelativeOrAbsolute);
            }
            catch (Exception ex)
            {
                toaster.ShowErrorToast(Toaster.ErrorTitle, ex.Message);
            }
        }

        private void OpenCardholderRegistrationPage()
        {
            var mainWindowVm = SimpleIoc.Default.GetInstance<MainWindowVM>();
            mainWindowVm.ActivePage = new Uri("/Views/RegisterCardholder.xaml", UriKind.RelativeOrAbsolute);
        }

        protected override void FilterCollections(object obj)
        {
            var filter = obj as string;

            var cardholders = AllObjects
                .Where(ch => ch.UserType.ToString().IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                                .ToList();

            FilteredCollection = new ObservableCollection<Cardholder>(cardholders);

            ChangeStyle(filter);
        }

        private async Task<Cardholder> LoadCardholderInfo()
        {
            if (SelectedObject?.UserType == SHCCardType.Employee)
            {
                return await RunMethodAsync(() => empService.Get(SelectedObject.ID), () => IsBusy);
            }
            if (SelectedObject?.UserType == SHCCardType.Tenant)
            {
                return await RunMethodAsync(() => tenantService.Get(SelectedObject.ID), () => IsBusy);
            }

            return null;
        }

        #endregion Command Methods
    }
}