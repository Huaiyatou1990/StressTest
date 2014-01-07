using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VcClient;

namespace WinForm_StressTest
{
    class DownloadAction : Action
    {
        public DownloadAction(int threads, ref TokenController Controller, ref RateProfile Profile,
                                string DestStream, string SourceStream,
                                MessageNotify exception_notify)
            : base(threads, ref Controller, ref Profile, DestStream, SourceStream, exception_notify)
        {
        }

        public override void ActionOn(object obj)
        {
            int index = (int)obj;
            string deststreampath = deststream + index;   // in case of deststream confliction among threads   

            try
            {
                if (!VC.StreamExists(sourcestream))
                {
                    msg_notify("source stream not exists!!");
                    return;
                }
            }
            catch (Exception e)
            {
                msg_notify(e.ToString() + "\nThread[" + (int)index + "] Exit!!");
                return;
            }                 

            while (Run_flags[index])
            {
                if (Controller.RequestToken())
                {
                    try
                    {
                        DateTime start_time = DateTime.Now;
                        VC.Download(sourcestream, deststreampath, 0, testdata_size * 1024, false, true);
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
