﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Restaurant.API.Models.DTO
{
    public class OrderHeaderUpdateDTO
    {
        [Key]
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        
        public double OrderTotal { get; set; }
        public int ItemsTotal { get; set; }
        public string? OrderStatus { get; set; }
        //public string? PaymentStatus { get; set; }
        //public string? TrackingNumber { get; set; }
        //public string? Carrier { get; set; }
        public string? PaymentIntentId { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }


        public IEnumerable<OrderDetail> OrderDetail { get; set; }
    }
}
