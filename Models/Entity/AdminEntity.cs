﻿using CaseManagementApp.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaseManagementApp.Models.Entity
{
    [Index(nameof(Email), IsUnique = true)]
    internal class AdminEntity : IUser
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = null!;

        [Required]
        [StringLength(100)]
        [Unicode(false)]
        public string Email { get; set; } = null!;

        [Required]
        public int AddressId { get; set; }
        public virtual AddressEntity Address { get; set; } = null!;

        public virtual ICollection<CaseEntity> Case { get; set; }


        public string FullName => $"{FirstName} {LastName}";

    }
}
