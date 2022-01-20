﻿using CaseManagementApp.Data;
using CaseManagementApp.Models;
using CaseManagementApp.Models.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaseManagementApp.Services
{
    internal class SqlService
    {
        private SqlContext _context;

        public  SqlService()
        {
            _context = new SqlContext();
        }


        #region CREATE
        public int CreateAddress(Address address)
        {
            var _address = _context.Addresses.Where(x => x.StreetName == address.StreetName && x.ZipCode == address.ZipCode).FirstOrDefault();
            if (_address == null)
            {
                var addressEntity = new AddressEntity() { StreetName = address.StreetName, ZipCode = address.ZipCode, City = address.City, Country = address.Country};
                _context.Addresses.Add(addressEntity);
                _context.SaveChanges();
                return addressEntity.Id;
            }

            return _address.Id;
        }

       
        public int CreateCustomer(Customer customer)
        {
            var _customer = _context.Customers.Where(x => x.Email == customer.Email).FirstOrDefault();       
            if (_customer == null)
            {
                var customerEntity = new CustomerEntity();

                customerEntity.FirstName = customer.FirstName;
                customerEntity.LastName = customer.LastName;
                customerEntity.Email = customer.Email;
                customerEntity.PhoneNumber = customer.PhoneNumber;
                customerEntity.AddressId = CreateAddress(customer.Address);
                

                _context.Customers.Add(customerEntity);
                _context.SaveChanges();

                return customerEntity.Id;
            }

            else
                return -1;

        }

        public int CreateAdmin(Admin admin)
        {
            var _admin = _context.Admins.Where(x => x.Email == admin.Email).FirstOrDefault();
            if (_admin == null)
            {
                var adminEntity = new AdminEntity();

                adminEntity.FirstName = admin.FirstName;
                adminEntity.LastName = admin.LastName;
                adminEntity.Email = admin.Email;
                adminEntity.AddressId = CreateAddress(admin.Address);


                _context.Admins.Add(adminEntity);
                _context.SaveChanges();

                return adminEntity.Id;
            }
            else
                return -1;
        }

        public int CreateCase(Case _case)
        {
            var caseEntity = new CaseEntity();
            var _customer = _context.Customers.Where(x => x.Id == _case.Customer.Id).FirstOrDefault();
            var _admin = _context.Admins.Where(x => x.Id == _case.Admin.Id).FirstOrDefault();


            caseEntity.Subject = _case.Subject;
            caseEntity.Description = _case.Description;
            caseEntity.State = _case.State.ToString();
            caseEntity.Customer = _customer;
            caseEntity.Admin = _admin;

            _context.Cases.Add(caseEntity);
            _context.SaveChanges();


            return caseEntity.CaseId;
        }

        #endregion

        #region READ

        //Single result

        public AddressEntity GetAddress(int id)
        {
            return _context.Addresses.SingleOrDefault(x => x.Id == id);
        }

        public CustomerEntity GetCustomer(int id)
        {
            return _context.Customers.Include(x => x.Address).SingleOrDefault(x =>x.Id == id);
        }

        public AdminEntity GetAdmin(int id)
        {
            return _context.Admins.Include(x => x.Address).SingleOrDefault(x => x.Id == id);
        }


        public CaseEntity GetCase(int id)
        {
            return _context.Cases.Include(x => x.Customer).Include(x => x.Admin).SingleOrDefault(x => x.CaseId == id);
        }

        //Multiple results

        public async Task<IEnumerable<AddressEntity>> GetAddressesAsync()
        {
            return await Task.FromResult(_context.Addresses);
        }

        public async Task<IEnumerable<CustomerEntity>> GetCustomersAsync()
        {
            return await Task.FromResult(_context.Customers.Include(x => x.Address));
        }

        public async Task<IEnumerable<AdminEntity>> GetAdminsAsync()
        {
            return await Task.FromResult(_context.Admins.Include(x => x.Address));
        }
        
        public async Task<IEnumerable<CaseEntity>> GetCasesAsync()
        {
            return await Task.FromResult(_context.Cases.Include(x => x.Customer).Include(x => x.Admin));
        }


        #endregion

        #region UPDATE

        public void UpdateAddress(int id, AddressEntity address)
        {
            var patchedAddress = _context.Addresses.Find(id);

            if (patchedAddress != null && patchedAddress.Id == id)
            {
                patchedAddress.StreetName = address.StreetName;
                patchedAddress.ZipCode = address.ZipCode;
                patchedAddress.City = address.City;
                patchedAddress.Country = address.Country;

                _context.Update(patchedAddress);
                _context.SaveChanges();
            }
        }


        public void UpdateCustomer(int id, CustomerEntity customer)
        {
            var patchedCustomer = _context.Customers.Find(id);

            if(patchedCustomer != null && patchedCustomer.Id == id)
            {
                patchedCustomer.FirstName = customer.FirstName;
                patchedCustomer.LastName = customer.LastName;
                patchedCustomer.Email = customer.Email;
                patchedCustomer.AddressId = customer.AddressId;

                _context.Update(patchedCustomer);
                _context.SaveChanges();
            }
        }

        public void UpdateAdmin(int id, AdminEntity admin)
        {
            var patchedAdmin = _context.Admins.Find(id);

            if(patchedAdmin != null && patchedAdmin.Id == id)
            {
                patchedAdmin.FirstName = admin.FirstName;
                patchedAdmin.LastName = admin.LastName;
                patchedAdmin.Email = admin.Email;
                patchedAdmin.AddressId = admin.AddressId;

                _context.Update(patchedAdmin);
                _context.SaveChanges();
            }
        }

        public void UpdateCase(int id, CaseEntity newCase)
        {
            var patchedCase = _context.Cases.Find(id);

            if(patchedCase != null && patchedCase.CaseId == id)
            {
                patchedCase.Admin = newCase.Admin;
                patchedCase.State = newCase.State;
                patchedCase.UpdatedDate = DateTime.Now;

                _context.Update(patchedCase);
                _context.SaveChanges();
            }
        }


        #endregion

    }

}

