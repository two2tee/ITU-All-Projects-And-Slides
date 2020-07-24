// User.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using LiftLog.Core.Enums;
using LiftLog.Data.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace LiftLog.Data.Entities
{
    /// <summary>
    ///     Represents a single user in the system
    /// </summary>
    public class User : IdentityUser<int>, IEntity
    {
        // Email, UserName and Id part of Identity User 
        public User()
        {
            Workouts = new List<Workout>();
            Rankings = new List<Ranking>();
            GivenChallenges = new List<Challenge>();
            ReceivedChallenges = new List<Challenge>();
        }

        public Guid ExportToken { get; set; }

        public DateTime CreationDate { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public DateTime BirthDate { get; set; }

        public Sex Sex { get; set; }

        public Country Country { get; set; }

        public long Height { get; set; }

        public long BodyWeight { get; set; }

        public virtual ICollection<Workout> Workouts { get; set; }

        public virtual ICollection<Ranking> Rankings { get; set; }

        [InverseProperty("ChallengerUser")]
        public virtual ICollection<Challenge> GivenChallenges { get; set; }

        [InverseProperty("ChallengeeUser")]
        public virtual ICollection<Challenge> ReceivedChallenges { get; set; }
    }
}