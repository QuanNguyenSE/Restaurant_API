﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.API.Models
{
    public class OrderHeader
    {
        [Key]
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }

        public DateTime OrderDate { get; set; }
        //public DateTime PaymentDate { get; set; }
        //public DateTime? ShippingDate { get; set; }
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
