using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VcClient;

namespace WinForm_StressTest
{
    class UploadAction : Action
    {
        public UploadAction(int threads, ref TokenController Controller, ref RateProfile Profile,
                                string DestStream, string SourceStream,
                                MessageNotify exception_notify)
            : base(1, ref Controller, ref Profile, DestStream, SourceStream, exception_notify) 
                // limit to 1 threads here
        { 
        }

        public override void ActionOn(object index)
        {          
            try
            {
                if (VC.StreamExists(deststream))
                {
                    msg_notify("DestStream File Exists!!");
                    return;
                }
                VC.Upload(sourcestream, deststream, false);
            }
            catch (Exception e)
            {
                msg_notify(e.ToString() + "\nThread[" + (int)index + "] Exit!!");               
                return;
            }

            msg_notify("Upload completed!!");
        }
    }
}
