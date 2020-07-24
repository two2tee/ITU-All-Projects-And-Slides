// PortableUserDto.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System;
using System.Collections.Generic;

namespace LiftLog.Core.Dto.PortableDto
{
    /// <summary>
    ///     Represents all stored data of user ready to be ported
    /// </summary>
    public class PortableUserDto
    {
        //Retrieval Information
        public DateTime RetrievalDate { get; set; }

        //Account Details
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public DateTime CreationDate { get; set; }

        //Personal details
        public string Name { get; set; }
        public DateTime BirthDay { get; set; }
        public string SexType { get; set; }
        public string CountryName { get; set; }
        public int BodyWeight { get; set; }
        public int Height { get; set; }

        //Domain details
        public ICollection<PortableWorkoutDto> Workouts { get; set; }
        public int ChallengeGiven { get; set; }
        public int ChallengeReceived { get; set; }
    }
}