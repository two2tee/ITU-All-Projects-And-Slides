// PdfProvider.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System;
using System.Text;
using System.Threading.Tasks;
using LiftLog.Core.Dto.PortableDto;
using LiftLog.Service.Interfaces;
using Microsoft.AspNetCore.NodeServices;

namespace LiftLog.Service.FileProviders
{
    /// <summary>
    ///     This file provider will return a pdf of personal data
    /// </summary>
    public class PdfProvider : IFIleProvider<PortableUserDto>
    {
        private readonly INodeServices _nodeServices;

        public PdfProvider(INodeServices nodeServices)
        {
            _nodeServices = nodeServices;
        }

        /// <summary>
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<FileDto> MakeFileAsync(PortableUserDto input)
        {
            var rawContent = FormatContent(input); //Raw content ready to be converted to pdf bytes
            var pdfContent = await ExecutePdfGeneration(rawContent);

            var dto = new FileDto
            {
                FileName =
                    $"LiftLog_StoredData_Of_{input.DisplayName}_{DateTime.Now.Day}_{DateTime.Now.Month}_{DateTime.Now.Year}.pdf",
                FileExtention = "pdf",
                FileContent = pdfContent
            };

            return dto;
        }

        /// <summary>
        ///     https://stackoverflow.com/questions/39364687/export-html-to-pdf-in-asp-net-core
        /// </summary>
        /// <returns></returns>
        private async Task<byte[]> ExecutePdfGeneration(string rawData)
        {
            var fileContent = await _nodeServices.InvokeAsync<byte[]>("./pdf", rawData);

            return fileContent;
        }

        /// <summary>
        ///     This function will formate data from user and create raw HTML which a pdf renderer will use
        /// </summary>
        private string FormatContent(PortableUserDto user)
        {
            var builder = new StringBuilder();

            //Account and personal data
            builder.Append($"<h1> Liftlog - Your personal data </h1>" +
                           $"<h2> Description</h2>" +
                           $"<div>This document contains all of your current personal data stored in Liftlog." +
                           $"<br/>This document was generated on <b>{DateTime.Now:F}</b></div>" +
                           $"<h2> Personal Data</h2>" +
                           $"This sections contains all personal data that describes who you are." +
                           $"<div><ul><li><b>Name: </b>{user.Name}</li>" +
                           $"<li><b> Birth date: </b>{user.BirthDay:d}</li>" +
                           $"<li><b> Sex: </b>{user.SexType}</li >" +
                           $"<li><b> Weight:</b> {user.BodyWeight}</li>" +
                           $"<li><b> Height:</b> {user.Height}</li>" +
                           $"<li><b> Country:</b> {user.CountryName}</li></ul>" +
                           $"<h2> Account Data</h2>" +
                           $"This sections contains all information about your account at LiftLog<div><ul>" +
                           $"<li><b>Creation date: </b>{user.CreationDate:d}</li>" +
                           $"<li><b>Display name: </b>{user.DisplayName}</li>" +
                           $"<li><b>Email: </b>{user.Email}</li></ul></div>" +
                           $"<h2> Challenge data </h2>" +
                           $"This sections contains information on your challenges. We cannot provide you details on the challenges because they contain personal data of others as well.But rest assured that they onlycontain an exercise created or received and a reference to another user." +
                           $"<div><ul><li><b> Challenge Given: </b>{user.ChallengeGiven}</li>" +
                           $"<li><b> Challenge Received: </b>{user.ChallengeReceived}</li></ul></div>");

            //Workout
            builder.Append($"<h2> Workout Data </h2>" +
                           $"This sections contains all your workouts and their entries stored at LiftLog" +
                           $"<li><b> Total workouts:</b>{user.Workouts.Count}</li>");

            //Adding workouts and their entries 

            foreach (var workout in user.Workouts)
            {
                builder.Append($"<h3>Workout: {workout.Name} </h3><div>" +
                               $"<ul><li><b>Creation Date:</b>{workout.CreationDate:d}</li>" +
                               $"</ul><b> Entries </b><li>" +
                               $"Total Entries: {workout.WorkoutEntryDtos.Count}</li>");

                foreach (var entry in workout.WorkoutEntryDtos)
                    builder.Append(
                        $"<br/><b>Exercise: {entry.ExerciseName}</b>" +
                        $"<li>Set: {entry.Set}</li>" +
                        $"<li>Weight: {entry.Weight}</li>" +
                        $"<li>Reps: {entry.Reps}</li></div>");
            }

            return builder.ToString();
        }
    }
}