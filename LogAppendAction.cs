using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VcClient;
using VcClientExceptions;

namespace WinForm_StressTest
{
    class LogAppendAction : Action
    {
        public LogAppendAction(int threads, ref TokenController Controller, ref RateProfile Profile, 
                                string DestStream, string SourceStream, 
                                MessageNotify msg_notify)
            : base(threads, ref Controller, ref Profile, DestStream, SourceStream, msg_notify)
        {
        }

        public override void ActionOn(object obj)
        {
            byte[] array = Encoding.ASCII.GetBytes(testdata);
            int index = (int)obj;

            while (Run_flags[index])
            {
                if (Controller.RequestToken())
                {
                    try
                    {
                        DateTime start_time = DateTime.Now;

                        VC.LogAppend(deststream, array, false, "date", "2013-09-20");

                        DateTime end_time = DateTime.Now;
                        TimeSpan time_consumed = end_time - start_time;
                        Profile.ProfileIn(testdata_size, time_consumed.TotalMilliseconds);
                    }
                    catch (Exception e)
                    {
                        msg_notify(e.ToString() + "\nThread[" + (int)index + "] Exit!!");
                        return;
                    }
                }
            }
        }
    }
}
