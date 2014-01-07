using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace WinForm_StressTest
{
    class TokenController
    {
        private int token_num;          // total token nums
        private int token_remained;     // remained token nums
        private double token_unit;      // data size a token represents (Kb)
        private uint time_interval;     // time interval of adding tokens (ms)
        private Timer timer;            // timer to control token adding cycle
        private object mylock;
        private bool unlimited_mod_on;

        public TokenController(double data_size_in_Kb,
                    double unit_size_in_Kb, uint time_interval_in_ms = 1000)
        {
            if (data_size_in_Kb <= 0 || unit_size_in_Kb <= 0)
                throw new ArgumentOutOfRangeException();

            token_unit = unit_size_in_Kb;
            token_num = (int)(data_size_in_Kb / unit_size_in_Kb);
            time_interval = time_interval_in_ms;
            mylock = new object();
            unlimited_mod_on = false;
        }

        public TokenController(double unit_size_in_Kb, uint time_interval_in_ms = 1000, bool unlimited_mod = true)
        {
            if (unit_size_in_Kb <= 0)
                throw new ArgumentOutOfRangeException();

            token_unit = unit_size_in_Kb;
            time_interval = time_interval_in_ms;
            mylock = new object();
            unlimited_mod_on = unlimited_mod;
            if (!unlimited_mod_on)
                token_num = 1;
        }

        ~TokenController()
        {
            ControllerStop();
        }

        public void ControllerStart()
        {
            timer = new Timer(TokenAdd, null, 0, time_interval);
        }

        public void ControllerStop()
        {
            if (timer != null)
            {
                timer.Dispose();
                timer = null;
            }
        }

        public bool RequestToken()
        {
            if (timer == null)
                throw new Exception("Controller not started! Do ControllerStart() first.");

            if (unlimited_mod_on)
                return true;

            bool Result = false;
            lock (mylock)
            {
                if (token_remained > 0)
                {
                    Result = true;
                    token_remained--;
                }
            }

            return Result;
        }

        public double GetTokenUnit()
        {
            return token_unit;
        }

        private void TokenAdd(object state)
        {
            lock (mylock)
            {
                token_remained = token_num;
            }
        }
    }
}
