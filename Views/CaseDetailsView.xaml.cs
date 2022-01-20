﻿using CaseManagementApp.Models;
using CaseManagementApp.Models.Entity;
using CaseManagementApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CaseManagementApp.Views
{

    enum ToggleState
    {
        EditCase,
        SaveCase
    }

    /// <summary>
    /// Interaction logic for CaseDetailsView.xaml
    /// </summary>
    public partial class CaseDetailsView : UserControl
    {

        private readonly SqlService customerService = new();
        private readonly SqlService adminService = new();
        private readonly SqlService caseService = new();
        private readonly SqlService updateCaseService = new();
        private readonly SqlService updateAdminService = new();
        private readonly List<AdminEntity> _admins = new();

        public CaseDetailsView()
        {
            InitializeComponent();
            GetLists();
            PopulateComboBoxes();
        }

      

        private async void PopulateComboBoxes()
        {
            await FillCustomerOptionsAsync();
            await FillAdminOptionsAsync();
            await FillStausOptionsAsync();
            ArrengeComboBox();
        }

        private void GetLists()
        {
            GetCustomerList();
            GetAdminList();
        }

        private AdminEntity SetValues(CaseEntity caseResult)
        {
            lblCaseId.Content = caseResult.CaseId;
            lblSubject.Content = caseResult.Subject;
            tbDescription.Text = caseResult.Description;
            lblAdmins.Content = caseResult.Admin.FullName;
            lblState.Content = caseResult.State;

            return caseResult.Admin;
        }

        private CaseEntity GetCase()
        {
            var caseId = (int)cbCustomersCase.SelectedValue;
            var _case = caseService.GetCase(caseId);

            SetValues(_case);

            return _case;
        }

        private async void GetCustomerList()
        {
            List<CustomerEntity> customerList = new();
            foreach(var customer in await customerService.GetCustomersAsync())
            {
                customerList.Add(customer);
            }
        }

        private async void GetAdminList()
        {
            List<AdminEntity> adminList = new();
            foreach (var admin in await adminService.GetAdminsAsync())
            {
                adminList.Add(admin);
            }
        }

        private async Task FillCustomerOptionsAsync()
        {
            //cbCustomersCase.Items.Clear();

            foreach (var _case in await customerService.GetCasesAsync())
            {
                cbCustomersCase.Items.Add(new KeyValuePair<int, string>(_case.CaseId, $"{_case.CaseId}: {_case.Customer.Email} <{_case.Subject}>"));
                //cbCustomers.Items.Add(customer);
            }
        }

        private async Task FillAdminOptionsAsync()
        {
            //cbAdmins.Items.Clear();


            foreach (var admin in await adminService.GetAdminsAsync())
            {
                cbAdmins.Items.Add(new KeyValuePair<int, string>(admin.Id, $"{admin.Id} : {admin.FirstName} {admin.LastName}"));
                _admins.Add(admin);
                //cbAdmins.Items.Add(admin);
            }
        }

        private async Task FillStausOptionsAsync()
        {
            cbState.Items.Clear();

            foreach (var _state in await Task.FromResult(Enum.GetValues(typeof(Status))))
            {
                cbState.Items.Add(new KeyValuePair<int, string>((int)_state, _state.ToString()));
            }
        }

        private void ArrengeComboBox()
        {
            cbCustomersCase.SelectedValuePath = "Key";
            cbCustomersCase.DisplayMemberPath = "Value";

            cbAdmins.SelectedValuePath = "Key";
            cbAdmins.DisplayMemberPath = "Value";

            cbState.SelectedValuePath = "Key";
            cbState.DisplayMemberPath = "Value";
        }

        private int UpdateCaseAdmin()
        {          
            if (cbAdmins.SelectedValue != null)
            {
                var newAdminId = (int)cbAdmins.SelectedValue;
                return newAdminId;
            }

            else
                return -1;
            
            

        }

        private string UpdateCaseState()
        {
            var newCaseState = (Status)Enum.Parse(typeof(Status), cbState.SelectedValue.ToString());
            return newCaseState.ToString();
        }

        private void btnSaveCase_Click(object sender, RoutedEventArgs e)
        {


            var _case = GetCase();
            int caseId = _case.CaseId;


            var newAdminId = UpdateCaseAdmin();
            string newState = UpdateCaseState();

            var listOfAdmins = FillAdminOptionsAsync();
            
            if(newAdminId > 0)
            {
                var newAdmin = _admins.Where(x => x.Id == newAdminId).FirstOrDefault();
                CaseEntity patchedCase = new()
                {
                    Admin = newAdmin,
                    State = newState,
                    UpdatedDate = DateTime.Now
                };

                updateCaseService.UpdateCase(caseId, patchedCase);
            }

            else
            {                          
                CaseEntity patchedCase = new()
                {
                    Admin = _case.Admin,
                    State = newState
                };

                updateCaseService.UpdateCase(caseId, patchedCase);
            }

            
            


            
            
        }

        private void cbCustomers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var _case = GetCase();
            var state = (Status)Enum.Parse(typeof(Status),_case.State);
            

            cbAdmins.SelectedValue = _case.Admin.Id;

        }
    }
}
