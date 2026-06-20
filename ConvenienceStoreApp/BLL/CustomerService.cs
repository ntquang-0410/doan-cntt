using System;
using System.Collections.Generic;
using ConvenienceStoreApp.DAL;
using ConvenienceStoreApp.DTOs;
using ConvenienceStoreApp.Models;

namespace ConvenienceStoreApp.BLL
{
    public class CustomerService
    {
        private readonly CustomerRepository _repository;

        public CustomerService()
            : this(new CustomerRepository())
        {
        }

        public CustomerService(CustomerRepository repository)
        {
            _repository = repository;
        }

        public List<CustomerListItemDto> SearchCustomers(string keyword)
        {
            return _repository.Search(keyword);
        }

        public Customer GetCustomer(int id)
        {
            return _repository.GetById(id);
        }

        public List<LoyaltyHistoryDto> GetLoyaltyHistory(int customerId)
        {
            return _repository.GetLoyaltyHistory(customerId);
        }

        public void SaveCustomer(int id, string name, string phone, string email, bool isActive)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Vui lòng nhập họ tên khách hàng.");
            }

            if (string.IsNullOrWhiteSpace(phone))
            {
                throw new ArgumentException("Vui lòng nhập số điện thoại khách hàng.");
            }

            Customer customer = new Customer
            {
                Id = id,
                Name = name.Trim(),
                Phone = phone.Trim(),
                Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim(),
                IsActive = isActive
            };

            if (id <= 0)
            {
                _repository.Create(customer);
            }
            else
            {
                _repository.Update(customer);
            }
        }
    }
}
