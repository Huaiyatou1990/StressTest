using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace WinForm_StressTest
{
    class RateProfile
    {
        private int time_interval;      // profile cycle (ms)
        private double data_size;        // Kb
        private double average_rate;     // average rate since profile start (Kb/s)
        private double total_data_size;
        private double total_time_interval;
        private double current_rate;     // rate in last cycle (Kb/s)
        private Timer timer;             // timer to control profile cycle
        private object mylock;
        private int first_update;
        private Update update_data;

        private int task_num_total;     // num of tasks completed in whole profile process
        private int task_num_current;   // num of tasks completed in one profile cycle
        private double time_consumed_total;     // time consumed in whole profile process (ms)
        private double time_consumed_current;   // time consumed in one profile cycle (ms)
        private double average_latency; // average time needs for each task in whole profile process
        private double current_latency; // average time needs for each task in current profile cycle

        public RateProfile(int time_interval_in_ms, Update update_data)
        {
            if (time_interval_in_ms <= 0)
                throw new ArgumentOutOfRangeException();

            time_interval = time_interval_in_ms;
            data_size = 0;
            average_rate = 0;
            current_rate = 0;
            first_update = 0;
            mylock = new object();
            this.update_data = update_data;

            task_num_current = task_num_total = 0;
            time_consumed_current = time_consumed_total = 0;
            average_latency = current_latency = 0;
        }

        ~RateProfile()
        {
            ProfileStop();
        }

        public void ProfileStart()
        {
            RestDataAll();
            timer = new Timer(UpdateRate, null, 0, time_interval);
        }

        public void ProfileIn(double datasize, double time_consumed)
        {
            if (datasize <= 0)
                return;

            if (timer == null)
                throw new Exception("Profile Not start! Do ProfileStart() first.");

            lock (mylock)
            {
                data_size += datasize;
                task_num_total++;
                task_num_current++;
                time_consumed_total += time_consumed;
                time_consumed_current += time_consumed;
            }
        }

        public void ProfileStop()
        {
            if (timer != null)
            {
                timer.Dispose();
                timer = null;
            }
        }

        public double GetCurrentRate()
        {
            double temp = 0;
            lock (mylock)
            {
                temp = current_rate;
            }
            return temp;
        }

        public double GetAverageRate()
        {
            double temp = 0;
            lock (mylock)
            {
                temp = average_rate;
            }

            return temp;
        }

        private void UpdateRate(object state)
        {
            lock (mylock)
            {
                if (first_update++ < 3)  //ignore for the first few updates
                {
                    ResetData();
                    return;
                }

                current_rate = data_size * 1000.0 / time_interval;  //Kb/s

                total_time_interval += time_interval;
                total_data_size += data_size;
                average_rate = total_data_size * 1000.0 / total_time_interval;

                if (task_num_current > 0)
                    current_latency = time_consumed_current / task_num_current;
                else
                    current_latency = time_interval;
                if (task_num_total > 0)
                    average_latency = time_consumed_total / task_num_total;
                else
                    average_latency = time_interval;

                update_data(current_rate, average_rate, 
                                current_latency, average_latency); // callback for forms data update

                Console.WriteLine("current_rate = " + current_rate + " Kb/s" +
                            "\taverage_rate = " + average_rate + "Kb/s");
                ResetData();
            }
        }

        private void ResetData()
        {
            data_size = 0;
            current_rate = 0;

            task_num_current = 0;
            time_consumed_current = 0;
            current_latency = 0;
        }

        private void RestDataAll()
        {
            data_size = 0;
            average_rate = 0;
            current_rate = 0;
            first_update = 0;

            task_num_current = task_num_total = 0;
            time_consumed_current = time_consumed_total = 0;
            average_latency = current_latency = 0;
        }
    }
}
