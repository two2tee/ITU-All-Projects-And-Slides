// NewUserDto.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System;
using LiftLog.Core.Enums;

namespace LiftLog.Core.Dto
{
    public class NewUserDto
    {
        public int UserId { get; set; }
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public Sex Sex { get; set; }
        public Country Country { get; set; }
        public int BodyWeight { get; set; }
        public int Height { get; set; }
    }
}