// IContext.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System;
using System.Threading;
using System.Threading.Tasks;
using LiftLog.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LiftLog.Data.Interfaces
{
    /// <summary>
    ///     The main context of Liftlog
    /// </summary>
    public interface IContext : IDisposable
    {
        DbSet<Exercise> Exercises { get; set; }
        DbSet<Ranking> Rankings { get; set; }
        DbSet<Workout> Workouts { get; set; }
        DbSet<WorkoutEntry> WorkoutEntries { get; set; }
        DbSet<User> Users { get; set; }
        Task<int> SaveChangesAsync(CancellationToken token = default(CancellationToken)); // TODO what is this?
        DbSet<T> Set<T>() where T : class;
    }
}