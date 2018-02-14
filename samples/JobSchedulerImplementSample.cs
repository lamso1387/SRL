using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SRL.SchedulerManager;

namespace SchedulerConsoleApp
{
    /// <summary>
    /// Class which executes the program.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Method which instantiate the job manager and execute
        /// all jobs.
        /// </summary>
        /// <param name="args">Arguments of execution. They are not
        /// used here.</param>
        static void Main(string[] args)
        {
            // execute in ths project (asembly)
            JobManager jobManager = new JobManager();
            jobManager.ExecuteAllJobs();
        }
    }


    /// <summary>
    /// A simple job which is executed only once.
    /// </summary>
    class SimgleExecutionJob : Job
    {
        /// <summary>
        /// Get the Job Name, which reflects the class name.
        /// </summary>
        /// <returns>The class Name.</returns>
        public override string GetName()
        {
            return this.GetType().Name;
        }

        /// <summary>
        /// Execute the Job itself. Just print a message.
        /// </summary>
        public override void DoJob()
        {
            System.Console.WriteLine(String.Format("The Job \"{0}\" was executed.", this.GetName()));
        }

        /// <summary>
        /// Determines this job is not repeatable.
        /// </summary>
        /// <returns>Returns false because this job is not repeatable.</returns>
        public override bool IsRepeatable()
        {
            return false;
        }

        /// <summary>
        /// In case this method is executed NotImplementedException is thrown
        /// because this method is not to to be used. This method is never used
        /// because it serves the purpose of stating the interval of which the job
        /// will be executed repeatedly. Since this job is a single-execution one,
        /// this method is rendered useless.
        /// </summary>
        /// <returns>Returns nothing because this method is not to be used.</returns>
        public override int GetRepetitionIntervalTime()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// A simple repeatable Job.
    /// </summary>
    class RepeatableJob : Job
    {
        /// <summary>
        /// Counter used to count the number of times this job has been
        /// executed.
        /// </summary>
        private int counter = 0;

        /// <summary>
        /// Get the Job Name, which reflects the class name.
        /// </summary>
        /// <returns>The class Name.</returns>
        public override string GetName()
        {
            return this.GetType().Name;
        }

        /// <summary>
        /// Execute the Job itself. Just print a message.
        /// </summary>
        public override void DoJob()
        {
            System.Console.WriteLine(String.Format("This is the execution number \"{0}\" of the Job \"{1}\".", counter.ToString(), this.GetName()));
            counter++;
        }

        /// <summary>
        /// Determines this job is repeatable.
        /// </summary>
        /// <returns>Returns true because this job is repeatable.</returns>
        public override bool IsRepeatable()
        {
            return true;
        }

        /// <summary>
        /// Determines that this job is to be executed again after
        /// 1 sec.
        /// </summary>
        /// <returns>1 sec, which is the interval this job is to be
        /// executed repeatadly.</returns>
        public override int GetRepetitionIntervalTime()
        {
            return 1000;
        }
    }


    /// <summary>
    /// A simple repeatable Job.
    /// </summary>
    class RepeatableJob2 : Job
    {
        /// <summary>
        /// Counter used to count the number of times this job has been
        /// executed.
        /// </summary>
        private int counter = 0;

        /// <summary>
        /// Get the Job Name, which reflects the class name.
        /// </summary>
        /// <returns>The class Name.</returns>
        public override string GetName()
        {
            return this.GetType().Name;
        }

        /// <summary>
        /// Execute the Job itself. Just print a message.
        /// </summary>
        public override void DoJob()
        {
            System.Console.WriteLine(String.Format("This is the execution number \"{0}\" of the Job \"{1}\".", counter.ToString(), this.GetName()));
            counter++;
        }

        /// <summary>
        /// Determines this job is repeatable.
        /// </summary>
        /// <returns>Returns true because this job is repeatable.</returns>
        public override bool IsRepeatable()
        {
            return true;
        }

        /// <summary>
        /// Determines that this job is to be executed again after
        /// 1 sec.
        /// </summary>
        /// <returns>1 sec, which is the interval this job is to be
        /// executed repeatadly.</returns>
        public override int GetRepetitionIntervalTime()
        {
            return 1000;
        }
    }
}
